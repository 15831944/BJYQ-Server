using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using HexiUtils;
using System.Net;
using System.Collections.Specialized;
using HexiServer.Business;
using HexiServer.Models;
using Newtonsoft.Json.Linq;

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
            string fileFullName = @"D:\wgxt\wgxt\wgxt\WYTWS\Files\" + docTableName + "\\" + recordId + "\\" + fileName;
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

        /// <summary>
        /// 提交工单完成照片
        /// </summary>
        /// <returns></returns>
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
            string ID = Request.Form["id"];
            string func = Request.Form["func"];
            string index = Request.Form["index"];
            string fileName = Request.Files.AllKeys[0];
            sr = RepairDal.SetRepairImage(ID, func, index, fileName);
            sr = SetImage(Request);
            return Json(sr);
        }


        /// <summary>
        /// 提交投诉处理完成照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetcomplainImage()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            string ID = Request.Form["id"];
            string func = Request.Form["func"];
            string index = Request.Form["index"];
            string fileName = Request.Files.AllKeys[0];
            sr = ComplainDal.SetComplainImage(ID, func, index, fileName);
            sr = SetImage(Request);
            return Json(sr);
        }


        /// <summary>
        /// 提交报事照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetPatrolImages()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            string ID = Request.Form["id"];
            string index = Request.Form["index"];
            string fileName = Request.Files.AllKeys[0];
            sr = PatrolDal.SetPatrolImage(ID, index, fileName);
            sr = SetImage(Request);
            return Json(sr);
        }


        /// <summary>
        /// 提交设备保养照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetEquipmentImage()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            string ID = Request.Form["id"];
            string func = Request.Form["func"];
            string fileName = Request.Files.AllKeys[0];
            sr = EquipmentDal.SetEquipmentImage(ID, func, fileName);
            sr = SetImage(Request);
            return Json(sr);
        }


        /// <summary>
        /// 提交设备故障维修照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetEquipmentTroubleImage()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
                string ID = Request.Form["id"];
                string func = Request.Form["func"];
                string index = Request.Form["index"];
                string fileName = "~~-" + (3383 + Convert.ToInt32(index)) + "-" + ID.ToString() + "|" + Request.Files.AllKeys[0];
                sr = EquipmentDal.SetEquipmentTroubleImage(ID, func, index, fileName);
                sr = SetImage(Request);
                return Json(sr);
        }

        /// <summary>
        /// 提交巡检照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetLookOverImage()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            string ID = Request.Form["id"];
            string index = Request.Form["index"];

            string sqlImagePath = "~~-" + (3456 + Convert.ToInt32(index)) + "-" + ID.ToString() + "|" + Request.Files.AllKeys[0];
            sr = LookOverDal.SetLookOverImage(ID, index, sqlImagePath);
            sr = SetImage(Request);
            return Json(sr);
        }


        private StatusReport SetImage (HttpRequestBase Request)
        {
            StatusReport sr = new StatusReport();
            //string ID = Request.Form["id"];
            //string func = Request.Form["func"];
            //string index = Request.Form["index"];
            string path = Request.Form["path"];
            string mainPath = Comman.file_main_path;
            try
            {
                string fileName = Request.Files.AllKeys[0];
                string imagePath = mainPath + path;
                if (!System.IO.Directory.Exists(imagePath))
                {
                    System.IO.Directory.CreateDirectory(imagePath);
                }
                imagePath += "\\" + fileName;
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                sr = SetLocalImage(Request);
                return sr;
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return sr;
            }
        }

        private StatusReport SetLocalImage(HttpRequestBase Request)
        {
            StatusReport sr = new StatusReport();
            HttpWebRequest request = null;
            Stream requestStream = null;
            WebResponse response = null;
            Stream responseStream = null;
            StreamReader streamReader = null;
            try
            {


                string url = "http://192.168.10.7:8080/bjyqlocal/File/OnSetImage";


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
                sr = (StatusReport)JsonConvert.DeserializeObject<StatusReport>(responseString);
                return sr;
            }
            catch (Exception exp)
            {
                return new StatusReport().SetFail("云服务器发生错误：" + exp.Message);
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