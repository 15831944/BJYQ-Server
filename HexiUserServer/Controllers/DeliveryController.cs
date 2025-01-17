﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiUserServer.Business;
using HexiUtils;

namespace HexiUserServer.Controllers
{
    public class DeliveryController : Controller
    {
        [HttpPost]
        public ActionResult OnGetDelivery(string phone)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(phone))
            {
                sr.status = "Fail";
                sr.result = "查询参数不完整";
            }
            else
            {
                sr = DeliveryDal.GetDelivery(phone);
            }
            return Json(sr);
        }
    }
}