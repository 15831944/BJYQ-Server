using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiServer.Business;
using HexiServer.Models;
using System.IO;
using Newtonsoft.Json;

namespace HexiServer.Controllers
{
    public class LookOverController : Controller
    {
        public ActionResult GetLookOverRouteInfo(string ztCode, string func, string name)
        {
            StatusReport sr = new StatusReport();
            return Json(LookOverDal.GetLookOverRouteInfo(ztCode, func, name));
        }

        public ActionResult OnGetLookOverInfo(string ztCode, string func, string route, string name)
        {
            StatusReport sr = new StatusReport();
            return Json(LookOverDal.GetLookOverInfo(ztCode, func,route, name));
            
        }

        public ActionResult OnSetLookOverResult(string isSpotCheck, string items)
        {
            return Json(LookOverDal.SetLookOverResult(isSpotCheck,items));
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
                string mainPath = Comman.file_main_path + "基础资料_巡检记录\\";
                string imagePath = mainPath + Request.Files.AllKeys[0];
                //string sqlImagePath = Request.Files.AllKeys[0];
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

                string sqlImagePath = "~~-" + (3322 + Convert.ToInt32(index)) + "-" + ID.ToString() + "|" + Request.Files.AllKeys[0];
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