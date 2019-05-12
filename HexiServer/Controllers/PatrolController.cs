using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiServer.Business;
using HexiServer.Models;
using HexiUtils;

namespace HexiServer.Controllers
{
    public class PatrolController : Controller
    {
        // GET: Patrol
        /// <summary>
        /// 将报事信息写入数据库
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="detail"></param>
        /// <param name="classify"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public ActionResult OnSetPatrol(string name, string address, string detail, string classify, string time)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(detail) || string.IsNullOrEmpty(classify) || string.IsNullOrEmpty(time))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = RepairDal.SetPatrol(name, address, detail, classify, time);
            return Json(sr);
        }

        /// <summary>
        /// 获取历史报事信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        public ActionResult OnGetPatrol(string name, string classify)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(classify))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = RepairDal.GetPatrol(name, classify);

            return Json(sr);
        }

        /// <summary>
        /// 保存报事照片
        /// </summary>
        /// <returns></returns>
        public ActionResult OnSetPatrolImage()
        {
            StatusReport sr = new StatusReport();
            if (Request.Files.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "没有图片";
                return Json(sr);
            }
            try
            {
                string imagePath = Config.patrolImageMainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string ID = Request.Form["id"];
                string func = Request.Form["func"];//a
                string index = Request.Form["index"];
                sr = RepairDal.SetPatrolImage(func, ID, index, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }
        }
    }
}