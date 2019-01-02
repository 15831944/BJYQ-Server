using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUtils;
using HexiUserServer.Business;

namespace HexiUserServer.Controllers
{
    public class HouseController : Controller
    {
        // GET: House
        [HttpPost]
        public ActionResult OnGetHouseDetail(string roomNumber)
        {
            return Json(HouseDal.GetHouseDetail(roomNumber));
        }
    }
}