using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Web.Areas.CampaignManagement.Models.Campaign;
using Core.Persistence;

namespace Web.Services
{
    public class ProcessProductLevelRequestStrategy
    {


        private readonly IQueryService<Contact> queryServiceContact;

        private readonly IQueryService<Outpost> queryServiceOutpost;

        private readonly IQueryService<OutpostStockLevel> queryServiceStockLevel;

        private ProductFilterByProductLevelRequest productFilter;

        public ProcessProductLevelRequestStrategy(IQueryService<Contact> queryServiceContact,
            IQueryService<OutpostStockLevel> queryServiceStockLevel,
            IQueryService<Outpost> queryServiceOutpost)
        {
            this.queryServiceStockLevel = queryServiceStockLevel;
            this.queryServiceOutpost = queryServiceOutpost;
            this.queryServiceContact = queryServiceContact;
        }

        public void Process(ProductLevelRequest productLevelRequest, Action<ProductLevelRequestMessageInput> execute)
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
                    Products = products,
                    ProductLevelRequest = productLevelRequest
                };

                execute(input);
            }
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
    }
}