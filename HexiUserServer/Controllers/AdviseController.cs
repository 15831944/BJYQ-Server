using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class AdviseController : Controller
    {
        [HttpPost]
        public ActionResult OnSetAdvise(string ztName, string name, string phone, string roomNumber, string type, string content, string submitDate)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(ztName) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(roomNumber) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(submitDate))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = AdviseDal.SetAdvise(ztName, name, phone, roomNumber, type, content, submitDate);
            return Json(sr);
        }


        public ActionResult OnGetAdvise(string ztName, string phone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(ztName) || string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = AdviseDal.GetAdvise(ztName, phone);
            return Json(sr);
        }

        public ActionResult OnSetAdviseImage()
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
                string mainPath = "D:\\wximages\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string ID = Request.Form["id"];
                string func = Request.Form["func"];
                string index = Request.Form["index"];
                sr = AdviseDal.SetAdviseImage(ID, func, index, sqlImagePath);
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