﻿using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.LocalizationResources;
using Web.Models.Alerts;
using Web.Services;
using Web.Services.SendEmail;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class StockLevelsMonitoringJob : IJob
    {
        private readonly Func<ISaveOrUpdateCommand<Alert>> _saveOrUpdateCommand;
        private readonly Func<IQueryService<Alert>> _queryAlerts;
        private readonly Func<IQueryService<OutpostStockLevel>> _queryOutpostStockLevel;
        private readonly Func<IQueryService<Client>> _queryClients;
        private readonly PreconfiguredEmailService _preconfiguredEmailService;
        private readonly ISendSmsService _sendSmsService;

        private const string JobName = "StockLevelsMonitoringJob";

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMinutes(14); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromMinutes(5); }
        }

        public string Name
        {
            get { return JobName; }
        }

        public StockLevelsMonitoringJob(Func<ISaveOrUpdateCommand<Alert>> saveOrUpdateCommand, Func<IQueryService<Alert>> queryAlerts,
                            Func<IQueryService<OutpostStockLevel>> queryOutpostStockLevel, Func<IQueryService<Client>> queryClients,
                            PreconfiguredEmailService preconfiguredEmailService, ISendSmsService sendSmsService)
        {
            _sendSmsService = sendSmsService;
            _preconfiguredEmailService = preconfiguredEmailService;
            _saveOrUpdateCommand = saveOrUpdateCommand;
            _queryAlerts = queryAlerts;
            _queryOutpostStockLevel = queryOutpostStockLevel;
            _queryClients = queryClients;
        }

        public Task Execute()
        {
            return new Task(() =>
                {
                    var foos =
                        (from outpostStockLevel in
                             _queryOutpostStockLevel().Query().Where(it => it.Updated.Value >= DateTime.UtcNow.AddHours(-2) && it.Created != it.Updated)
                         where
                             outpostStockLevel.StockLevel <= outpostStockLevel.Product.LowerLimit
                         select new AlertOutputModel
                             {
                                 OutpostId = outpostStockLevel.Outpost.Id,
                                 OutpostName = outpostStockLevel.Outpost.Name,
                                 ProductGroupId = outpostStockLevel.ProductGroup.Id,
                                 ProductGroupName = outpostStockLevel.ProductGroup.Name,
                                 OutpostStockLevelId = outpostStockLevel.Id,
                                 Contact = outpostStockLevel.Outpost.DetailMethod,
                                 StockLevel = outpostStockLevel.StockLevel,
                                 LastUpdated = outpostStockLevel.Updated,
                                 RefCode = outpostStockLevel.Product.SMSReferenceCode,
                                 ClientId = outpostStockLevel.Client.Id,
                                 ProductLimit = outpostStockLevel.Product.LowerLimit,
                                 DistrictManagerPhoneNumber =
                                     outpostStockLevel.Outpost.District.DistrictManager != null
                                         ? outpostStockLevel.Outpost.District.DistrictManager.PhoneNumber
                                         : string.Empty
                             }).ToList();

                    foreach (var f in foos)
                    {
                        if (!ExistsAlert(f.OutpostStockLevelId, f.LastUpdated))
                        {
                            Alert alert = CreateAlert(f);
                            _saveOrUpdateCommand().Execute(alert);

                            SendEmailToCentralAccount(f);
                            SendSmsMessageToDistrictManager(f);
                        }
                    }
                });
        }

        private void SendSmsMessageToDistrictManager(AlertOutputModel alertOutputModel)
        {
            if (!string.IsNullOrEmpty(alertOutputModel.DistrictManagerPhoneNumber))
            {
                _sendSmsService.SendSms(alertOutputModel.DistrictManagerPhoneNumber, BuildSmsMessage(alertOutputModel), true);
            }
        }

        private string BuildSmsMessage(AlertOutputModel model)
        {
            return string.Format(Strings.StockBellowLimitSmsBody, Environment.NewLine,
                                 model.OutpostName, model.RefCode, model.StockLevel, model.Contact);
        }

        private void SendEmailToCentralAccount(AlertOutputModel model)
        {
            var message = _preconfiguredEmailService.CreatePartialMailMessageFromConfig();
            message.Subject = Strings.StockBellowLimitEmailSubject;
            message.Body = string.Format(Strings.StockBellowLimitEmailBody, Environment.NewLine,
                                         model.OutpostName, model.RefCode, model.StockLevel, model.Contact, Strings.AutoGeneratedEmail, model.ProductLimit);

            _preconfiguredEmailService.SendEmail(message);
        }

        private bool ExistsAlert(Guid outpostStockLevelId, DateTime? lastUpdated)
        {
            return _queryAlerts().Query().Any(it => it.OutpostStockLevelId == outpostStockLevelId && it.Created >= lastUpdated);
        }

        private Alert CreateAlert(AlertOutputModel f)
        {
            return new Alert
                {
                    Client = _queryClients().Load(f.ClientId),
                    Contact = f.Contact,
                    LastUpdate = f.LastUpdated,
                    LowLevelStock = f.RefCode + " - " + f.StockLevel,
                    OutpostId = f.OutpostId,
                    OutpostName = f.OutpostName,
                    ProductGroupId = f.ProductGroupId,
                    ProductGroupName = f.ProductGroupName,
                    OutpostStockLevelId = f.OutpostStockLevelId,
                    Created = DateTime.UtcNow,
                    AlertType = AlertType.StockLevel
                };
        }
    }
}