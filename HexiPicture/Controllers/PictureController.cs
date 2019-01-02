using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using HexiUtils;
using HexiPicture.Business;

namespace HexiPicture.Controllers
{
    public class PictureController : Controller
    {
        // GET: Picture
        [HttpPost]
        public ActionResult OnGetPeriodsInfo(string sessionKey)
        {
            StatusReport sr = new StatusReport();
            sr = PictureDal.GetPeriodsInfo();
            return Json(sr);
        }

        public ActionResult OnSetPicture ()
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
                string mainPath = "D:\\wximages\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string openid = GetOpenid(Request.Form["sessionKey"]);
                string nackname = Request.Form["nackname"];
                string phone = Request.Form["phone"];
                string picName = Request.Form["picName"];
                string periodId = Request.Form["periodId"];
                string periods = Request.Form["periods"];
                string theme = Request.Form["theme"];
                string submitTime = Request.Form["submitTime"];
                string description = Request.Form["picDescription"];
                sr = PictureDal.SetPicture(openid, nackname, phone, picName, periodId, periods, theme, sqlImagePath,submitTime,description);
                //sr = EquipmentDal.SetEquipmentImage(ID, func, sqlImagePath);
                return Json(sr);
            }
            catch (NotImplementedException exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                return Json(sr);
            }

        }

        [HttpPost]
        public ActionResult OnGetPictures(string periodId, string sortType)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(periodId))
            {
                sr.status = "Fail";
                sr.result = "信息不完整";
                return Json(sr);
            }
            sr = PictureDal.GetPictures(periodId,sortType);
            return Json(sr);
        }

        public ActionResult OnSetPictureVote(string id, string periodId, string sessionKey)
        {
            StatusReport sr = new StatusReport();
            string openid = GetOpenid(sessionKey);
            int vote = PictureDal.GetUserVote(openid, periodId);
            if (vote >= 3)
            {
                sr.status = "fail";
                sr.result = "投票次数超过限制";
                return Json(sr);
            }
            sr = PictureDal.SetUserVote(id, openid, periodId);
            if (sr.status == "Fail")
            {
                return Json(sr);
            }
            sr = PictureDal.SetPictureVote(id);
            return Json(sr);
        }

        public ActionResult OnSetPictureTaped(string picId)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(picId))
            {
                sr.status = "Fail";
                sr.result = "未提供图片ID";
                return Json(sr);
            }
            sr = PictureDal.SetPictureTaped(picId);
            return Json(sr);
        }


        private string GetOpenid(string sessionKey)
        {
            SessionBag sessionBag = SessionContainer.GetSession(sessionKey);
            string opeinId = sessionBag.OpenId;
            return opeinId;
        }


    }
}