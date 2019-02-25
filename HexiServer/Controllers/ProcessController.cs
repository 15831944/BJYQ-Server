using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HexiServer.Business;
using HexiUtils;

namespace HexiServer.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public ActionResult GetProcessList(string userId)
        {
            StatusReport sr = new StatusReport();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(sr.SetFail("userId无效或不存在"));
            }
            sr = ProcessDal.BusinessTask(userId);
            return Json(sr,JsonRequestBehavior.AllowGet);
        }

        public ActionResult BussinessHandler_TableData(string userId, string linkId, string docTableName, string docTableId, string businessId, string transferObjectId, string objectType, string transferObjectType)
        {
            StatusReport sr = ProcessDal.BussinessHandler_TableData(userId, linkId, docTableName, docTableId, businessId, transferObjectId, objectType, transferObjectType);
            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BussinessHandler_NextLink(string registId, string transferObjectId, string receiveLinkId, string receiveLinkIds, string userId, string businessId, string linkId, string task, string staffId, string lastInstanceId, string department, string secondDepartment, string thirdDepartment, string registerId)
        {
            StatusReport sr = ProcessDal.BussinessHandler_NextLink(registId,transferObjectId,receiveLinkId,receiveLinkIds,userId,businessId,linkId,task,staffId,lastInstanceId,department,secondDepartment,thirdDepartment,registerId);
            return Json(sr, JsonRequestBehavior.AllowGet);
        }


        public ActionResult BussinessHandler_NextLink_ReceiveStaffs(string registId, string userId, string businessId, string linkId, string task, string staffId, string operatorLimit, string roleMemberId, string handlerInfo, string lastInstanceId, string department, string secondDepartment, string thirdDepartment, string registerId)
        {
            StatusReport sr = ProcessDal.BussinessHandler_NextLink_ReceiveStaffs(userId,registId,businessId,linkId,task,staffId,operatorLimit,roleMemberId,handlerInfo,lastInstanceId,department,secondDepartment,thirdDepartment,registerId);
            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BussinessHandler_Save(string instanceId, string userId, string leaveMessage,
            string docTableName, string docTableId, string updateData, string updateDataKeys, string registId,
            string transformConditionAndExplain, string checkResult, string businessId, string tableNumber, string nextControlLinkId,
            string nextControlLink_UserId, string nextOutOfControlLinkIds, string nextOutOfControlLink_UserIds)
        {
            StatusReport sr = ProcessDal.BussinessHandler_Save(instanceId,userId,leaveMessage,docTableName,docTableId, updateData, updateDataKeys, registId,transformConditionAndExplain,checkResult,businessId,tableNumber,nextControlLinkId,nextControlLink_UserId,nextOutOfControlLinkIds,nextOutOfControlLink_UserIds);
            return Json(sr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BussinessHandler_End(string instanceId, string userId, string leaveMessage, string isEnd,
          string docTableName, string docTableId, string updateData, string updateDataKeys, string registId,
          string transformConditionAndExplain)
        {
            StatusReport sr = ProcessDal.BussinessHandler_End(instanceId,userId,leaveMessage,isEnd,docTableName,docTableId, updateData, updateDataKeys, registId,transformConditionAndExplain);
            return Json(sr, JsonRequestBehavior.AllowGet);
        }
    }
}