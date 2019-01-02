using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class EquipmentController : Controller
    {
        /// <summary>
        /// 获取设备保养信息
        /// </summary>
        /// <param name="classify"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetEquipment(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(isDone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = EquipmentDal.GetEquipment(classify, isDone);
            return Json(sr);
        }

        /// <summary>
        /// 提交设备保养信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isDone"></param>
        /// <param name="inputMan"></param>
        /// <param name="doneInfo"></param>
        /// <param name="inputDate"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetEquipment(string id, string isDone, string inputMan, string doneInfo, string inputDate)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(id))
            {
                sr.status = "Fail";
                sr.result = "ID不能为空";
                return Json(sr);
            }
            sr = EquipmentDal.SetEquipment(id, isDone, inputMan, doneInfo, inputDate);
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
            try
            { 
                string mainPath = "F:\\wytws\\Files\\bywj\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string ID = Request.Form["ID"];
                string func = Request.Form["func"];
                sr = EquipmentDal.SetEquipmentImage(ID, func, sqlImagePath);
                return Json(sr);
            }
            catch(NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
            
            
        }

        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <param name="operationNumber"></param>
        /// <returns></returns>
        public ActionResult OnSearchEquipment(string operationNumber)
        {
            StatusReport sr = new StatusReport();
            sr = EquipmentDal.SearchEquipment(operationNumber);
            return Json(sr);
        }

        public ActionResult OnSearchEquipmentMaintain(string operationNumber)
        {
            StatusReport sr = new StatusReport();
            sr = EquipmentDal.SearchEquipmentMaintain(operationNumber);
            return Json(sr);
        }


        /// <summary>
        /// 获取设备故障信息
        /// </summary>
        /// <param name="classify"></param>
        /// <param name="isDone"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetEquipmentTrouble(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(isDone))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = EquipmentDal.GetEquipmentTrouble(classify, isDone);
            return Json(sr);
        }

        /// <summary>
        /// 提交设备故障维修情况
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fee"></param>
        /// <param name="doneTime"></param>
        /// <param name="doneInfo"></param>
        /// <returns></returns>
        public ActionResult OnSetEquipmentTrouble(string id, string fee, string doneTime, string doneInfo)
        {
            StatusReport sr = new StatusReport();
            sr = EquipmentDal.SetEquipmentTrouble(id, fee, doneInfo, doneTime);
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
                sr = EquipmentDal.SetEquipmentTroubleImage(ID, func, index, sqlImagePath);
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
        //public ActionResult OnGetEquipmentTroubleStatistics(string ztcode, string level)
        //{
        //    StatusReport sr = new StatusReport();
        //    sr = EquipmentDal.GetEquipmentTroubleStatistics(ztcode,level);
        //    return Json(sr);
        //}

    }
}


