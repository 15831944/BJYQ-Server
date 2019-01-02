using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class RequestPaymentFlowController : Controller
    {
        [HttpPost]
        public ActionResult OnGetRequestPaymentSheet(string userId)
        {
            StatusReport sr = new StatusReport();
            sr = RequestPaymentFlowDal.GetRequestPaymentSheet(userId);
            return Json(sr);
        }

        public ActionResult OnSetRequestPaymentFlow(string userName, string result, string leaveWord, string registerId, string linkId, string id, string documentId, string documentNumber, string opinion)
        {
            StatusReport sr = new StatusReport();
            sr = RequestPaymentFlowDal.SetRequestPaymentFlow(userName, result, leaveWord, registerId, linkId, id, documentId, documentNumber, opinion);
            return Json(sr);
        }
    }
}