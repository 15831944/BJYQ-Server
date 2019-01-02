using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class ComplaintController : Controller
    {
        // GET: Complaint
        [HttpPost]
        public ActionResult OnSetComplaint(string receptionDate, string name, string address, string content, string classify, string phone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(receptionDate) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                sr.parameters = Request.QueryString;
            }
            else
            {
                sr = ComplaintDal.SetComplaint(receptionDate, name, address, content, classify, phone);
            }
            return Json(sr);
        }

        [HttpPost]
        public ActionResult OnGetComplaintList(string classify, string name, string phone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                sr.parameters = Request.QueryString;
            }
            else
            {
                sr = ComplaintDal.GetComplaintList(classify,name,phone);
                sr.parameters = Request.QueryString;
            }
            return Json(sr);
        }

        public ActionResult OnSetComplaintImage()
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
                string mainPath = "F:\\wytws\\Files\\jczl_fwrwgl\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string ID = Request.Form["id"];
                string func = Request.Form["func"];
                string index = Request.Form["index"];
                sr = ComplaintDal.SetComplainImage(ID, func, index, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }


        public ActionResult OnEvaluation(string evaluation, string isSatisfying, string isFinish, string id)
        {
            StatusReport sr = new StatusReport();
            sr = ComplaintDal.Evaluation(evaluation, isSatisfying, isFinish, id);
            return Json(sr);
        }
    }
}