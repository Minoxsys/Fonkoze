using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Web.Bootstrap
{
	public class AppSettings
	{
		public static TimeSpan DefaultCacheAbsoluteTimeExpiration = TimeSpan.Parse(ConfigurationManager.AppSettings["DefaultCacheAbsoluteTimeExpiration"]);
		public static TimeSpan StaticFileHttpMaxAge = TimeSpan.Parse(ConfigurationManager.AppSettings["StaticFileHttpMaxAge"]);

		public static string ScriptsVersion = ConfigurationManager.AppSettings["ScriptsVersion"];

        public static string SendMailFrom = ConfigurationManager.AppSettings["SendMail.From"];

        public static string EmailResponseUrl = ConfigurationManager.AppSettings["EmailResponse.Url"];

        /// <summary>
        /// The URL of the SMS API used to post the SMS data.
        /// </summary>
        public static string SmsGatewayUrl = ConfigurationManager.AppSettings["SmsGateway.Url"];

        /// <summary>
        /// The SMS API account username to be used.
        /// </summary>
        public static string SmsGatewayUserName = ConfigurationManager.AppSettings["SmsGateway.UserName"];

        /// <summary>
        /// The SMS API account password to be used.
        /// </summary>
        public static string SmsGatewayPassword = ConfigurationManager.AppSettings["SmsGateway.Password"];

        /// <summary>
        /// The SMS from text. Use "xreplyx" to be able to receive replys.
        /// </summary>
        public static string SmsGatewayFrom = ConfigurationManager.AppSettings["SmsGateway.From"];

        /// <summary>
        /// Use to set-up the SmsGatewayService in test mode, if you don't want to send the SMS messages.
        /// Values to be used are 0 for real mode and 1 for test mode.
        /// </summary>
        public static string SmsGatewayTestMode = ConfigurationManager.AppSettings["SmsGateway.TestMode"];

        /// <summary>
        /// Used to configure the SMS API to send detailed information about the sent SMS and account.
        /// </summary>
        public static string SmsGatewayDebugMode = ConfigurationManager.AppSettings["SmsGateway.DebugMode"];

	}
}