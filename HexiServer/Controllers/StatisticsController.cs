using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class StatisticsController : Controller
    {
        /// <summary>
        /// 财务_月收费统计
        /// </summary>
        /// <param name="ztcode">帐套代码</param>
        /// <param name="level">用户身份</param>
        /// <param name="startTime">开始时间</param>
        /// <returns></returns>
        public ActionResult OnGetMonthChargeStatistics(string ztcode, string level, string startTime )
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(level) || string.IsNullOrEmpty(ztcode) || string.IsNullOrEmpty(startTime))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = ChargeDal.GetMonthChargeStatistics(ztcode, level, startTime);
            return Json(sr);
        }


        public ActionResult OnGetArrearageStatistics(string ztcode, string level, string month, string type)
        {
            //StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(level) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(month))
            {
                return Json(new StatusReport().SetFail("信息不完整"));
            }
            if (level == "公司")
            {
                return Json(ChargeDal.GetArrearageStatisticsCompany(month, type));
            }
            else if (level == "项目经理")
            {
                return Json(ChargeDal.GetArrearageStatisticsProject(ztcode, month, type));
            }
            else
            {
                return Json(new StatusReport().SetFail("没有权限"));
            }
        }


        public ActionResult OnGetChargeStatistics(string ztcode, string level, string month)
        {
            //StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(level) || string.IsNullOrEmpty(month))
            {
                return Json(new StatusReport().SetFail("信息不完整"));
            }
            if (level == "公司")
            {
                return Json(ChargeDal.GetChargeStatisticsCompany(month));
            }
            else if (level == "项目经理")
            {
                return Json(ChargeDal.GetChargeStatisticsProject(month,ztcode));
            }
            else
            {
                return Json(new StatusReport().SetFail("没有权限"));
            }
        }


        //public ActionResult OnGetStatistics(string ztcode, string level, string func, string username, string before,string month)
        //{
        //    StatusReport sr = new StatusReport();
        //    if (string.IsNullOrEmpty(level) || string.IsNullOrEmpty(func))
        //    {
        //        sr.status = "Fail";
        //        sr.result = "信息不完整";
        //        return Json(sr);
        //    }
        //    switch (func)
        //    {
        //        case "收费统计":
        //sr = ChargeDal.GetChargeStatistics(ztcode, level, username, month);
        //            break;
        //        case "工单统计":
        //sr = RepairDal.GetRepairStatistics(ztcode, level, username, before);
        //            break;
        //        case "设备统计":
        //            sr = EquipmentDal.GetEquipmentStatistics(ztcode, level);
        //            break;
        //        case "投诉统计":
        //            sr = ComplainDal.GetComplainStatistics(ztcode,level, before);
        //            break;
        //        case "设备故障统计":
        //            sr = EquipmentDal.GetEquipmentTroubleStatistics(ztcode, level);
        //            break;
        //    }

        //    return Json(sr);
        //}
    }
}