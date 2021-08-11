using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AutomationFrame_GlobalIntake.Utils
{
    public class clsRest
    {
        private String baseUrl;
        private Dictionary<String, String> headers;

        public clsRest(String url)
        {
            this.baseUrl = url;
            headers = new Dictionary<String, String>();
        }

        public void ClearHeaders()
        {
            headers.Clear();
        }

        public bool AddHeader(String key, String value)
        {
            try { headers.Add(key, value); return true; }
            catch (Exception ex) { Debug.WriteLine(ex.Message); return false; }
        }

        public HTTP_RESPONSE GET(String endpoint)
        {
            return request("GET", endpoint);
        }

        public HTTP_RESPONSE POST(String endpoint, String data)
        {
            return request("POST", endpoint, data);
        }

        private HTTP_RESPONSE request(String requestType, String endpoint, String body = null)
        {
            HttpWebRequest request = WebRequest.CreateHttp(baseUrl + endpoint);
            HTTP_RESPONSE response = new HTTP_RESPONSE();
            //Setup request
            byte[] data = null;
            request.Method = requestType;
            request.Accept = "*/*";
            request.ContentType = "application/json;";
            request.KeepAlive = false;
            //Add Headers
            foreach (KeyValuePair<String, String> kvp in headers) 
            {
                request.Headers.Add(kvp.Key, kvp.Value);
            }
            //Add Body
            if (!String.IsNullOrEmpty(body))
            {
                data = ASCIIEncoding.ASCII.GetBytes(body);
                request.ContentLength = data.Length;

                using (var streamWriter = request.GetRequestStream())
                {
                    streamWriter.Write(data, 0, data.Length);
                    streamWriter.Close();
                }
            }
            //Get Response
            try
            {
                using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse()) 
                {
                    response = getResponseDetails(webResponse);
                }
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    using (HttpWebResponse errResponse = (HttpWebResponse)exception.Response) 
                    {
                        response = getResponseDetails(errResponse);
                    }
                }
                else throw new Exception(exception.Message);
            }

            return response;
        }

        private HTTP_RESPONSE getResponseDetails(HttpWebResponse webResponse)
        {
            HTTP_RESPONSE output = new HTTP_RESPONSE();
            using (var streamWriter = webResponse.GetResponseStream())
            {

                StreamReader reader = new StreamReader(streamWriter);
                string response2 = reader.ReadToEnd();
                output.StatusCode = webResponse.StatusCode;
                output.MessageBody = response2;
            }
            return output;
        }
    }

    public class HTTP_RESPONSE
    {
        public HttpStatusCode StatusCode;
        public Dictionary<String, String> Headers;
        public String MessageBody;
        public TimeSpan Time;

        public HTTP_RESPONSE()
        {
            Headers = new Dictionary<String, String>();
        }

        public void PopulateHeaders(WebHeaderCollection headersIn)
        {
            for (int i = 0; i < headersIn.Count; i++)
            {
                this.Headers.Add(headersIn.Keys[i], headersIn[i]);
            }
        }



    }
}
