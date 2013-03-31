using System;
using System.Collections.Generic;
using System.Linq;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.Campaign;

namespace Web.Services
{
    public class ProductLevelRequestMessagesDispatcherService : IProductLevelRequestMessagesDispatcherService
    {

        private readonly ProcessProductLevelRequestStrategy processProductLevelStrategy;

        private readonly ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord;

        private readonly List<IProductLevelRequestMessageSenderService> senderServices;


        public ProductLevelRequestMessagesDispatcherService(
            ProcessProductLevelRequestStrategy processProductLevelStrategy,
            ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord,
            IEnumerable<IProductLevelRequestMessageSenderService> senderServices)
        {
            this.processProductLevelStrategy = processProductLevelStrategy;
            this.saveOrUpdateRequestRecord = saveOrUpdateRequestRecord;
            this.senderServices = new List<IProductLevelRequestMessageSenderService>(senderServices);
        }

        public void DispatchMessagesForProductLevelRequest(ProductLevelRequest productLevelRequest)
        {
            processProductLevelStrategy.Process(productLevelRequest, DispatchProductLevelRequestMessages);
        }

        private void DispatchProductLevelRequestMessages(ProductLevelRequestMessageInput input)
        {
            foreach (IProductLevelRequestMessageSenderService service in senderServices)
            {
                if (service.SendProductLevelRequestMessage(input))
                {
                    RequestRecord executedCampaign = GenerateRequestInfos(input);
                    saveOrUpdateRequestRecord.Execute(executedCampaign);
                }
            }
        }

        private RequestRecord GenerateRequestInfos(ProductLevelRequestMessageInput input)
        {
            var campaign = input.ProductLevelRequest.Campaign;

            RequestRecord requestRecord = new RequestRecord();
            requestRecord.ProductLevelRequestId = input.ProductLevelRequest.Id;
            requestRecord.CampaignId = campaign.Id;
            requestRecord.CampaignName = campaign.Name;
            requestRecord.Client = input.Client;
            requestRecord.ByUser = input.ByUser;
            requestRecord.Created = DateTime.UtcNow;
            requestRecord.ProductGroupId = input.ProductGroup.Id;
            requestRecord.ProductGroupName = input.ProductGroup.Name;
            requestRecord.ProductsNo = input.Products.Count;
            requestRecord.OutpostId = input.Outpost.Id;
            requestRecord.OutpostName = input.Outpost.Name;

            return requestRecord;
        }

       
    }
}