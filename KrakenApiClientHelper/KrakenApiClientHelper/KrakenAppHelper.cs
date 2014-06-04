using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifyHelper.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ShopifyHelper
{
    public class KrakenAppHelper
    {
        private string apiKey;
        private string apiSecreet;
        private string apiEndpointURL;

        public KrakenAppHelper(string apiKey, string apiScreet)
        {
            this.apiKey = apiKey;
            this.apiSecreet = apiScreet;

            this.apiEndpointURL = "https://api.kraken.io/v1";
        }

        public KrakenResponse UploadFile(FileType fileType, string pathOrURL, bool isLossy)
        {
            KrakenResponse response = null;

            try
            {
                JObject obj;

                if (fileType == FileType.URL)
                {
                    string jsonObj = string.Format(@"{{""auth"": {{ ""api_key"": ""{0}"", ""api_secret"": ""{1}"" }}, ""wait"": true, ""lossy"": {2}, ""url"": ""{3}"" }}",
                    this.apiKey, this.apiSecreet, isLossy.ToString().ToLowerInvariant(), pathOrURL);

                    object jsonResponse = this.Post("/url", jsonObj);
                    obj = JObject.Parse(jsonResponse.ToString());
                }
                else
                {
                    string jsonObj = string.Format(@"{{""auth"": {{ ""api_key"": ""{0}"", ""api_secret"": ""{1}"" }}, ""wait"": true, ""lossy"": {2} }}",
                    this.apiKey, this.apiSecreet, isLossy.ToString().ToLowerInvariant());

                    object jsonResponse = this.Post("/upload", jsonObj, pathOrURL);
                    obj = JObject.Parse(jsonResponse.ToString());
                }

                bool status = (bool)obj["success"];
                if (!status)
                {
                    string message = obj["message"].ToString();
                    throw new Exception(message);
                }
                response = JsonConvert.DeserializeObject<KrakenResponse>(obj.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;

        }

        public KrakenResponse UploadFile(FileType fileType, string pathOrURL, bool isLossy, int imageWidth, int imageHeight, ImageResizingType resizingType)
        {

            KrakenResponse response = null;

            try
            {
                JObject obj;

                if (fileType == FileType.URL)
                {
                    string jsonObj = string.Format(@"{{""auth"": {{ ""api_key"": ""{0}"", ""api_secret"": ""{1}"" }}, ""wait"": true, ""lossy"": {2}, ""resize"": {{ ""width"": {3}, ""height"": {4}, ""strategy"": ""{5}"" }}, ""url"": ""{6}""  }}",
                        this.apiKey, this.apiSecreet, isLossy.ToString().ToLowerInvariant(), imageWidth, imageHeight, resizingType.ToString(), pathOrURL);

                    object jsonResponse = this.Post("/url", jsonObj);
                    obj = JObject.Parse(jsonResponse.ToString());
                }
                else
                {
                    string jsonObj = string.Format(@"{{""auth"": {{ ""api_key"": ""{0}"", ""api_secret"": ""{1}"" }}, ""wait"": true, ""lossy"": {2}, ""resize"": {{ ""width"": {3}, ""height"": {4}, ""strategy"": ""{5}"" }} }}",
                        this.apiKey, this.apiSecreet, isLossy.ToString().ToLowerInvariant(), imageWidth, imageHeight, resizingType.ToString());

                    object jsonResponse = this.Post("/upload", jsonObj, pathOrURL);
                    obj = JObject.Parse(jsonResponse.ToString());
                }

                bool status = (bool)obj["success"];
                if (!status)
                {
                    string message = obj["message"].ToString();
                    throw new Exception(message);
                }

                response = JsonConvert.DeserializeObject<KrakenResponse>(obj.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;

        }

        /// <summary>
        /// Make an HTTP Request to the Shopify API
        /// </summary>
        /// <param name="method">method to be used in the request</param>
        /// <param name="path">the path that should be requested</param>
        /// <param name="jsonStringData">any parameters needed or expected by the API</param>
        /// <seealso cref="http://api.shopify.com/"/>
        /// <returns>the server response</returns>
        private object Post(string endPoint, string jsonStringData)
        {
            string url = this.apiEndpointURL + endPoint;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";

            // authentication
            //var cache = new CredentialCache();
            //cache.Add(new Uri(url), "Basic", new NetworkCredential(this.apiKey, this.apiSecreet));
            //request.Credentials = cache;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //add the requst body to the request stream
            if (!String.IsNullOrEmpty(jsonStringData))
            {
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(jsonStringData);
                    writer.Close();
                }
            }

            string result = null;
            var response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }

        public string Post(string endpoint, string jsonStringData, string filePath)
        {
            string boundary = DateTime.Now.Ticks.ToString("x");

            string url = this.apiEndpointURL + endpoint;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Accept = "application/json";
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.KeepAlive = true;

            boundary = "--" + boundary;

            // authentication
            //var cache = new CredentialCache();
            //cache.Add(new Uri(url), "Basic", new NetworkCredential(this.apiKey, this.apiSecreet));
            //httpWebRequest.Credentials = cache;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            using (Stream memStream = new System.IO.MemoryStream())
            {
                string formdataTemplate = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                string formdata = string.Format(formdataTemplate, "data", jsonStringData);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formdata);
                memStream.Write(formitembytes, 0, formitembytes.Length);

                string fileExt = Path.GetExtension(filePath).ToLowerInvariant().Trim('.');
                string headerTemplate = "\r\n" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "upload", Path.GetFileName(filePath), "image/" + (fileExt.Equals("jpg") ? "jpeg" : fileExt));
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[fileStream.Length];
                    fileStream.Read(bytes, 0, (int)fileStream.Length);
                    memStream.Write(bytes, 0, (int)fileStream.Length);
                    fileStream.Close();

                    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n" + boundary + "--\r\n");
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                }

                httpWebRequest.ContentLength = memStream.Length;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                }
            }

            string result = null;
            var response = (HttpWebResponse)httpWebRequest.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;

        }

        /// <summary>
        /// The enumeration of HTTP Methods used by the API
        /// </summary>
        private enum HttpMethods
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public enum ImageResizingType
        {
            exact,
            portrait,
            landscape,
            auto,
            crop,
            //square,
            //fill
        }

        public enum FileType
        {
            URL,
            LocalFile
        }
    }
}
