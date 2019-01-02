using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;

namespace HexiUserServer.Controllers
{
    public class SMSTestController : Controller
    {
        // GET: SMSTest
        public ActionResult Index()
        {
            //SMSHelper.SendMessage("18514502514");
            HttpApplicationStateBase state = this.HttpContext.Application;
            state.Add("phone", "123456789");
            return Json(state, JsonRequestBehavior.AllowGet);

            //return Json(SMSHelper.SendMessage("18514502514",""),JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test()
        {
            HttpApplicationStateBase state = this.HttpContext.Application;
            return Json(state.AllKeys, JsonRequestBehavior.AllowGet);
        }
    }
}