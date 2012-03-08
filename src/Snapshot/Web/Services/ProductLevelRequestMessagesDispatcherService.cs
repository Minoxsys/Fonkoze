using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.Persistence;
using Domain;
using Web.Areas.CampaignManagement.Models.Campaign;

namespace Web.Services
{
    public class ProductLevelRequestMessagesDispatcherService : IProductLevelRequestMessagesDispatcherService
    {
        private List<IProductLevelRequestMessageSenderService> senderServices;

        IQueryService<Outpost> queryServiceOutpost;
        private IQueryService<Contact> queryServiceContact;
        private IQueryService<OutpostStockLevel> queryServiceStockLevel;
        private IProductFilter productFilter;
        private ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord;

        public ProductLevelRequestMessagesDispatcherService(IQueryService<Outpost> queryServiceOutpost, IQueryService<Contact> queryServiceContact,
            IQueryService<OutpostStockLevel> QueryServiceStockLevel, ISaveOrUpdateCommand<RequestRecord> saveOrUpdateRequestRecord, IEnumerable<IProductLevelRequestMessageSenderService> senderServices)
        {
            this.queryServiceOutpost = queryServiceOutpost;
            this.queryServiceContact = queryServiceContact;
            this.queryServiceStockLevel = QueryServiceStockLevel;
            this.saveOrUpdateRequestRecord = saveOrUpdateRequestRecord;
            this.senderServices = new List<IProductLevelRequestMessageSenderService>(senderServices);
        }

        public void AddMessageSenderService(IProductLevelRequestMessageSenderService service)
        {
            if (senderServices == null)
            {
                senderServices = new List<IProductLevelRequestMessageSenderService>();
            }

            senderServices.Add(service);
        }

        public void DispatchMessagesForProductLevelRequest(ProductLevelRequest productLevelRequest)
        {
            productFilter = new ProductFilterByProductLevelRequest(productLevelRequest);
            List<Guid> outpostIds = GetOutpostIdsFromCampaign(productLevelRequest.Campaign);

            foreach (Guid outpostId in outpostIds)
            {
                Contact contact = queryServiceContact.Query().Where(c => c.Outpost.Id == outpostId && c.IsMainContact).FirstOrDefault();
                List<Product> products = GetProductsForOutpostId(outpostId);

                Outpost outpost = queryServiceOutpost.Load(outpostId);

                ProductLevelRequestMessageInput input = new ProductLevelRequestMessageInput
                {
                    Client = productLevelRequest.Client,
                    ByUser = productLevelRequest.ByUser,
                    Contact = contact,
                    Outpost = outpost,
                    ProductGroup = productLevelRequest.ProductGroup,
                    Products = products
                };

                DispatchProductLevelRequestMessages(input, productLevelRequest.Campaign, productLevelRequest.Id);
            }
        }

        private void DispatchProductLevelRequestMessages(ProductLevelRequestMessageInput input, Campaign campaign, Guid productLevelRequestId)
        {
            foreach (IProductLevelRequestMessageSenderService service in senderServices)
            {
                if (service.SendProductLevelRequestMessage(input))
                {
                    RequestRecord executedCampaign = GenerateRequestInfos(input, campaign, productLevelRequestId);
                    saveOrUpdateRequestRecord.Execute(executedCampaign);
                }
            }
        }

        private RequestRecord GenerateRequestInfos(ProductLevelRequestMessageInput input, Campaign campaign, Guid productLevelRequestId)
        {
            RequestRecord requestRecord = new RequestRecord();
            requestRecord.ProductLevelRequestId = productLevelRequestId;
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

        private List<Product> GetProductsForOutpostId(Guid outpostId)
        {
            List<OutpostStockLevel> outpostStockLevels = queryServiceStockLevel.Query().Where(osl => osl.Outpost.Id == outpostId).ToList();
            List<Product> products = new List<Product>();

            foreach (OutpostStockLevel osl in outpostStockLevels)
            {
                if (productFilter.CheckProduct(osl.Product))
                {
                    products.Add(osl.Product);
                }
            }
            return products;
        }

        private List<Guid> GetOutpostIdsFromCampaign(Campaign campaign)
        {
            List<Guid> outpostIds = new List<Guid>();

            foreach (string outpostIdString in campaign.RestoreOptions<OptionsModel>().Outposts.Split(',').ToList())
            {
                Guid outpostId;
                if (Guid.TryParse(outpostIdString, out outpostId))
                {
                    outpostIds.Add(outpostId);
                }
            }

            return outpostIds;
        }
    }
}