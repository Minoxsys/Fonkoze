using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using Core.Persistence;
using Persistence.Queries.Products;
using System.Text;
using Web.Areas.CampaignManagement.Models.Campaign;
using Web.Areas.CampaignManagement.Models.ProductLevelRequest;

namespace Web.Services
{
    public class SmsRequestService : ISmsRequestService, IProductLevelRequestMessageSenderService
    {
        public const string MESSAGE_NOT_DELIVERED = "Message not delivered";
        public const string SMS_UPDATED_METHOD = "SMS";

        private IQueryService<Outpost> queryServiceOutpost;
        private IQueryService<ProductGroup> queryServiceProductGroup;
        private IQueryService<OutpostStockLevel> queryServiceStockLevel;
        private IQueryService<SmsRequest> queryServiceSmsRequest;
        private IOutpostStockLevelService outpostStockLevelService;

        private ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest;
        private ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel;

        public ISmsGatewayService smsGatewayService;
        private FormattingStrategy formattingStrategy;

        public SmsRequestService(IQueryService<Outpost> queryServiceOutpost, IQueryService<ProductGroup> queryServiceProductGroup,
            IQueryService<OutpostStockLevel> queryServiceStockLevel, IQueryService<SmsRequest> queryServiceSmsRequest, 
            ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest, IOutpostStockLevelService outpostStockLevelService, 
            ISaveOrUpdateCommand<OutpostStockLevel> saveCommandOutpostStockLevel, ISmsGatewayService smsGatewayService,
            FormattingStrategy formattingStrategy)
        {
            this.queryServiceOutpost = queryServiceOutpost;
            this.queryServiceProductGroup = queryServiceProductGroup;
            this.queryServiceStockLevel = queryServiceStockLevel;
            this.saveCommandSmsRequest = saveCommandSmsRequest;
            this.queryServiceSmsRequest = queryServiceSmsRequest;
            this.outpostStockLevelService = outpostStockLevelService;
            this.saveCommandOutpostStockLevel = saveCommandOutpostStockLevel;
            this.smsGatewayService = smsGatewayService;
            this.formattingStrategy = formattingStrategy;
        }


        public void UpdateOutpostStockLevelsWithValuesReceivedBySms(SmsReceived smsReceived)
        {
            var stockLevels = GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(smsReceived.ProductGroupReferenceCode, smsReceived.Number);

            foreach (OutpostStockLevel outpostStockLevel in stockLevels)
            {
                UpdateSingleOutpostStockLevelWithValuesReceivedBySms(outpostStockLevel, smsReceived);
            }
        }

        public bool SendProductLevelRequestMessage(ProductLevelRequestMessageInput input)
        {
            if ((input.Products.Count > 0) && input.Contact != null && input.Contact.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE))
            {
                SmsRequest smsRequest = new SmsRequest();

                smsRequest.Client = input.Client;
                smsRequest.Number = input.Contact.ContactDetail;
                smsRequest.OutpostId = input.Outpost.Id;
                smsRequest.ProductGroupId = input.ProductGroup.Id;
                smsRequest.ProductGroupReferenceCode = input.ProductGroup.ReferenceCode;

                smsRequest.Message = MESSAGE_NOT_DELIVERED;
                saveCommandSmsRequest.Execute(smsRequest);

                smsRequest.Message = formattingStrategy.FormatSms(input);
                saveCommandSmsRequest.Execute(smsRequest);

                try
                {
                    smsGatewayService.SendSmsRequest(smsRequest);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }


        private List<OutpostStockLevel> GetOutpostStockLevelsByProductGroupReferenceAndPhoneNumber(string productGroupReferenceCode, string phoneNumber)
        {
            List<OutpostStockLevel> stockLevels = new List<OutpostStockLevel>();
            SmsRequest smsRequest = queryServiceSmsRequest.Query().Where(r => r.ProductGroupReferenceCode == productGroupReferenceCode && r.Number.Contains(phoneNumber)).OrderByDescending(r => r.Created).FirstOrDefault();

            if (smsRequest != null)
            {
                stockLevels.AddRange(GetOutpostStockLevelsByOutpostAndProductGroup(smsRequest.OutpostId, smsRequest.ProductGroupId));
            }

            return stockLevels;
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