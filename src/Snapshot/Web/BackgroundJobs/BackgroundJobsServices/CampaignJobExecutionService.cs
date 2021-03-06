﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using Domain;
using Web.Services;

namespace Web.BackgroundJobs.BackgroundJobsServices
{
    public class CampaignJobExecutionService: IJobExecutionService
    {
        private readonly IQueryService<ProductLevelRequest> _queryProductLevelRequests;
        private readonly IQueryService<RequestRecord> _queryExistingRequests;
        private readonly IProductLevelRequestMessagesDispatcherService _dispatcherService;

        public CampaignJobExecutionService(IQueryService<ProductLevelRequest> queryProductLevelRequests,
                                           IQueryService<RequestRecord> queryExecutedCampaign,
                                           IProductLevelRequestMessagesDispatcherService dispatcherService)
        {
            _queryProductLevelRequests = queryProductLevelRequests;
            _queryExistingRequests = queryExecutedCampaign;
            _dispatcherService = dispatcherService;
        }

        public void ExecuteJob()
        {
            IEnumerable<ProductLevelRequest> needToBeExecuted = GetListOfProductLevelRequestsThatNeedToBeExecuted();
            var dispatcher = _dispatcherService;
            foreach (var productLevelRequest in needToBeExecuted)
            {
                dispatcher.DispatchMessagesForProductLevelRequest(productLevelRequest);
            }
        }


        private IEnumerable<ProductLevelRequest> GetListOfProductLevelRequestsThatNeedToBeExecuted()
        {
            var needToBeExecuted = new List<ProductLevelRequest>();

            var productlevelrequests = _queryProductLevelRequests.Query()
                                                                   .Where(it => it.Campaign.StartDate.Value.Date <= DateTime.UtcNow.Date)
                                                                   .Where(it => it.IsStopped == false)
                                                                   .Where(it => it.Schedule != null)
                                                                   .Where(it => it.Campaign.Opened).ToList();


            foreach (var productLevelRequest in productlevelrequests)
            {
                var lastExecuted = GetLastExecuted(productLevelRequest);

                if (productLevelRequest.Created.HasValue && (!lastExecuted.HasValue &&
                                                             DateTime.UtcNow.Date >=
                                                             NextExecutionDate(productLevelRequest.Created.Value, productLevelRequest.Schedule.FrequencyValue)))
                //Has not been Executed yet
                {
                    needToBeExecuted.Add(productLevelRequest);
                }

                if (lastExecuted.HasValue &&
                    lastExecuted.Value.Date != DateTime.UtcNow.Date &&
                    DateTime.UtcNow.Date == NextExecutionDate(lastExecuted.Value, productLevelRequest.Schedule.FrequencyValue))
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
            var existingRequests = _queryExistingRequests.Query()
                                                           .Where(it => it.ProductLevelRequestId == productLevelRequest.Id)
                                                           .OrderByDescending(it => it.Created);

            if (existingRequests.Any())
            {
                var firstOrDefault = existingRequests.FirstOrDefault();
                if (firstOrDefault != null) return firstOrDefault.Created;
            }

            return null;
        }
    }
}