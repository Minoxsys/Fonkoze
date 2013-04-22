using Core.Persistence;
using Domain;
using System;

namespace Web.Services
{
    public class SmsRequestService : IProductLevelRequestMessageSenderService
    {
        public const string MessageNotDelivered = "Message not delivered";

        private readonly ISaveOrUpdateCommand<SmsRequest> _saveCommandSmsRequest;
        private readonly ISendSmsService _smsGatewayService;
        private readonly FormattingStrategy _formattingStrategy;

        public SmsRequestService(ISaveOrUpdateCommand<SmsRequest> saveCommandSmsRequest, ISendSmsService smsGatewayService,
                                 FormattingStrategy formattingStrategy)
        {
            _saveCommandSmsRequest = saveCommandSmsRequest;
            _smsGatewayService = smsGatewayService;
            _formattingStrategy = formattingStrategy;
        }

        public bool SendProductLevelRequestMessage(ProductLevelRequestMessageInput input)
        {
            if ((input.Products.Count > 0) && input.Contact != null && input.Contact.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE))
            {
                var smsRequest = new SmsRequest
                    {
                        Client = input.Client,
                        Number = input.Contact.ContactDetail,
                        OutpostId = input.Outpost.Id,
                        ProductGroupId = input.ProductGroup.Id,
                        ProductGroupReferenceCode = input.ProductGroup.ReferenceCode,
                        Message = MessageNotDelivered
                    };

                _saveCommandSmsRequest.Execute(smsRequest);

                smsRequest.Message = _formattingStrategy.FormatSms(input);
                _saveCommandSmsRequest.Execute(smsRequest);

                try
                {
                    _smsGatewayService.SendSmsRequest(smsRequest, false);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}