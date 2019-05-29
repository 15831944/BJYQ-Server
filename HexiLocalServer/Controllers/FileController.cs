using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;

namespace HexiLocalServer.Controllers
{
    public class FileController : Controller
    {
        // GET: File
        public ActionResult OnSetImage()
        {
            StatusReport sr = new StatusReport();
            string ID = Request.Form["id"];
            string func = Request.Form["func"];
            string index = Request.Form["index"];
            string path = Request.Form["path"];
            //string mainPath = "D:\\Servers\\bjyqServer\\wgxt\\WYTWS\\Files\\";
            string mainPath = "D:\\wgxt\\wgxt\\WYTWS\\Files\\";
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
                ////sr = SetLocalImage(Request);
                sr.status = "Success";
                sr.result = "照片保存成功";
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }
    }
}