using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using HexiServer.Models;
using HexiUtils;

namespace HexiServer.Common
{
    public class RequestHelper1
    {
        public static StatusReport getRequest1(string url)
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

        public static StatusReport PostRequest1(string url, NameValueCollection data)
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
            foreach (string key in data)
            {
                param += String.Format("\"{0}\":\"{1}\",", key, data[key]);
            }
            if (!(String.IsNullOrEmpty(param)))
            {
                param = param.TrimEnd(',');
                param = "{" + param + "}";
                sr.parameters = param;
            }
            try
            {
                byte[] byteParam = System.Text.Encoding.UTF8.GetBytes(param);
                request = WebRequest.Create(url);
                request.Method = "POST";
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
    }
}