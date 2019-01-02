using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace HexiUtils
{
    public class RequestHelper
    {
        public static StatusReport getRequest(string url)
        {
            WebRequest request = null;
            WebResponse response = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            StatusReport sr = new StatusReport();
            sr.status = "success";
            sr.result = "成功";
            try
            {
                request = WebRequest.Create(url);
                request.Method = "GET";
                response = request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream);
                sr.data = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                sr.status = "fail";
                sr.result = "发生错误：" + ex.Message;
            }
            finally
            {
                if (!(streamReader == null)) streamReader.Close();
                if (!(responseStream == null)) responseStream.Close();
                if (!(response == null)) response.Close();
            }

            return sr;
        }

        public static StatusReport PostRequestTest(string url, NameValueCollection data)
        {
            StatusReport report = new StatusReport();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.BaseAddress = new Uri(url);

            HttpRequestMessage requestMessage = new HttpRequestMessage();
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            requestMessage.Headers.Clear();
            requestMessage.Headers.Add("ContentType", "application/x-www-form-urlencoded;charset=uft-8");
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Content = new FormUrlEncodedContent(ToDictionary(data));
            responseMessage = client.SendAsync(requestMessage).Result;
            report.data = responseMessage.Content.ReadAsStringAsync().Result ;
            return report;
        }

        public static StatusReport PostRequest(string url, NameValueCollection data)
        {
            WebRequest request = null;
            WebResponse response = null;
            Stream requestStream = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            StatusReport sr = new StatusReport();
            sr.status = "success";
            sr.result = "成功";
            string result = "";
            string param = "";
            //param = JsonConvert.SerializeObject(ToDictionary(data));
            foreach (string key in data)
            {
                param += String.Format("{0}={1}&", key, data[key]);

                //param += String.Format("\"{0}\"=\"{1}\",", key, data[key]);
            }
            if (!(String.IsNullOrEmpty(param)))
            {
                param = param.TrimEnd('&');
                //param = param.TrimEnd(',');
                //param = "{" + param + "}";
                sr.parameters = param;
            }
            try
            {
                byte[] byteParam = System.Text.Encoding.UTF8.GetBytes(param);
                request = WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Clear();
                request.ContentType = "application/x=www-form-urlencoded;charset=utf8";
                request.ContentLength = byteParam.Length;

                requestStream = request.GetRequestStream();
                requestStream.Write(byteParam, 0, byteParam.Length);
                response = request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream);
                result = streamReader.ReadToEnd();
                sr.data = result;
            }
            catch (Exception ex)
            {
                sr.status = "fail";
                sr.result = "发生错误：" + ex.Message;
            }
            finally
            {
                if (!(streamReader == null)) streamReader.Close();
                if (!(responseStream == null)) responseStream.Close();
                if (!(response == null)) response.Close();
                if (!(requestStream == null)) requestStream.Close();
            }
            return sr;
        }

        public static StatusReport PostRequest(string url, string data)
        {
            WebRequest request = null;
            WebResponse response = null;
            Stream requestStream = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            StatusReport sr = new StatusReport();
            sr.status = "success";
            sr.result = "成功";
            string result = "";
            try
            {
                byte[] byteParam = System.Text.Encoding.UTF8.GetBytes(data);
                request = WebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Clear();
                request.ContentType = "application/x=www-form-urlencoded;charset=utf8";
                request.ContentLength = byteParam.Length;

                requestStream = request.GetRequestStream();
                requestStream.Write(byteParam, 0, byteParam.Length);
                response = request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream);
                result = streamReader.ReadToEnd();
                sr.data = result;
            }
            catch (Exception ex)
            {
                sr.status = "fail";
                sr.result = "发生错误：" + ex.Message;
            }
            finally
            {
                if (!(streamReader == null)) streamReader.Close();
                if (!(responseStream == null)) responseStream.Close();
                if (!(response == null)) response.Close();
                if (!(requestStream == null)) requestStream.Close();
            }
            return sr;
        }

        private static Dictionary<string,string> ToDictionary (NameValueCollection nvc)
        {
            string[] keys = nvc.AllKeys;
            Dictionary<string, string> resDic = new Dictionary<string, string>();
            foreach(string key in keys)
            {
                resDic.Add(key, nvc[key]);
            }
            return resDic;
        }
    }
}