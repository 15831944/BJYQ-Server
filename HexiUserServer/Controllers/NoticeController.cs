using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class NoticeController : Controller
    {
        // GET: Notice
        [HttpPost]
        public ActionResult OnGetNotice(string ztcode)
        {
            return Json(NoticeDal.GetNotice(ztcode));
        }

        [HttpPost]
        public ActionResult OnGetLaw(string ztcode)
        {
            return Json(NoticeDal.GetLaw(ztcode));
        }
    }
}