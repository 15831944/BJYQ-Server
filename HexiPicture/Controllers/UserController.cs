using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using HexiUtils;
using HexiPicture.Models;

namespace HexiPicture.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// 微信登陆，如成功则返回3th session-key。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnLogin(string code)
        {
            var jsonResult = SnsApi.JsCode2Json(Common.Appid, Common.AppSecret, code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                //Session["WxOpenUser"] = jsonResult;
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                Session[sessionBag.Key] = jsonResult;
                Session.Timeout = 60;
                return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key, result = Session[sessionBag.Key] });
            }
            else
            {
                return Json(new { success = false, mag = jsonResult.errmsg, result = jsonResult });
            }
        }

        ////[HttpGet]
        ///// <summary>
        ///// 获取员工信息，使用openId获取员工信息
        ///// </summary>
        ///// <param name="sessionId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult OnGetUserInfo(string sessionId)
        //{
        //    StatusReport sr = new StatusReport();

        //    if (string.IsNullOrEmpty(sessionId))//如果sessionId为空，则返回错误信息
        //    {
        //        sr.status = "Fail";
        //        sr.result = "sessionId不存在";
        //        sr.parameters = sessionId;
        //        return Json(sr);
        //    }
        //    SessionBag sessionBag = null;
        //    sessionBag = SessionContainer.GetSession(sessionId);
        //    if (sessionBag == null)
        //    {
        //        sr.status = "Fail";
        //        sr.result = "session已失效";
        //        return Json(sr);
        //    }
        //    string openId = sessionBag.OpenId;
        //    sr = EmployeeDal.CheckOpenIdExist(openId);
        //    //if (sr.data != null)
        //    //{
        //    //    var o = JsonConvert.DeserializeObject(sr.data);
        //    //    return Json(new { status = "Success", result = "成功", data = o });
        //    //}
        //    //else
        //    //{
        //    return Json(sr);
        //    //}
        //}

        ///// <summary>
        ///// 将用户Id与openId进行绑定
        ///// </summary>
        ///// <param name="userName"></param>
        ///// <param name="password"></param>
        ///// <param name="sessionId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult OnBindUser(string userName, string password, string sessionId)
        //{
        //    StatusReport sr = new StatusReport();
        //    SessionBag sessionBag = null;
        //    sessionBag = SessionContainer.GetSession(sessionId);
        //    if (sessionBag == null)
        //    {
        //        sr.status = "Fail";
        //        sr.result = "session已失效";
        //        return Json(sr);
        //    }
        //    string openId = sessionBag.OpenId;
        //    //string openId = "oTTDy0KN71B2XLMXobrapvhqlHcY";
        //    int id = EmployeeDal.CheckEmployeeExist(userName, password);
        //    string temp = id > 0 ? "存在" : "不存在";
        //    if (id > 0)
        //    {
        //        sr = EmployeeDal.BindEmployee(id, openId);
        //        return Json(sr);
        //    }
        //    else
        //    {
        //        var data = new
        //        {
        //            msg = "hello world",
        //            username = userName,
        //            password = password,
        //            isExist = temp
        //        };
        //        return Json(data);
        //    }

    }
}