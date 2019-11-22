using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;
using HexiUserServer.Models;

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
        public ActionResult OnGetComplaintList( string name, string phone)
        {
            StatusReport sr = new StatusReport();
            if ( string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                sr.parameters = Request.QueryString;
            }
            else
            {
                sr = ComplaintDal.GetComplaintList(name,phone);
                sr.parameters = Request.QueryString;
            }
            return Json(sr);
        }


        //public ActionResult OnSetComplaintImage()
        //{


        //    StatusReport sr = new StatusReport();
        //    if (Request.Files.Count == 0)
        //    {
        //        sr.status = "Fail";
        //        sr.result = "没有图片";
        //        return Json(sr);
        //    }
        //    try
        //    {
        //        string mainPath = Comman.file_main_path + "基础资料_巡检记录\\";
        //        string imagePath = mainPath + Request.Files.AllKeys[0];
        //        //string sqlImagePath = Request.Files.AllKeys[0];
        //        HttpPostedFileBase uploadImage = (Request.Files[0]);
        //        uploadImage.SaveAs(imagePath);
        //        string ID = Request.Form["id"];
        //        //using (StreamWriter sw = new StreamWriter("D:\\1_importTemp\\TestFile2.txt"))
        //        //{
        //        //    sw.WriteLine(ID);
        //        //    //sw.WriteLine(JsonConvert.SerializeObject(items));
        //        //}
        //        //string func = Request.Form["func"];
        //        string index = Request.Form["index"];

        //        string sqlImagePath = "~~-" + (3322 + Convert.ToInt32(index)) + "-" + ID.ToString() + "|" + Request.Files.AllKeys[0];
        //        sr = LookOverDal.SetLookOverImage(ID, index, sqlImagePath);
        //        return Json(sr);
        //    }
        //    catch (NotImplementedException exp)
        //    {
        //        sr.status = "Fail";
        //        sr.result = exp.Message;
        //        return Json(sr);
        //    }
        //}

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
                string mainPath = Config.complaintImageMainPath;
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