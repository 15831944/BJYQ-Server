using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using HexiUserServer.Business;
using HexiUtils;
using System.Collections.Specialized;
using System.IO;

namespace HexiUserServer.Controllers
{
    public class ChargeController : Controller
    {
        /// <summary>
        /// 获取应收列表
        /// </summary>
        /// <param name="ztCode"></param>
        /// <param name="roomNumber"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetCharges(string ztCode, string roomNumber, string userName )
        {
            return Json(ChargeDal.GetCharges(ztCode, roomNumber, userName));
        }

        /// <summary>
        /// 收费成功后变更应收款状态
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="proprietorName"></param>
        /// <param name="chargeIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnSetCharges(string datetime, string proprietorName, string tradeNumber, string[] chargeIds)
        {
            //return Json(new { datetime = datetime, proprietorName = proprietorName, chargeIds = chargeIds });
            return Json(ChargeDal.SetCharges(datetime, proprietorName, tradeNumber, chargeIds));
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="totalCharge"></param>
        /// <param name="dataBag"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnUnifiedOrder(string sessionId, double totalCharge, string dataBag)
        {
            StatusReport sr = new StatusReport();
            SessionBag sessionBag = null;
            sessionBag = SessionContainer.GetSession(sessionId);
            if (sessionBag == null)
            {
                sr.status = "Fail";
                sr.result = "session已失效";
                return Json(sr);
            }
            string openId = sessionBag.OpenId;
            sr = WXPayDal.UnifiedOrder(openId, totalCharge, dataBag);
            return Json(sr);
        }


        public ActionResult OnTest()
        {
            
            StreamReader reader = new StreamReader(Request.InputStream);
            string value = reader.ReadToEnd();
            using (StreamWriter sw = new StreamWriter("D:\\1_importTemp\\TestFile.txt"))
            {
                sw.WriteLine(value);
            }
            return null;
        }

        //public ActionResult OnGetPrepayId(string xmlResult)
        //{
        //    return Json(WXPayDal.GetOrderInfo(xmlResult));
        //}
    }
}