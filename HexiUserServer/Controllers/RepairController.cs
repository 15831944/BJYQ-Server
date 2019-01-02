using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class RepairController : Controller
    {
        /// <summary>
        /// 提交报修信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="address"></param>
        /// <param name="content"></param>
        /// <param name="time"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetRepairOrder(string name, string phone, string address, string content, string time,string classify)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(classify))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
            }
            else
            {
                sr = RepairDal.SetRepairOrder(name, phone, address, content, time,classify);
            }
            return Json(sr);
        }

        /// <summary>
        /// 获取报修列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetRepairOrder(string name, string phone)
        {
            StatusReport sr = new StatusReport();

            //throw new Exception("hello world");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
            }
            else
            {
                sr = RepairDal.GetRepairOrder(name, phone);
            }
            return Json(sr);
        }

        /// <summary>
        /// 上传报修图片
        /// </summary>
        /// <returns></returns>
        public ActionResult OnSetRepairImage()
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


        public ActionResult OnEvaluation(string evaluation, string isSatisfying, string isFinish, string id)
        {
            StatusReport sr = new StatusReport();
            sr = RepairDal.Evaluation(evaluation, isSatisfying, isFinish, id);
            return Json(sr);
        }
    }
}