using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Areas.CampaignManagement.Models.Campaign;
using System.Text;

namespace Web.Helpers
{
    public class ConvertHelper
    {
        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static string ConvertToJSON(object model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(model);
        }

        public static string EncodeTo64(string toEncode)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(toEncode)); ;
        }
    }
}