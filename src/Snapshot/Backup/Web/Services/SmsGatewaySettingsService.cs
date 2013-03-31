using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Bootstrap;

namespace Web.Services
{
    public class SmsGatewaySettingsService : ISmsGatewaySettingsService
    {
        public virtual string SmsGatewayUrl
        {
            get
            {
                return AppSettings.SmsGatewayUrl;
            }
        }

        public virtual string SmsGatewayUserName
        {
            get
            {
                return AppSettings.SmsGatewayUserName;
            }
        }

        public virtual string SmsGatewayPassword
        {
            get
            {
                return AppSettings.SmsGatewayPassword;
            }
        }

        public virtual string SmsGatewayFrom
        {
            get
            {
                return AppSettings.SmsGatewayFrom;
            }
        }

        public virtual string SmsGatewayTestMode
        {
            get
            {
                return AppSettings.SmsGatewayTestMode;
            }
        }

        public virtual string SmsGatewayDebugMode
        {
            get
            {
                return AppSettings.SmsGatewayDebugMode;
            }
        }
    }
}