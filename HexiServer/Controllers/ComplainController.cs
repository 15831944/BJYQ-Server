using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class ComplainController : Controller
    {
        [HttpPost]
        public ActionResult OnSetComplain(string id, string finishDate, string finishStatus)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(id))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                sr.parameters = Request.QueryString;
            }
            else
            {
                sr = ComplainDal.SetComplaint(id,finishDate,finishStatus);
            }
            return Json(sr);
        }

        [HttpPost]
        public ActionResult OnGetComplainList(string classify, string status)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(status))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                sr.parameters = Request.QueryString;
            }
            else
            {
                sr = ComplainDal.GetComplaintList(classify, status);
                sr.parameters = Request.QueryString;
            }
            return Json(sr);
        }

        public ActionResult OnSetComplainImage()
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
                sr = ComplainDal.SetComplainImage(ID, func, index, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }


        public ActionResult OnEvaluation(string evaluation, string isSatisfying, string id)
        {
            StatusReport sr = new StatusReport();
            sr = ComplainDal.Evaluation(evaluation, isSatisfying, id);
            return Json(sr);
        }
    }
}