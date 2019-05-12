using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using HexiUtils;
using System.Net;
using System.Collections.Specialized;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult GetFiles(string tempFileName, string docTableName)
        {
            string[] temp = tempFileName.Split('-');
            string[] docTemp = temp[temp.Length - 1].Split('|');
            string recordId = docTemp[docTemp.Length - 2];//记录ID
            string fileName = docTemp[docTemp.Length - 1];//文件名
            string fileType = fileName.Split('.')[fileName.Split('.').Length - 1];//文件类型
            string fileFullName = "D:\\Files" + "\\" + docTableName + "\\" + recordId + "\\" + fileName;
            FileStream fStream = null;
            try
            {
                fStream = new FileStream(fileFullName, FileMode.Open);
                FileStreamResult fsr = File(fStream, "application/" + fileType);
                return fsr;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (fStream != null)
                {
                    fStream.Close();
                }
            }

        }



        [HttpPost]
        public ActionResult OnSetRepairImages()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            try
            {
                //string mainPath7 = "\\\\PS-03\\Files\\jczl_fwrwgl\\";
                //string mainPath8 = "\\\\PS-03\\Files2\\jczl_fwrwgl\\";
                string mainPath7 = "\\\\PS-03\\Files\\jczl_fwrwgl\\";
                string mainPath8 = "\\\\PS-03\\Files2\\jczl_fwrwgl\\";
                string sqlImagePath = Request.Files.AllKeys[0];
                string imagePath7 = mainPath7 + sqlImagePath;
                string imagePath8 = mainPath8 + sqlImagePath;
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath7);
                uploadImage.SaveAs(imagePath8);
                string ID = Request.Form["id"];
                string func = Request.Form["func"];
                string index = Request.Form["index"];
                sr = RepairDal.SetRepairImage(ID, func, index, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }


        //[HttpPost]
        //public string OnSetImage()
        //{
        //    string url = Request["serverUrl"];
        //    if (string.IsNullOrEmpty(url))
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "目标url未指定" + "\"}";
        //    }
        //    if (Request.Files.Count == 0)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "没有接收到图片信息" + "\"}";
        //    }
        //    try
        //    {
        //        string mainPath = "C:\\inetpub\\wxInnerCloudServer\\wximages\\";
        //        string imagePath = mainPath + Request.Files.AllKeys[0];
        //        HttpPostedFileBase uploadImage = (Request.Files[0]);
        //        uploadImage.SaveAs(imagePath);
        //    }
        //    catch (NotImplementedException exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }

        //    HttpWebRequest request = null;
        //    Stream requestStream = null;
        //    WebResponse response = null;
        //    Stream responseStream = null;
        //    StreamReader streamReader = null;
        //    try
        //    {
        //        byte[] byteParam = new byte[Request.InputStream.Length];
        //        Request.InputStream.Read(byteParam, 0, byteParam.Length);
        //        request = HttpWebRequest.CreateHttp(url);
        //        NameValueCollection headers = Request.Headers;
        //        request.Method = "POST";
        //        request.ContentType = Request.ContentType;
        //        request.ContentLength = byteParam.Length;
        //        requestStream = request.GetRequestStream();
        //        requestStream.Write(byteParam, 0, byteParam.Length);

        //        response = request.GetResponse();

        //        responseStream = response.GetResponseStream();

        //        streamReader = new StreamReader(responseStream);
        //        string responseString = streamReader.ReadToEnd();
        //        return "hello" + responseString;
        //    }
        //    catch (Exception exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }
        //    finally
        //    {
        //        if (!(streamReader == null))
        //        {
        //            streamReader.Close();
        //        }
        //        if (!(requestStream == null))
        //        {
        //            requestStream.Close();
        //        }
        //        if (!(responseStream == null))
        //        {
        //            responseStream.Close();
        //        }
        //    }
        //}































        //[HttpPost]
        //public string OnSetImage1()
        //{
        //    string url = Request["serverUrl"];
        //    if (string.IsNullOrEmpty(url))
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "目标url未指定" + "\"}";
        //    }
        //    if (Request.Files.Count == 0)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "没有接收到图片信息" + "\"}";
        //    }
        //    try
        //    {
        //        string mainPath = "C:\\inetpub\\wxInnerCloudServer\\wximages\\";
        //        string imagePath = mainPath + Request.Files.AllKeys[0];
        //        HttpPostedFileBase uploadImage = (Request.Files[0]);
        //        uploadImage.SaveAs(imagePath);
        //    }
        //    catch (NotImplementedException exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }

        //    HttpWebRequest request = null;
        //    Stream requestStream = null;
        //    WebResponse response = null;
        //    Stream responseStream = null;
        //    StreamReader streamReader = null;
        //    try
        //    {
        //        byte[] byteParam = new byte[Request.InputStream.Length];
        //        Request.InputStream.Read(byteParam, 0, byteParam.Length);
        //        request = HttpWebRequest.CreateHttp(url);
        //        NameValueCollection headers = Request.Headers;
        //        request.Method = "POST";
        //        request.ContentType = Request.ContentType;
        //        request.ContentLength = byteParam.Length;
        //        requestStream = request.GetRequestStream();
        //        requestStream.Write(byteParam, 0, byteParam.Length);

        //        response = request.GetResponse();

        //        responseStream = response.GetResponseStream();

        //        streamReader = new StreamReader(responseStream);
        //        string responseString = streamReader.ReadToEnd();
        //        return "hello" + responseString;
        //    }
        //    catch (Exception exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }
        //    finally
        //    {
        //        if (!(streamReader == null))
        //        {
        //            streamReader.Close();
        //        }
        //        if (!(requestStream == null))
        //        {
        //            requestStream.Close();
        //        }
        //        if (!(responseStream == null))
        //        {
        //            responseStream.Close();
        //        }
        //    }
        //}



        //public string OnGetImage()
        //{
        //    string url = Request["serverUrl"];
        //    if (string.IsNullOrEmpty(url))
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "目标url未指定" + "\"}";
        //    }
        //    if (Request.Files.Count == 0)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + "没有接收到图片信息" + "\"}";
        //    }
        //    try
        //    {
        //        string mainPath = "C:\\inetpub\\wxInnerCloudServer\\wximages\\";
        //        string imagePath = mainPath + Request.Files.AllKeys[0];
        //        HttpPostedFileBase uploadImage = (Request.Files[0]);
        //        uploadImage.SaveAs(imagePath);
        //    }
        //    catch (NotImplementedException exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }

        //    HttpWebRequest request = null;
        //    Stream requestStream = null;
        //    WebResponse response = null;
        //    Stream responseStream = null;
        //    StreamReader streamReader = null;
        //    try
        //    {
        //        byte[] byteParam = new byte[Request.InputStream.Length];
        //        Request.InputStream.Read(byteParam, 0, byteParam.Length);
        //        request = HttpWebRequest.CreateHttp(url);
        //        NameValueCollection headers = Request.Headers;
        //        request.Method = "POST";
        //        request.ContentType = Request.ContentType;
        //        request.ContentLength = byteParam.Length;
        //        requestStream = request.GetRequestStream();
        //        requestStream.Write(byteParam, 0, byteParam.Length);

        //        response = request.GetResponse();

        //        responseStream = response.GetResponseStream();

        //        streamReader = new StreamReader(responseStream);
        //        string responseString = streamReader.ReadToEnd();
        //        return "hello" + responseString;
        //    }
        //    catch (Exception exp)
        //    {
        //        return "{\"status\": \"Fail\", \"result\": \"云服务器发生错误：" + exp.Message + "\"}";
        //    }
        //    finally
        //    {
        //        if (!(streamReader == null))
        //        {
        //            streamReader.Close();
        //        }
        //        if (!(requestStream == null))
        //        {
        //            requestStream.Close();
        //        }
        //        if (!(responseStream == null))
        //        {
        //            responseStream.Close();
        //        }
        //    }
        //}
    }
}