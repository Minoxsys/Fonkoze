using Core.Persistence;
using Domain;
using Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.Alerts;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class AddAlertsJob : IJob
    {
        private readonly Func<ISaveOrUpdateCommand<Alert>> _saveOrUpdateCommand;
        private readonly Func<IQueryService<Alert>> _queryAlerts;
        private readonly Func<IQueryService<OutpostStockLevel>> _queryOutpostStockLevel;
        private readonly Func<IQueryService<Client>> _queryClients;

        private const string JobName = "AddAlertsJob";

        public TimeSpan Interval
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        public TimeSpan Timeout
        {
            get { return TimeSpan.FromSeconds(90); }
        }

        public string Name
        {
            get { return JobName; }
        }

        public AddAlertsJob(Func<ISaveOrUpdateCommand<Alert>> saveOrUpdateCommand, Func<IQueryService<Alert>> queryAlerts,
                            Func<IQueryService<OutpostStockLevel>> queryOutpostStockLevel, Func<IQueryService<Client>> queryClients)
        {
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
                                 ClientId = outpostStockLevel.Client.Id
                             }).ToList();

                    foreach (var f in foos)
                    {
                        if (!ExistsAlert(f.OutpostStockLevelId, f.LastUpdated))
                        {
                            Alert alert = CreateAlert(f);
                            _saveOrUpdateCommand().Execute(alert);
                        }
                    }
                });
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