using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Persistence;
using Domain;
using Web.Models.Alerts;
using WebBackgrounder;

namespace Web.BackgroundJobs
{
    public class AddAlertsJob : IJob
    {
        private readonly Func<ISaveOrUpdateCommand<Alert>> saveOrUpdateCommand;
        private readonly Func<IQueryService<Alert>> queryAlerts;
        private readonly Func<IQueryService<OutpostStockLevel>> queryOutpostStockLevel;
        private readonly Func<IQueryService<Client>> queryClients;

        private const string JOB_NAME = "AddAlertsJob";
        public TimeSpan Interval { get { return TimeSpan.FromMinutes(1); } }
        public TimeSpan Timeout { get { return TimeSpan.FromSeconds(90); } }
        public string Name { get { return JOB_NAME; } }

        public AddAlertsJob(Func<ISaveOrUpdateCommand<Alert>> saveOrUpdateCommand, Func<IQueryService<Alert>> queryAlerts,
            Func<IQueryService<OutpostStockLevel>> queryOutpostStockLevel, Func<IQueryService<Client>> queryClients)
        {
            this.saveOrUpdateCommand = saveOrUpdateCommand;
            this.queryAlerts = queryAlerts;
            this.queryOutpostStockLevel = queryOutpostStockLevel;
            this.queryClients = queryClients;
        }

        public System.Threading.Tasks.Task Execute()
        {
            return new Task(() =>
            {
                var foos = (from outpostStockLevel in queryOutpostStockLevel().Query().Where(it => it.Updated.Value >= DateTime.UtcNow.AddHours(-2) && it.Created != it.Updated)
                            where outpostStockLevel.StockLevel <= outpostStockLevel.Product.LowerLimit || outpostStockLevel.StockLevel >= outpostStockLevel.Product.UpperLimit
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


                List<Alert> alerts = new List<Alert>();
                foreach (var f in foos)
                {
                    if (!ExistsAlert(f.OutpostStockLevelId, f.LastUpdated))
                    {
                        Alert alert = CreateAlert(f);
                        saveOrUpdateCommand().Execute(alert);
                    }
                }

            });
        }

        private bool ExistsAlert(Guid outpostStockLevelId, DateTime? lastUpdated)
        {
            return queryAlerts().Query().Where(it => it.OutpostStockLevelId == outpostStockLevelId && it.Created >= lastUpdated).Any();
        }

        private Alert CreateAlert(AlertOutputModel f)
        {
            return new Alert()
            {
                Client = queryClients().Load(f.ClientId),
                Contact = f.Contact,
                LastUpdate = f.LastUpdated,
                LowLevelStock = f.RefCode + " - " + f.StockLevel,
                OutpostId = f.OutpostId,
                OutpostName = f.OutpostName,
                ProductGroupId = f.ProductGroupId,
                ProductGroupName = f.ProductGroupName,
                OutpostStockLevelId = f.OutpostStockLevelId,
                Created = DateTime.UtcNow
            };
        }


    }
}