using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;

namespace HexiServer.Controllers
{
    public class DecorationController : Controller
    {
        // GET: Decoration
        [HttpPost]
        public ActionResult OnGetDecorationList(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            sr = DecorationDal.GetDecorationList(classify, isDone);
            return Json(sr);
        }

        [HttpPost]
        public ActionResult OnGetDecorationPatrolList(string classify)
        {
            StatusReport sr = new StatusReport();
            sr = DecorationDal.GetDecorationPatrolList(classify);
            return Json(sr);
        }

        [HttpPost]
        public ActionResult OnSetDecorationResult(string id, string accessController, string accessControllerSignDate,
            string checkEngineer, string checkEngineerSignDate, string checkMan1, string checkResult1, string checkResultExplain1, 
            string checkTime1, string checkMan2, string checkResult2, string checkResultExplain2,
            string checkTime2, string engineeringCheckAccessMan, string engineeringCheckAccessManSignDate, 
            string engineeringCheckMan, string engineeringCheckManSignDate, string engineeringManager,
            string engineeringManagerSignDate, string engineeringPatrolMan, string engineeringPatrolManSignDate)
        {
            StatusReport sr = new StatusReport();
            sr = DecorationDal.SetDecorationResult(id, accessController, accessControllerSignDate, checkEngineer, checkEngineerSignDate,
                checkMan1, checkResult1, checkResultExplain1, checkTime1, checkMan2, checkResult2, checkResultExplain2, checkTime2,
                engineeringCheckAccessMan, engineeringCheckAccessManSignDate, engineeringCheckMan, engineeringCheckManSignDate,
                engineeringManager, engineeringManagerSignDate, engineeringPatrolMan, engineeringPatrolManSignDate);
            return Json(sr);
        }


        public ActionResult OnSetDecorationPatrolResult(string id, string classify, string checkMan, string checkDate, 
            string dealWay, string havePeople, string isNormal, string otherUnnormalItemExplain, string schedule, 
            string unnormalItemNumber, string roomNumber)
        {
            StatusReport sr = new StatusReport();
            sr = DecorationDal.SetDecorationPatrolResult(id, classify, checkMan, checkDate, dealWay, havePeople, isNormal, 
                otherUnnormalItemExplain, schedule, unnormalItemNumber,roomNumber);
            return Json(sr);
        }

        public ActionResult OnSetDecorationPatrolImage()
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
                string mainPath = "F:\\wytws\\Files\\jczl_fwrwgl\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                string sqlImagePath = Request.Files.AllKeys[0];
                HttpPostedFileBase uploadImage = (Request.Files[0]);
                uploadImage.SaveAs(imagePath);
                string ID = Request.Form["id"];
                //string func = Request.Form["func"];
                string index = Request.Form["index"];
                sr = DecorationDal.SetDecorationPatrolImage(ID, index, sqlImagePath);
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