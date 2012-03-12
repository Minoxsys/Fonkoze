using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBackgrounder;
using System.Threading.Tasks;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Services;

namespace Web.BackgroundJobs
{
    public class CampaignExecutionJob : IJob
    {
        private const string JOB_NAME = "CampaignExecutionJob";
        public TimeSpan Interval { get { return TimeSpan.FromSeconds(70); } }
        public TimeSpan Timeout { get { return TimeSpan.FromSeconds(130); } }
        public string Name { get { return JOB_NAME; } }

        private readonly Func<IQueryService<ProductLevelRequest>> queryProductLevelRequests;
        private readonly Func<IQueryService<RequestRecord>> queryExistingRequests;
        private readonly Func<IProductLevelRequestMessagesDispatcherService> dispatcherService;

        public CampaignExecutionJob(
           Func<IQueryService<ProductLevelRequest>> queryProductLevelRequests,
           Func<IQueryService<RequestRecord>> queryExecutedCampaign,
           Func<IProductLevelRequestMessagesDispatcherService> dispatcherService)
        {
            this.queryProductLevelRequests = queryProductLevelRequests;
            this.queryExistingRequests = queryExecutedCampaign;
            this.dispatcherService = dispatcherService;
        }

        public System.Threading.Tasks.Task Execute()
        {
            return new Task(() =>
            {
                List<ProductLevelRequest> needToBeExecuted = GetListOfProductLevelRequestsThatNeedToBeExecuted();
                var dispatcher = dispatcherService();
                foreach (var productLevelRequest in needToBeExecuted)
                {
                    dispatcher.DispatchMessagesForProductLevelRequest(productLevelRequest);
                }

            });
        }

        private List<ProductLevelRequest> GetListOfProductLevelRequestsThatNeedToBeExecuted()
        {
            List<ProductLevelRequest> needToBeExecuted = new List<ProductLevelRequest>();

            var productlevelrequests = queryProductLevelRequests().Query()
                .Where(it => it.Campaign.StartDate.Value.Date <= DateTime.UtcNow.Date)
                .Where(it => it.IsStopped == false)
                .Where(it => it.Schedule != null)
                .Where(it => it.Campaign.Opened == true).ToList();


            foreach (var productLevelRequest in productlevelrequests)
            {
                var lastExecuted = GetLastExecuted(productLevelRequest);

                if (!lastExecuted.HasValue && 
                    DateTime.UtcNow.Date >= NextExecutionDate(productLevelRequest.Created.Value, productLevelRequest.Schedule.FrequencyValue)) //Has not been Executed yet
                {
                    needToBeExecuted.Add(productLevelRequest);
                }

                if ( lastExecuted.HasValue &&
                    lastExecuted.Value.Date != DateTime.UtcNow.Date &&
                    DateTime.UtcNow.Date == NextExecutionDate(lastExecuted.Value, productLevelRequest.Schedule.FrequencyValue) )
                {
                    needToBeExecuted.Add(productLevelRequest);
                }
            }

            return needToBeExecuted;
        }

        private static DateTime NextExecutionDate(DateTime date, int frequencyValue)
        {
            return date.AddDays(frequencyValue).Date;
        }

        private DateTime? GetLastExecuted(ProductLevelRequest productLevelRequest)
        {
            var existingRequests = queryExistingRequests().Query()
                .Where(it => it.ProductLevelRequestId == productLevelRequest.Id)
                .OrderByDescending(it => it.Created);

            if (existingRequests.Any())
                return existingRequests.FirstOrDefault().Created;

            return null;
        }

    }
}