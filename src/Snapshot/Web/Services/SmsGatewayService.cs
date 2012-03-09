using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using Persistence.Queries.Outposts;
using Core.Persistence;

namespace Web.Services
{
    public class SmsGatewayService : ISmsGatewayService
    {
        private const string CONTENT_FRAGMENTATION_REGEX = @"^(?<ProductGroupReferenceCode>([A-Z]|[a-z])+)\s+(?<ProductStockLevels>((([A-Z]|[a-z])+(\d)+)\s*)+)";
        private const string PRODUCT_STOCK_LEVEL_REGEX = @"(?<ProductStockLevel>((?<ProductReferenceCode>([A-Z|a-z]+))(?<StockLevel>\d+))\s*)";
        private const string PRODUCT_GROUP_REFERENCE_CODE = "ProductGroupReferenceCode";
        private const string PRODUCT_STOCK_LEVELS = "ProductStockLevels";
        private const string PRODUCT_REFERENCE_CODE = "ProductReferenceCode";
        private const string STOCK_LEVEL = "StockLevel";

        private IHttpService httpService;
        private ISmsGatewaySettingsService smsGatewaySettingsService;
        private IQueryOutposts queryOutposts;

        private IQueryService<Contact> queryServiceContact;

        public SmsGatewayService(ISmsGatewaySettingsService smsGatewaySettingsService, IHttpService httpService, IQueryOutposts queryOutposts, IQueryService<Contact> queryServiceContact)
        {
            this.httpService = httpService;
            this.smsGatewaySettingsService = smsGatewaySettingsService;
            this.queryOutposts = queryOutposts;
            this.queryServiceContact = queryServiceContact;
        }

        public string SendSmsRequest(SmsRequest smsRequest)
        {
            string postData = GetPostDataFromSettingsAndSmsRequest(smsRequest);
            string postResponse = httpService.Post(smsGatewaySettingsService.SmsGatewayUrl, postData);
            return postResponse;
        }

        public RawSmsReceived AssignOutpostToRawSmsReceivedBySenderNumber(RawSmsReceived rawSmsReceived)
        {
            string number = rawSmsReceived.Sender;
            Contact contact = queryServiceContact.Query().Where(
                c => c.ContactType.Equals(Contact.MOBILE_NUMBER_CONTACT_TYPE) && c.ContactDetail.Contains(number)).FirstOrDefault();
            Outpost outpost = queryOutposts.GetAllContacts().Where(o => o.Contacts.Contains(contact)).FirstOrDefault();
            if (outpost != null)
            {
                rawSmsReceived.OutpostId = outpost.Id;
            }
            return rawSmsReceived;
        }

        public RawSmsReceivedParseResult ParseRawSmsReceived(RawSmsReceived rawSmsReceived)
        {
            ReceivedContentFragments contentFragments = GetContentFragments(rawSmsReceived.Content.Trim().ToUpper());

            RawSmsReceivedParseResult parseResult = new RawSmsReceivedParseResult();
            SmsReceived smsReceived = new SmsReceived();

            smsReceived.Number = rawSmsReceived.Sender;

            if (!string.IsNullOrEmpty(contentFragments.ProductStockLevels))
            {
                smsReceived.RawSmsReceivedId = rawSmsReceived.Id;
                smsReceived.ProductGroupReferenceCode = GetProductGroupReferenceCodeFromSplitContent(contentFragments);
                smsReceived.ReceivedStockLevels = GetReceivedStockLevelsFromContent(contentFragments);

                parseResult.ParseSucceeded = true;
                parseResult.SmsReceived = smsReceived;
            }
            else
            {
                parseResult.ParseSucceeded = false;
                parseResult.ParseErrorMessage = "Sms was not parsed correctly!";
            }
            return parseResult;
        }

        private string GetPostDataForSmsRequestFromSettings()
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("uname=" + smsGatewaySettingsService.SmsGatewayUserName);
            postDataBuilder.Append("&pword=" + smsGatewaySettingsService.SmsGatewayPassword);
            postDataBuilder.Append("&from=" + smsGatewaySettingsService.SmsGatewayFrom);
            postDataBuilder.Append("&test=" + smsGatewaySettingsService.SmsGatewayTestMode);
            postDataBuilder.Append("&info=" + smsGatewaySettingsService.SmsGatewayDebugMode);

            return postDataBuilder.ToString();
        }

        private string GetPostDataFromSmsRequest(SmsRequest smsRequest)
        {
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("selectednums=" + smsRequest.Number);
            postDataBuilder.Append("&custom=" + smsRequest.Id);
            postDataBuilder.Append("&message=" + HttpUtility.HtmlEncode(smsRequest.Message));

            return postDataBuilder.ToString();
        }

        private string GetPostDataFromSettingsAndSmsRequest(SmsRequest smsRequest)
        {
            return GetPostDataForSmsRequestFromSettings() + "&" + GetPostDataFromSmsRequest(smsRequest);
        }

        private ReceivedContentFragments GetContentFragments(string content)
        {
            ReceivedContentFragments splitContent = new ReceivedContentFragments();

            foreach (Match myMatch in Matches(CONTENT_FRAGMENTATION_REGEX, content))
            {
                if (myMatch.Success)
                {
                    splitContent.ProductGroupReferenceCode = myMatch.Groups[PRODUCT_GROUP_REFERENCE_CODE].Value;
                    splitContent.ProductStockLevels = myMatch.Groups[PRODUCT_STOCK_LEVELS].Value;
                }
            }

            return splitContent;
        }

        private string GetProductGroupReferenceCodeFromSplitContent(ReceivedContentFragments fragmentedContent)
        {
            return fragmentedContent.ProductGroupReferenceCode;
        }

        private List<ReceivedStockLevel> GetReceivedStockLevelsFromContent(ReceivedContentFragments fragmentedContent)
        {
            List<ReceivedStockLevel> receivedStockLevels = new List<ReceivedStockLevel>();

            foreach (Match myMatch in Matches(PRODUCT_STOCK_LEVEL_REGEX, fragmentedContent.ProductStockLevels))
            {
                if (myMatch.Success)
                {
                    ReceivedStockLevel receivedStockLevel = new ReceivedStockLevel();
                    receivedStockLevel.ProductSmsReference = myMatch.Groups[PRODUCT_REFERENCE_CODE].Value;
                    receivedStockLevel.StockLevel = int.Parse(myMatch.Groups[STOCK_LEVEL].Value);
                    receivedStockLevels.Add(receivedStockLevel);
                }
            }

            return receivedStockLevels;
        }

        private IEnumerable Matches(string strRegex, string content)
        {
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);

            return myRegex.Matches(content);
        }

        private struct ReceivedContentFragments
        {
            public string ProductGroupReferenceCode;
            public string ProductStockLevels;
        }
    }
}