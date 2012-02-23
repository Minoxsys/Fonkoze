using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Core.Persistence;
using Persistence.Queries.Products;
using System.Text;

namespace Web.Services
{
    public class SmsRequestService : ISmsRequestService
    {
        public const string MESSAGE_NOT_DELIVERED = "Message not delivered";
        public const string SMS_UPDATED_METHOD = "SMS";
        private const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n{1} {2}";

        private IQueryService<Outpost> queryServiceOutpost;
        private IQueryService<ProductGroup> queryServiceProductGroup;
        private IQueryService<OutpostStockLevel> queryServiceStockLevel;
        private IQueryService<SmsRequest> queryServiceSmsRequest;
        private IOutpostStockLevelService outpostStockLevelService;

        private ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest;
        private ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel;

        private ProductGroup productGroup;
        private Outpost outpost;

        public SmsRequestService(IQueryService<Outpost> queryServiceOutpost, IQueryService<ProductGroup> queryServiceProductGroup,
            IQueryService<OutpostStockLevel> queryServiceStockLevel, IQueryService<SmsRequest> queryServiceSmsRequest,
            ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest, IOutpostStockLevelService outpostStockLevelService,
            ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel)
        {
            this.queryServiceOutpost = queryServiceOutpost;
            this.queryServiceProductGroup = queryServiceProductGroup;
            this.queryServiceStockLevel = queryServiceStockLevel;
            this.saveCommandSmsRequest = saveCommandSmsRequest;
            this.queryServiceSmsRequest = queryServiceSmsRequest;
            this.outpostStockLevelService = outpostStockLevelService;
            this.saveCommandOutpostStockLevel = saveCommandOutpostStockLevel;
        }

        public SmsRequest CreateSmsRequestUsingOutpostIdAndProductGroupIdForClient(Guid outpostId, Guid productGroupId, Client client)
        {
            outpost = queryServiceOutpost.Load(outpostId);
            productGroup = queryServiceProductGroup.Load(productGroupId);

            SmsRequest smsRequest = new SmsRequest();
            string numbers = GetNumberFromOutpost();

            if (!string.IsNullOrEmpty(numbers))
            {
                smsRequest.Client = client;
                smsRequest.Number = numbers;
                smsRequest.OutpostId = outpostId;
                smsRequest.ProductGroupId = productGroupId;
                smsRequest.ProductGroupReferenceCode = productGroup.ReferenceCode;

                smsRequest.Message = MESSAGE_NOT_DELIVERED;
                saveCommandSmsRequest.Execute(smsRequest);

                smsRequest.Message = string.Format(SMS_MESSAGE_TEMPLATE, productGroup.Name, productGroup.ReferenceCode, GetProductReferenceCodesAndStockLevelsFromOutpostAndProductGroup(outpostId, productGroupId));
                saveCommandSmsRequest.Execute(smsRequest);
            }

            return smsRequest;
        }

        public void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived)
        {
            var stockLevels = GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(smsReceived.ProductGroupReferenceCode, smsReceived.Number);

            foreach (OutpostStockLevel outpostStockLevel in stockLevels)
            {
                UpdateSingleOutpostStockLevelWithValuesReceivedBySms(outpostStockLevel, smsReceived);
            }
        }

        private List<OutpostStockLevel> GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(string productGroupReferenceCode, string phoneNumber)
        {
            List<OutpostStockLevel> stockLevels = new List<OutpostStockLevel>();
            SmsRequest smsRequest = queryServiceSmsRequest.Query().Where(r => r.ProductGroupReferenceCode == productGroupReferenceCode && r.Number == phoneNumber).OrderByDescending(r => r.Created).FirstOrDefault();

            if (smsRequest != null)
            {
                stockLevels.AddRange(GetOutpostStockLevelsByOutpostAndProductGroup(smsRequest.OutpostId, smsRequest.ProductGroupId));
            }

            return stockLevels;
        }

        private string GetNumberFromOutpost()
        {
            Contact contact = outpost.Contacts.Where(c => c.IsMainContact && c.ContactType.Equals("Mobile Number")).FirstOrDefault();
            return contact != null ? contact.ContactDetail : string.Empty;
        }

        private string GetProductReferenceCodesAndStockLevelsFromOutpostAndProductGroup(Guid outpostId, Guid productGroupId)
        {
            StringBuilder result = new StringBuilder();
            var stockLevels = GetOutpostStockLevelsByOutpostAndProductGroup(outpostId, productGroupId);
            stockLevels.ForEach(stockLevelItem => result.Append(stockLevelItem.Product.SMSReferenceCode + "0"));

            return result.ToString();
        }

        private List<OutpostStockLevel> GetOutpostStockLevelsByOutpostAndProductGroup(Guid outpostId, Guid productGroupId)
        {
            return queryServiceStockLevel.Query().Where(s => s.Outpost.Id == outpostId && s.ProductGroup.Id == productGroupId).ToList();
        }

        private void UpdateSingleOutpostStockLevelWithValuesReceivedBySms(OutpostStockLevel outpostStockLevel, SmsReceived smsReceived)
        {
            outpostStockLevelService.SaveHistoricalOutpostStockLevelToPreviousOutpostStockLevelOfCurrent(outpostStockLevel);

            foreach (ReceivedStockLevel receivedStockLevel in smsReceived.ReceivedStockLevels)
            {
                if (outpostStockLevel.Product.SMSReferenceCode.Equals(receivedStockLevel.ProductSmsReference))
                {
                    outpostStockLevel.PrevStockLevel = outpostStockLevel.StockLevel;
                    outpostStockLevel.StockLevel = receivedStockLevel.StockLevel;
                    outpostStockLevel.UpdateMethod = SMS_UPDATED_METHOD;
                    saveCommandOutpostStockLevel.Execute(outpostStockLevel);
                }
            }
        }
    }
}