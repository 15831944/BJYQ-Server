using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using HexiUserServer.Business;
using HexiUserServer.Models;
using HexiUtils;

namespace HexiServer.Controllers
{
    public class WxUserController : Controller
    {
        //HttpSessionStateBase mySession
        /// <summary>
        /// 微信登陆，如成功则返回3th session-key。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnLogin(string code)
        {
            StatusReport sr = new StatusReport();
            var jsonResult = SnsApi.JsCode2Json(Common.Appid, Common.AppSecret, code);
            if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
            {
                var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key);
                Session[sessionBag.Key] = jsonResult;
                Session.Timeout = 60;
                string openId = sessionBag.OpenId;
                sr = ProprietorDal.CheckOpenIdExist(openId);
                sr.parameters = sessionBag.Key;
                return Json(sr);
                //return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key, result = Session[sessionBag.Key] });
            }
            else
            {
                return Json(sr.SetFail("微信登录失败：" + jsonResult.errmsg));
                //return Json(new { success = false, mag = jsonResult.errmsg, result = jsonResult });
            }
        }

        //[HttpGet]
        /// <summary>
        /// 获取员工信息，使用openId获取占用者信息
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnGetUserInfo(string sessionId)
        {
            StatusReport sr = new StatusReport();

            if (string.IsNullOrEmpty(sessionId))//如果sessionId为空，则返回错误信息
            {
                sr.status = "Fail";
                sr.result = "sessionId不存在";
                sr.parameters = sessionId;
                return Json(sr);
            }
            SessionBag sessionBag = null;
            sessionBag = SessionContainer.GetSession(sessionId);
            if (sessionBag == null)
            {
                sr.status = "Fail";
                sr.result = "session已失效";
                return Json(sr);
            }
            string openId = sessionBag.OpenId;
            sr = ProprietorDal.CheckOpenIdExist(openId);
            //if (sr.data != null)
            //{
            //    var o = JsonConvert.DeserializeObject(sr.data);
            //    return Json(new { status = "Success", result = "成功", data = o });
            //}
            //else
            //{
            return Json(sr);
            //}
        }


        /// <summary>
        /// 将用户Id与openId进行绑定
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OnBindUser(string userName, string phoneNumber, string sessionId)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(sessionId))//如果sessionId为空，则返回错误信息
            {
                sr.status = "Fail";
                sr.result = "sessionId不存在";
                sr.parameters = sessionId;
                return Json(sr);
            }
            SessionBag sessionBag = null;
            sessionBag = SessionContainer.GetSession(sessionId);
            if (sessionBag == null)
            {
                sr.status = "Fail";
                sr.result = "session已失效";
                return Json(sr);
            }
            string openId = sessionBag.OpenId;
            return Json(ProprietorDal.BindProprietor(userName, phoneNumber, openId));

        }



        [HttpPost]
        public ActionResult OnGetCode(string userName, string phoneNumber)
        {
            return Json(ProprietorDal.getCode(userName, phoneNumber));
        }

        /// <summary>
        /// 添加家庭成员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address"></param>
        /// <param name="birth"></param>
        /// <param name="company"></param>
        /// <param name="idNumber"></param>
        /// <param name="idType"></param>
        /// <param name="job"></param>
        /// <param name="nation"></param>
        /// <param name="nationality"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="relation"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        //public ActionResult OnAddFamily(int id, string gender, string address, string birth, string company, string idNumber, string idType, string job, string nation, string nationality, string phoneNumber, string relation, string userName, string[] roomId)
        //{
        //    StatusReport sr = new StatusReport();
        //    sr = ProprietorDal.AddFamily(id, gender, address, birth, company, idNumber, idType, job, nation, nationality, phoneNumber, relation, userName,roomId);
        //    return Json(sr);

        //}

        public ActionResult OnAddFamily(int id, string phoneNumber, string relation, string userName, string roomId)
        {
            StatusReport sr = new StatusReport();
            sr = ProprietorDal.AddFamily(id, phoneNumber, relation, userName, roomId);
            return Json(sr);

        }

        [HttpPost]
        public ActionResult OnGetFamilyMembers(string name, string phone)
        {
            return Json(ProprietorDal.GetFamilyMembers(name, phone));
        }
    }
}