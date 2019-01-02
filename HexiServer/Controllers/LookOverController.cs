using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;
using System.IO;
using Newtonsoft.Json;

namespace HexiServer.Controllers
{
    public class LookOverController : Controller
    {
        // GET: LookOver
        public ActionResult OnGetLookOverInfo(string ztCode, string func, string period)
        {
            StatusReport sr = new StatusReport();
            return Json(LookOverDal.GetLookOverInfo(ztCode, func,period));
            //if (func == "空房巡检")
            //{
            //    return Json(LookOverDal.GetHouseLookOverInfo(ztCode, func));
            //}
            //else
            //{
            //    return Json(LookOverDal.GetNormalLookOverInfo(ztCode, func));
            //}
        }

        public ActionResult OnSetLookOverResult(string userName, string isSpotCheck, string items)
        {
            return Json(LookOverDal.SetLookOverResult(userName,isSpotCheck,items));
        }

        public ActionResult OnSetLookOverImage()
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
                //using (StreamWriter sw = new StreamWriter("D:\\1_importTemp\\TestFile2.txt"))
                //{
                //    sw.WriteLine(ID);
                //    //sw.WriteLine(JsonConvert.SerializeObject(items));
                //}
                //string func = Request.Form["func"];
                string index = Request.Form["index"];
                sr = LookOverDal.SetLookOverImage(ID, index, sqlImagePath);
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