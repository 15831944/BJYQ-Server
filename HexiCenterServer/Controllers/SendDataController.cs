using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using System.Collections.Specialized;

namespace HexiCenterServer.Controllers
{
    public class SendDataController : Controller
    {
        // GET: SendData

        public ActionResult Index()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc = Request.QueryString;
            string queryString = nvc.ToString();
            string url = Request["url"];
            return Json(queryString,JsonRequestBehavior.AllowGet);
        }
    }
}