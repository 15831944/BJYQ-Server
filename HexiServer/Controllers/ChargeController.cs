using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiServer.Business;
using HexiUtils;

namespace HexiServer.Controllers
{
    public class ChargeController : Controller
    {
        /// <summary>
        /// 获取符合查询条件的已收费列表
        /// </summary>
        /// <param name="homeNumber"></param>
        /// <param name="name"></param>
        /// <param name="ztcode"></param>
        /// <param name="startMonth"></param>
        /// <param name="endMonth"></param>
        /// <param name="isCharge"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetChargedList(string homeNumber, string buildingNumber, string name, string ztcode, string startMonth, string endMonth)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(ztcode) || string.IsNullOrEmpty(startMonth) || string.IsNullOrEmpty(endMonth))
            {
                return Json(new {status = "Fail" , result = "信息不完整" });
            }
            return Json(ChargeDal.GetChargedList(homeNumber, buildingNumber, name, ztcode, startMonth, endMonth));
        }

        
        /// <summary>
        /// 获取选定的已收费详情
        /// </summary>
        /// <param name="RoomNumber"></param>
        /// <param name="Name"></param>
        /// <param name="ZTCode"></param>
        /// <param name="startMonth"></param>
        /// <param name="endMonth"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetChargedDetail(string RoomNumber, string Name, string ZTCode, string startMonth, string endMonth)
        {
            if (string.IsNullOrEmpty(RoomNumber) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ZTCode) || string.IsNullOrEmpty(startMonth) || string.IsNullOrEmpty(endMonth))
            {
                return Json(new { status = "Fail", result = "信息不完整" });
            }
            return Json(ChargeDal.GetChargedDetail(RoomNumber, Name, ZTCode, startMonth, endMonth));
        }

        /// <summary>
        /// 获取符合查询条件的应收款列表
        /// </summary>
        /// <param name="homeNumber"></param>
        /// <param name="name"></param>
        /// <param name="ztcode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetChargeList(string homeNumber, string name, string ztcode, string buildingNumber)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(ztcode))
            {
                return Json(new { status = "Fail", result = "信息不完整" });
            }
            return Json(ChargeDal.GetChargeList(homeNumber, name, ztcode, buildingNumber));
        }

        /// <summary>
        /// 获取选定的应收款详情
        /// </summary>
        /// <param name="ZTCode"></param>
        /// <param name="RoomNumber"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetChargeDetail(string ZTCode, string RoomNumber, string Name)
        {
            if (string.IsNullOrEmpty(RoomNumber) || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ZTCode))
            {
                return Json(new { status = "Fail", result = "信息不完整" });
            }
            return Json(ChargeDal.GetCharges(ZTCode, RoomNumber, Name));
        }


        [HttpPost]
        public ActionResult OnSetCharges(string datetime, string name, string[] chargeIds, string paymentMethod)
        {
            //return Json(new { datetime = datetime, proprietorName = proprietorName, chargeIds = chargeIds });
            return Json(ChargeDal.SetCharges(datetime, name, chargeIds, paymentMethod));
        }


        public ActionResult OnGetChargeStatistics(string ztCode, string level, string userCode, string month)
        {
            return Json(ChargeDal.GetChargeStatistics(ztCode, level, userCode, month),JsonRequestBehavior.AllowGet);
        }
    }
}