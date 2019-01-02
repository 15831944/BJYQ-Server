using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Net.Http;

namespace HexiInnerCloudServer.Controllers
{
    public class SetImageController : Controller
    {
        [HttpPost]
        public string OnSetImage()
        {
            string url = Request["serverUrl"];
            if (string.IsNullOrEmpty(url))
            {
                return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "目标url未指定" + "\"}";
            }
            if (Request.Files.Count == 0)
            {
                return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "没有接收到图片信息" + "\"}";
            }
            try
            {
                string mainPath = "C:\\inetpub\\wxInnerCloudServer\\wximages\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
            }
            catch (NotImplementedException exp)
            {
                return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
            }

            HttpWebRequest request = null;
            Stream requestStream = null;
            WebResponse response = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            try
            {
                byte[] byteParam = new byte[Request.InputStream.Length];
                Request.InputStream.Read(byteParam, 0, byteParam.Length);
                request = HttpWebRequest.CreateHttp(url);
                NameValueCollection headers = Request.Headers;
                request.Method = "POST";
                request.ContentType = Request.ContentType;
                request.ContentLength = byteParam.Length;
                requestStream = request.GetRequestStream();
                requestStream.Write(byteParam, 0, byteParam.Length);

                response = request.GetResponse();

                responseStream = response.GetResponseStream();

                streamReader = new StreamReader(responseStream);
                string responseString = streamReader.ReadToEnd();
                return "hello" + responseString;
            }
            catch (Exception exp)
            {
                return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
            }
            finally
            {
                if (!(streamReader == null))
                {
                    streamReader.Close();
                }
                if (!(requestStream == null))
                {
                    requestStream.Close();
                }
                if (!(responseStream == null))
                {
                    responseStream.Close();
                }
            }
        }
    }
}