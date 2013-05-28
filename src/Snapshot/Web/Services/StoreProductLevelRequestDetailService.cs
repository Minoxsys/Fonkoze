using System;
using System.Linq;
using Core.Persistence;
using Domain;

namespace Web.Services
{
    public class StoreProductLevelRequestDetailService 
    {

        private readonly ISaveOrUpdateCommand<ProductLevelRequestDetail> saveOrUpdateCommand;
        private readonly IQueryService<ProductLevelRequestDetail> queryService;

        private readonly IDeleteCommand<ProductLevelRequestDetail> deleteCommand;

        public StoreProductLevelRequestDetailService(
            IQueryService<ProductLevelRequestDetail> queryService,
            IDeleteCommand<ProductLevelRequestDetail> deleteCommand,
            ISaveOrUpdateCommand<ProductLevelRequestDetail> saveOrUpdateCommand)
        {
            this.deleteCommand = deleteCommand;
            this.queryService = queryService;
            this.saveOrUpdateCommand = saveOrUpdateCommand;
        }

        public bool StoreProductLevelRequestDetail(ProductLevelRequestMessageInput input)
        {
            var oldProductLevelDetail = this.queryService.Query()
                .Where(p => p.ProductLevelRequestId == input.ProductLevelRequest.Id)
                .Where(p => p.OutpostName.StartsWith( input.Outpost.Name ))
                .Where(p => p.ProductGroupName == input.ProductGroup.Name).FirstOrDefault();

            if (oldProductLevelDetail != null)
            {
                deleteCommand.Execute(oldProductLevelDetail);
                oldProductLevelDetail = null;
            }

            var productLevelDetail = new ProductLevelRequestDetail
            {

                OutpostName = ToOutpostName(input.Outpost),
                ProductGroupName = input.ProductGroup.Name,
                ProductLevelRequestId = input.ProductLevelRequest.Id,
                ByUser = input.ByUser,
                Method = GetContactMethod(input),
                RequestMessage = GetReqMessage(input)

            };

            this.saveOrUpdateCommand.Execute(productLevelDetail);
            return true;
        }

        private string GetContactMethod(ProductLevelRequestMessageInput input)
        {
            var contactNethod = input.Contact == null ? "No main contact method" : string.Format("{0} ({1})",input.Contact.ContactType, input.Contact.ContactDetail);

            if (input.Products == null || input.Products.Count == 0)
            {
                return "None";

            }

            return contactNethod;
        }

        private static string ToOutpostName(Outpost outpost)
        {
            return string.Format("{0} ( {1}/{2}/{3} )", outpost.Name, outpost.Country.Name, outpost.Region.Name, outpost.District.Name);
        }

        private string GetReqMessage(ProductLevelRequestMessageInput input)
        {
            if (input.Contact == null)
            {
                return "-";
            }

            if (input.Products == null || input.Products.Count == 0)
            {
                return "No product(s) for this outpost matched those assigned to this product level request";
            }

            switch (input.Contact.ContactType)
            {
                case Contact.MOBILE_NUMBER_CONTACT_TYPE:
                    return new FormattingStrategy().FormatSms(input);
            }
            return string.Empty;
        }
    }
}