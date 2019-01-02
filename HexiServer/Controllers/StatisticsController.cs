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
        // GET: Statistics
        public ActionResult OnGetStatistics(string ztcode, string level, string func, string username, string before,string month)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(level) || string.IsNullOrEmpty(func))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            switch (func)
            {
                case "收费统计":
                    sr = ChargeDal.GetChargeStatistics(ztcode, level, username, month);
                    break;
                case "工单统计":
                    sr = RepairDal.GetRepairStatistics(ztcode, level, username, before);
                    break;
                case "设备统计":
                    sr = EquipmentDal.GetEquipmentStatistics(ztcode, level);
                    break;
                case "投诉统计":
                    sr = ComplainDal.GetComplainStatistics(ztcode,level, before);
                    break;
                case "设备故障统计":
                    sr = EquipmentDal.GetEquipmentTroubleStatistics(ztcode, level);
                    break;
            }

            return Json(sr);
        }
    }
}