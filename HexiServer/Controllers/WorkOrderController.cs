using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using HexiServer.Business;
using HexiServer.Models;
using HexiUtils;

namespace HexiServer.Controllers
{
    public class WorkOrderController : Controller
    {

        /// <summary>
        /// 获取某接单人的工单列表
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="ztCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetRepairList(string userCode, string ztCode, string status, string orderType)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(ztCode) || string.IsNullOrEmpty(status) || string.IsNullOrEmpty(orderType))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = RepairDal.GetRepairOrder(userCode, ztCode, status, orderType);

            return Json(sr);
        }




        /// <summary>
        /// 将工单的处理详情写入数据库
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="id"></param>
        /// <param name="arriveTime"></param>
        /// <param name="completeTime"></param>
        /// <param name="completeStatus"></param>
        /// <param name="laborExpense"></param>
        /// <param name="materialExpense"></param>
        /// <returns></returns>
        public ActionResult OnSetRepairOrder(string sessionId, string id, string arriveTime, string completeTime, string completeStatus, string chargeType, string laborExpense, string materialExpense, string status, string lateReason, string lateTime, string isPaid)
        {
            StatusReport sr = new StatusReport();
            sr = RepairDal.SetRepairOrder(id, arriveTime, completeTime, completeStatus, chargeType, laborExpense, materialExpense,status,lateReason,lateTime,isPaid);
            return Json(sr);
        }



        public ActionResult OnSetPatrol(string name, string address, string detail, string classify, string time)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(detail) || string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(time))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = RepairDal.SetPatrol(name, address, detail, classify, time);
            return Json(sr);
        }

        public ActionResult OnGetPatrol(string name, string classify)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(classify))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = RepairDal.GetPatrol(name,classify);

            return Json(sr);
        }



        public ActionResult OnSetOrderIsRead(string id)
        {
            StatusReport sr = new StatusReport();
            sr = RepairDal.SetOrderIsRead(id);
            return Json(sr);
        }


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

        public ActionResult OnSetPatrolImage()
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
                sr = RepairDal.SetPatrolImage(func, ID, index, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }




        //private string GetOpenId (string sessionId)
        //{
        //    SessionBag sessionbag = null;
        //    sessionbag = SessionContainer.GetSession(sessionId);
        //    if (sessionbag != null)
        //    {
        //        return sessionbag.OpenId;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}