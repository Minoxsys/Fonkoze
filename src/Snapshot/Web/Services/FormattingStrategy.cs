using System;
using System.Linq;
using System.Text;

namespace Web.Services
{
    public class FormattingStrategy
    {

        private const string SMS_MESSAGE_TEMPLATE = "Please provide current stock level for product group {0} using format\n SC {1}";

        public string FormatEmail(string productGroupName, string link)
        {
            string messageBody = "Please update the stock information for product group <b>" + productGroupName + "</b> at this " + link;
            return messageBody;
        }

        public string FormatSms(ProductLevelRequestMessageInput input)
        {
            StringBuilder productReferenceCodes = new StringBuilder();
            input.Products.ForEach(p => productReferenceCodes.Append(p.SMSReferenceCode + "0 "));
            string smsMessage = string.Format(SMS_MESSAGE_TEMPLATE, input.ProductGroup.Name, productReferenceCodes);
            return smsMessage;
        }
    }
}