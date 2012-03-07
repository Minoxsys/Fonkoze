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

namespace Web.BackgroundJobs
{
    public class CampaignExecutionJob : IJob
    {
        private const string JOB_NAME = "CampaignExecutionJob";
        public TimeSpan Interval { get { return TimeSpan.FromSeconds(70); } }
        public TimeSpan Timeout { get { return TimeSpan.FromSeconds(130); } }
        public string Name { get { return JOB_NAME; } }

        private readonly IQueryService<ProductLevelRequest> queryProductLevelRequests;
        private readonly IQueryService<RequestRecord> queryExistingRequests;
        private readonly IQueryService<Outpost> queryOutposts;
        private readonly ISaveOrUpdateCommand<RequestRecord> saveOrUpdateCommand;

        public CampaignExecutionJob(IQueryService<ProductLevelRequest> queryProductLevelRequests, IQueryService<RequestRecord> queryExecutedCampaign, ISaveOrUpdateCommand<RequestRecord> saveOrUpdateCommand, IQueryService<Outpost> queryOutposts)
        {
            this.queryProductLevelRequests = queryProductLevelRequests;
            this.queryExistingRequests = queryExecutedCampaign;
            this.saveOrUpdateCommand = saveOrUpdateCommand;
            this.queryOutposts = queryOutposts;
        }

        public System.Threading.Tasks.Task Execute()
        {
            return new Task(() =>
            {
                List<ProductLevelRequest> needToBeExecuted = GetListOfProductLevelRequestsThatNeedToBeExecuted();
                foreach (var productLevelRequest in needToBeExecuted)
                {
                    //Execute(productLevelRequest);
                    SaveLastExecutionFor(productLevelRequest);
                }

            });
        }

        private List<ProductLevelRequest> GetListOfProductLevelRequestsThatNeedToBeExecuted()
        {
            List<ProductLevelRequest> needToBeExecuted = new List<ProductLevelRequest>();

            var productlevelrequests = queryProductLevelRequests.Query()
                .Where(it => it.Campaign.StartDate.Value.Date <= DateTime.UtcNow.Date)
                .Where(it => it.IsStopped == false)
                .Where(it => it.Schedule != null)
                .Where(it => it.Campaign.Opened == true).ToList();


            foreach (var productLevelRequest in productlevelrequests)
            {
                var lastExecuted = GetLastExecuted(productLevelRequest);
                if (!lastExecuted.HasValue) //Has not been Executed yet
                {
                    needToBeExecuted.Add(productLevelRequest);
                }
                else
                {
                    if (DateTime.UtcNow.Date == lastExecuted.Value.AddDays(productLevelRequest.Schedule.FrequencyValue).Date && lastExecuted.Value.Date != DateTime.UtcNow.Date)
                    {
                        needToBeExecuted.Add(productLevelRequest);
                    }
                }
            }

            return needToBeExecuted;
        }

        private DateTime? GetLastExecuted(ProductLevelRequest productLevelRequest)
        {
            var existingRequests = queryExistingRequests.Query()
                .Where(it => it.CampaignId == productLevelRequest.Campaign.Id)
                .Where(it => it.ProductGroupId == productLevelRequest.ProductGroup.Id)
                .OrderByDescending(it => it.Created);

            if (existingRequests.Any())
                return existingRequests.FirstOrDefault().Created;

            return null;
        }

        private void SaveLastExecutionFor(ProductLevelRequest productLevelRequest)
        {
            List<Outpost> outposts = GetListOfOutpostsFrom(productLevelRequest.Campaign.Options);
            foreach (var outpost in outposts)
            {
                RequestRecord executedCampaign = GenerateRequestInfos(productLevelRequest);
                executedCampaign.OutpostId = outpost.Id;
                executedCampaign.OutpostName = outpost.Name;

                saveOrUpdateCommand.Execute(executedCampaign);
            }
        }

        private RequestRecord GenerateRequestInfos(ProductLevelRequest productLevelRequest)
        {
            RequestRecord executedCampaign = new RequestRecord();
            executedCampaign.CampaignId = productLevelRequest.Campaign.Id;
            executedCampaign.CampaignName = productLevelRequest.Campaign.Name;
            executedCampaign.Client = productLevelRequest.Client;
            executedCampaign.ByUser = productLevelRequest.ByUser;
            executedCampaign.Created = DateTime.UtcNow;
            executedCampaign.ProductGroupId = productLevelRequest.ProductGroup.Id;
            executedCampaign.ProductGroupName = productLevelRequest.ProductGroup.Name;
            executedCampaign.ProductsNo = GetNumberOfProducts(productLevelRequest);
            return executedCampaign;
        }

        private int GetNumberOfProducts(ProductLevelRequest productLevelRequest)
        {
            int noofProducts = 0;
            var products = productLevelRequest.RestoreProducts<ProductModel[]>();
            foreach (var product in products)
                if (product.Selected)
                    noofProducts++;
            return noofProducts;
        }

        private List<Outpost> GetListOfOutpostsFrom(byte[] options)
        {
            List<Outpost> outposts = new List<Outpost>();

            OptionsModel model = ConvertFromJson(ByteArrayToStr(options));
            string[] outpostIds = model.Outposts.Split(',');

            foreach (var outpostId in outpostIds)
            {
                if (! string.IsNullOrEmpty(outpostId))
                {
                    Outpost outpost = queryOutposts.Load(new Guid(outpostId));
                    if (outpost != null)
                        outposts.Add(outpost);
                }
            }

            return outposts;
        }

        private OptionsModel ConvertFromJson(string json)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<OptionsModel>(json);
        }

        private string ByteArrayToStr(byte[] bites)
        {
            return System.Text.Encoding.UTF8.GetString(bites);
        }
    }
}