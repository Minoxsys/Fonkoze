using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace Web.Services
{
    public class HttpService : IHttpService
    {
        WebRequest webRequest;

        public void SetWebRequest(WebRequest webRequest)
        {
            this.webRequest = webRequest;
        }

        private void PrepareWebRequestForPost(string url, string data)
        {
            webRequest = (HttpWebRequest)WebRequest.Create(url);

            webRequest.Method = "POST";
            webRequest.ContentLength = Encoding.UTF8.GetByteCount(data);
            webRequest.ContentType = "application/x-www-form-urlencoded";
        }

        private void SendRequest(string data)
        {
            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(webRequest.GetRequestStream());
                myWriter.Write(data);
                myWriter.Flush();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                myWriter.Close();
            }
        }

        private string GetResponse()
        {
            String result = "";

            HttpWebResponse objResponse = (HttpWebResponse)webRequest.GetResponse();
            using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
            {
                result = sr.ReadToEnd();
                // Close and clean up the StreamReader
                sr.Close();
            }

            return result;
        }

        public string Post(string url, string data)
        {
            PrepareWebRequestForPost(url, data);
            
            SendRequest(data);

            return GetResponse();
        }
    }
}