using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class MyServerController : Controller
    {
        [HttpPost]
        public ActionResult OnGetServerInfo(string serverName, string ztName)
        {
            StatusReport sr = new StatusReport();
            sr = MyServerDal.GetServerInfo(serverName, ztName);
            return Json(sr);
        }
    }
}