using Core.Persistence;
using Domain;
using System.Linq;
using Web.LocalizationResources;

namespace Web.Services
{
    public class StoreProductLevelRequestDetailService
    {

        private readonly ISaveOrUpdateCommand<ProductLevelRequestDetail> _saveOrUpdateCommand;
        private readonly IQueryService<ProductLevelRequestDetail> _queryService;
        private readonly IDeleteCommand<ProductLevelRequestDetail> _deleteCommand;

        public StoreProductLevelRequestDetailService(
            IQueryService<ProductLevelRequestDetail> queryService,
            IDeleteCommand<ProductLevelRequestDetail> deleteCommand,
            ISaveOrUpdateCommand<ProductLevelRequestDetail> saveOrUpdateCommand)
        {
            _deleteCommand = deleteCommand;
            _queryService = queryService;
            _saveOrUpdateCommand = saveOrUpdateCommand;
        }

        public bool StoreProductLevelRequestDetail(ProductLevelRequestMessageInput input)
        {
            var oldProductLevelDetail = _queryService.Query()
                                                     .Where(p => p.ProductLevelRequestId == input.ProductLevelRequest.Id)
                                                     .Where(p => p.OutpostName.StartsWith(input.Outpost.Name))
                                                     .FirstOrDefault(p => p.ProductGroupName == input.ProductGroup.Name);

            if (oldProductLevelDetail != null)
            {
                _deleteCommand.Execute(oldProductLevelDetail);
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

            _saveOrUpdateCommand.Execute(productLevelDetail);
            return true;
        }

        private string GetContactMethod(ProductLevelRequestMessageInput input)
        {
            var contactNethod = input.Contact == null
                                    ? "No main contact method"
                                    : string.Format("{0} ({1})", input.Contact.ContactType, input.Contact.ContactDetail);

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
                return
                    Strings
                        .StoreProductLevelRequestDetailService_GetReqMessage_No_product_s__for_this_seller_matched_those_assigned_to_this_product_level_request;
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