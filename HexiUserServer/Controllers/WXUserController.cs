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
            SessionBag sessionBag = null;
            sessionBag = SessionContainer.GetSession(sessionId);
            //string sessionCode = (string)sessionBag.Name;
            //string sessionCode =(string)HttpContext.Session[phoneNumber];

            if (sessionBag == null)//如果sessionId失效，返回失败信息
            {
                sr.status = "Fail";
                sr.result = "session已失效";
                return Json(sr, JsonRequestBehavior.AllowGet);
            }

            //if (string.IsNullOrEmpty(sessionCode))//如果sessionCode失效，返回失败信息
            //{
            //    sr.status = "Fail";
            //    sr.result = "codeSession已失效";
            //    sr.parameters = phoneNumber;
            //    return Json(sr,JsonRequestBehavior.AllowGet);
            //}

            //if (code != sessionCode)//如果验证码与用户收到的验证码不一致，返回失败信息
            //{
            //    sr.status = "Fail";
            //    sr.result = "code错误";
            //    return Json(sr, JsonRequestBehavior.AllowGet);
            //}

            string openId = sessionBag.OpenId;
            int id = ProprietorDal.CheckProprietorExist(userName, phoneNumber);
            string temp = id != 0 ? "存在" : "不存在";
            if (id != 0)
            {
                sr = ProprietorDal.BindProprietor(Math.Abs(id), userName, phoneNumber, openId, id > 0 ? true : false);
                return Json(sr, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = new
                {
                    msg = "hello world",
                    username = userName,
                    phoneNumber = phoneNumber,
                    isExist = temp
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult OnGetCode(string userName, string phoneNumber)
        {
            
            //StatusReport sr = new StatusReport();
            //SessionBag sessionBag = null;
            //sessionBag = SessionContainer.GetSession(sessionId);
            //if (sessionBag == null)
            //{
            //    sr.status = "Fail";
            //    sr.result = "session已失效";
            //    return Json(sr);
            //}
            //string openId = sessionBag.OpenId;
            int id = ProprietorDal.CheckProprietorExist(userName, phoneNumber);
            string temp = id > 0 ? "存在" : "不存在";
            if (id != 0)
            {
                string code = getCode();
                //SessionBag sessionBag = null;
                //sessionBag = SessionContainer.GetSession(sessionId);
                //sessionBag.Name = code;
                //filterContext.HttpContext.Session.Add(phoneNumber, code);
                //Session[phoneNumber] = code;
                //Session.Timeout = 60;
                var data = new
                {
                    msg = "success",
                    code = code,
                    status = "exist"
                };
                return Json(data);
                //sr = ProprietorDal.BindProprietor(id, openId);
                //return Json(sr);
            }
            else
            {
                var data = new
                {
                    msg = "fail",
                    code = "",
                    status = "notexist"
                };
                return Json(data);
            }
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
        public ActionResult OnAddFamily(int id, string gender, string address, string birth, string company, string idNumber, string idType, string job, string nation, string nationality, string phoneNumber, string relation, string userName, string[] roomId)
        {
            StatusReport sr = new StatusReport();
            sr = ProprietorDal.AddFamily(id, gender, address, birth, company, idNumber, idType, job, nation, nationality, phoneNumber, relation, userName,roomId);
            return Json(sr);

        }

        [HttpPost]
        public ActionResult OnGetFamilyMembers(int id)
        {
            return Json(ProprietorDal.GetFamilyMembers(id));
        }


        private static string getCode()
        {
            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);
            return randomNumber.ToString();
        }
    }
}