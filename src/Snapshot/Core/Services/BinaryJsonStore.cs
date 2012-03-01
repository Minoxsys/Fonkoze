using System;
using System.Linq;

namespace Core.Services
{
    public class BinaryJsonStore<T>
    {
        private string ConvertToJSON(T model)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(model);
        }

        private T ConvertFromJson(string json)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        private string ByteArrayToStr(byte[] bites)
        {
            return System.Text.Encoding.UTF8.GetString(bites);
        }

        private byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static T From(byte[] value)
        {
            var store = new BinaryJsonStore<T>();

            return store.ConvertFromJson(store.ByteArrayToStr(value));
        }

        public static byte[] From(T model)
        {
            var store = new BinaryJsonStore<T>();

            return store.StrToByteArray(store.ConvertToJSON(model));
        }
    }
}