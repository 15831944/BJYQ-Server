using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult OnGetReport(string ztcode, string level, string func, string username, string before)
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
                case "工单上报":
                    sr = RepairDal.GetRepairReport(ztcode, level);
                    break;
                case "设备上报":
                    if (level == "公司")
                    {
                        sr = EquipmentDal.GetEquipmentReportAbstractList();
                    }
                    else if (level == "一线")
                    {
                        sr.status = "Fail";
                        sr.result = "没有此权限";
                    }
                    else
                    {
                        sr = EquipmentDal.GetEquipmentReport(ztcode);
                    }
                    break;
                case "设备故障上报":
                    if (level == "公司")
                    {
                        sr = EquipmentDal.GetEquipmentTroubleReportAbstractList();
                    }
                    else if (level == "一线")
                    {
                        sr.status = "Fail";
                        sr.result = "没有此权限";
                    }
                    else
                    {
                        sr = EquipmentDal.GetEquipmentTroubleReport(ztcode);
                    }
                    break;
                case "投诉上报":
                    if (level == "一线" || level == "助理" || level == "项目经理")
                    {
                        sr.status = "Fail";
                        sr.result = "没有此权限";
                    }
                    else
                    {
                        sr = ComplainDal.GetComplainReport();
                    }
                    break;
            }

            return Json(sr);
        }
    }
}