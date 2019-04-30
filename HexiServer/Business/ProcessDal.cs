using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
/// <summary>
/// 任务值：
///     0： 传阅
///     4： 知会
///     5： 并行
/// </summary>
namespace HexiServer.Business
{
    public class ProcessDal
    {

        public static StatusReport BusinessTask(string userId)
        {
            
            if (string.IsNullOrEmpty(userId))
            {
                return new StatusReport().SetFail("userId不能为空");
            }
            string sqlString = "[sp_extcall_业务任务]";
            SqlParameter sp = new SqlParameter("@用户ID", SqlDbType.Int);

            //sp.Value = 514;
            sp.Value = Convert.ToInt32(userId);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, sp);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail("未发现需要处理的流程业务");
            }
            List<object> list = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                if (DataTypeHelper.GetIntValue(dr["文档表ID"]) == 0)//在返回的行集中，请删除 文档表ID=0 的记录，这类任务表示正在发起，但还没创建表单记录
                {
                    continue;
                }
                var item = new
                {
                    id = DataTypeHelper.GetIntValue(dr["ID"]),
                    registId = DataTypeHelper.GetIntValue(dr["登记ID"]),
                    businessId = DataTypeHelper.GetIntValue(dr["业务ID"]),
                    linkId = DataTypeHelper.GetIntValue(dr["环节ID"]),
                    tableNubmer = DataTypeHelper.GetStringValue(dr["表单编号"]),
                    taskRole = DataTypeHelper.GetStringValue(dr["任务角色"]),
                    taskExplain = DataTypeHelper.GetStringValue(dr["任务说明"]),
                    //remark = DataTypeHelper.GetStringValue(dr["备注"]),
                    summary = DataTypeHelper.GetStringValue(dr["摘要"]),
                    sender = DataTypeHelper.GetStringValue(dr["送件人"]),
                    lastInstanceDepartment = DataTypeHelper.GetStringValue(dr["上一实例人员部门"]),
                    reachTime = DataTypeHelper.GetDateStringValue(dr["送达时间"]),
                    initiator = DataTypeHelper.GetStringValue(dr["发起人"]),
                    initiateDepartment = DataTypeHelper.GetStringValue(dr["发起人部门"]),
                    initiateSecondDepartment = DataTypeHelper.GetStringValue(dr["发起人二级部门"]),
                    initiateThirdDepartment = DataTypeHelper.GetStringValue(dr["发起人三级部门"]),
                    initiateTime = DataTypeHelper.GetDateStringValue(dr["发起时间"]),
                    businessName = DataTypeHelper.GetStringValue(dr["业务名称"]),
                    docTableName = DataTypeHelper.GetStringValue(dr["文档表名称"]),
                    docTableId = DataTypeHelper.GetIntValue(dr["文档表ID"]),
                    task = DataTypeHelper.GetIntValue(dr["任务"]),
                    remainTime = DataTypeHelper.GetDecimalValue(dr["停留时间"]),
                    senderLeaveMessage = DataTypeHelper.GetStringValue(dr["送件人留言"]),
                    leaveMessage = DataTypeHelper.GetStringValue(dr["留言"]),
                    isRead = DataTypeHelper.GetBooleanValue(dr["阅读"]),
                    lastInstanceId = DataTypeHelper.GetIntValue(dr["上一实例ID"]),
                    usedTime = DataTypeHelper.GetDecimalValue(dr["已历时间"]),
                    objectType = DataTypeHelper.GetStringValue(dr["对象类型"]),
                    transferObjectId = DataTypeHelper.GetIntValue(dr["传递对象ID"]),
                    transferObjectType = DataTypeHelper.GetStringValue(dr["传递对象类型"]),
                    receiveLinkId = DataTypeHelper.GetIntValue(dr["接收环节ID"]),
                    receiveLinkIds = DataTypeHelper.GetStringValue(dr["接收环节IDs"]),
                    allowRemainTime = DataTypeHelper.GetDecimalValue(dr["允许停留时间"]),
                    isImportant = DataTypeHelper.GetBooleanValue(dr["重要"]),
                    workTip = DataTypeHelper.GetStringValue(dr["工作指引"]),
                    fieldSpreadOut = DataTypeHelper.GetBooleanValue(dr["表单字段分组展开"]),
                    transmitConditionAndExplain = DataTypeHelper.GetStringValue(dr["传递条件及说明"]),
                    staffId = DataTypeHelper.GetIntValue(dr["人员ID"]),
                    registerId = DataTypeHelper.GetIntValue(dr["登记人ID"]),
                    linkStaffId = DataTypeHelper.GetIntValue(dr["环节人员ID"]),
                    tableChangeMarkup = DataTypeHelper.GetStringValue(dr["表单变化标识"]),
                    objectExplain = DataTypeHelper.GetStringValue(dr["对象描述"])
                };
                list.Add(item);
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = list.ToArray();
            return sr;
        }

        public static StatusReport BussinessHandler_TableData(string userId, string linkId, string docTableName, string docTableId, string businessId, string transferObjectId, string objectType, string transferObjectType)
        {
            string sqlString = "[sp_extcall_业务任务处理_表单数据]";
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spLinkId = new SqlParameter("@环节ID", SqlDbType.Int);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);
            SqlParameter spDocTableId = new SqlParameter("@文档表ID", SqlDbType.Int);

            //spUserId.Value = 514;
            //spLinkId.Value = 132;
            //spDocTableName.Value = "业务文档_请款单流程表";
            //spDocTableId.Value = 619;
            spUserId.Value = Convert.ToInt32(userId);
            spLinkId.Value = Convert.ToInt32(linkId);
            spDocTableName.Value = docTableName;
            spDocTableId.Value = Convert.ToInt32(docTableId);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spUserId,spLinkId,spDocTableName,spDocTableId);
            DataTable dtItem = ds.Tables[0];
            DataTable dtDefine = ds.Tables[1];
            List<object> itemList = new List<object>();
            foreach(DataRow dr in dtItem.Rows)
            {
                List<object> columnList = new List<object>();
                foreach (DataColumn dc in dtItem.Columns)
                {
                    string columnName = Convert.ToString(dc.ColumnName);
                    object content = null;
                    if (columnName.Substring(columnName.Length - 2) == "时间" || columnName.Substring(columnName.Length - 2) == "日期")
                    {
                        content = DataTypeHelper.GetDateStringValue(dr[columnName]);
                    }
                    //else if (columnName.Contains("附件"))
                    //{
                    //    string columnValue = DataTypeHelper.GetStringValue(dr[columnName]);//获取对应列的值
                    //    if (columnValue.Contains("-") && columnValue.Contains("|"))//如果值中存在-和|，执行以下操作
                    //    {
                    //        string docValue = columnValue.Split('-')[columnValue.Split('-').Length - 1];//获取最后一个 “-” 后的所有字符
                    //        string[] splitArr = docValue.Split('|');
                    //        string fileName = splitArr[splitArr.Length - 1];
                    //        string recordId = splitArr[splitArr.Length - 2];
                    //        content = docTableName + "\\" + recordId + "\\" + fileName;
                    //    }
                    //    else
                    //    {
                    //        content = "";
                    //    }
                    //}
                    else
                    {
                        content = dr[columnName];
                    }
                    var item = new { title = columnName, content = content };
                    columnList.Add(item);
                }
                itemList.Add(columnList.ToArray());
            }
            List<object> defineList = new List<object>();
            foreach(DataRow dr in dtDefine.Rows)
            {
                //List<object> columnList = new List<object>();
                //foreach(DataColumn dc in dtDefine.Columns)
                //{
                //    string columnName = Convert.ToString(dc.ColumnName);
                //    var item = new { title = columnName, content = dr[columnName] };
                //    columnList.Add(item);
                //}
                //defineList.Add(columnList.ToArray());
                var item = new
                {
                    fieldName = DataTypeHelper.GetStringValue(dr["字段名称"]),
                    fieldSourceName = DataTypeHelper.GetStringValue(dr["字段源名称"]),
                    required = DataTypeHelper.GetBooleanValue(dr["必填列"]),
                    defaultValue = DataTypeHelper.GetStringValue(dr["默认写入值"]),
                    allowEdit = DataTypeHelper.GetBooleanValue(dr["允许编辑"]),
                    isFile = DataTypeHelper.GetBooleanValue(dr["是否文件字段"])
                };
                defineList.Add(item);
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            if (transferObjectType == "Decision")
            {
                sr.data = new { items = itemList.ToArray(), defines = defineList.ToArray(), checkDataDefines = BussinessHandler_CheckData(Convert.ToInt32(businessId), Convert.ToInt32(transferObjectId), docTableName,Convert.ToInt32(docTableId)) };
            }
            if (objectType == "Terminator")
            {
                sr.data = new { items = itemList.ToArray(), defines = defineList.ToArray() };
            }
            //var data = new { items = itemList.ToArray(), defines = defineList.ToArray(), checkDataDefines = BussinessHandler_CheckData(Convert.ToInt32(businessId),Convert.ToInt32(transferObjectId), docTableName) };
            //StatusReport sr = new StatusReport();
            //sr.status = "Success";
            //sr.result = "Success";
            //sr.data = data;
            return sr;
        }
        

        public static StatusReport BussinessHandler_NextLink(string registId, string transferObjectId, string receiveLinkId, string receiveLinkIds, string userId, string businessId, string linkId, string task, string staffId, string lastInstanceId, string department, string secondDepartment, string thirdDepartment, string registerId)
        {
            string sqlString = "[sp_extcall_业务任务处理_下一环节数据]";
            SqlParameter spRegistId = new SqlParameter("@登记ID", SqlDbType.Int);
            SqlParameter spTransferObjectId = new SqlParameter("@传递对象ID", SqlDbType.Int);
            SqlParameter spReceiveLinkId = new SqlParameter("@接收环节ID", SqlDbType.Int);
            SqlParameter spReceiveLinkIds = new SqlParameter("@接收环节IDs", SqlDbType.NVarChar);

            //spRegistId.Value = 868;
            //spTransferObjectId.Value = 28;
            //spReceiveLinkId.Value = 196;
            //spReceiveLinkIds.Value = DBNull.Value;
            spRegistId.Value = registId;
            spTransferObjectId.Value = transferObjectId;
            spReceiveLinkId.Value = DataTypeHelper.GetDBValue(receiveLinkId);
            spReceiveLinkIds.Value = DataTypeHelper.GetDBValue(receiveLinkIds);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString,  spRegistId, spTransferObjectId, spReceiveLinkId, spReceiveLinkIds);
            DataTable dtallowReceiveLinks = ds.Tables[0];
            List<object> allowReceiveLinkList = new List<object>();
            List<object> outOfControlLinkList = new List<object>();
            foreach (DataRow dr in dtallowReceiveLinks.Rows)
            {
                int? theTask = DataTypeHelper.GetIntValue(dr["任务"]);
                if (theTask.HasValue && (theTask == 4 || theTask == 5))
                {
                    var item = new
                    {
                        linkId = DataTypeHelper.GetIntValue(dr["环节ID"]),
                        task = theTask,
                        staffId = DataTypeHelper.GetIntValue(dr["人员ID"]),
                        operationStaff = DataTypeHelper.GetStringValue(dr["操作人员"]),
                        department = DataTypeHelper.GetStringValue(dr["部门"]),
                        selectExplain = DataTypeHelper.GetStringValue(dr["选择说明"]),
                        //selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]),
                        assignOperator = DataTypeHelper.GetIntValue(dr["指定具体接收人"]),
                        objectExplain = DataTypeHelper.GetStringValue(dr["对象描述"]),
                        operationStaffLimit = DataTypeHelper.GetStringValue(dr["操作人员限制"]),
                        selectedRoleMemberId = DataTypeHelper.GetIntValue(dr["指定角色成员ID"]),
                        staffHandling = DataTypeHelper.GetStringValue(dr["经手人信息"]),
                        staffHandled = DataTypeHelper.GetStringValue(dr["曾经经手"]),
                        userChecked = true
                    };
                    outOfControlLinkList.Add(item);
                }
                else
                {
                    var item = new
                    {
                        linkId = DataTypeHelper.GetIntValue(dr["环节ID"]),
                        task = theTask,
                        staffId = DataTypeHelper.GetIntValue(dr["人员ID"]),
                        operationStaff = DataTypeHelper.GetStringValue(dr["操作人员"]),
                        department = DataTypeHelper.GetStringValue(dr["部门"]),
                        selectExplain = DataTypeHelper.GetStringValue(dr["选择说明"]),
                        //selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]),
                        assignOperator = DataTypeHelper.GetIntValue(dr["指定具体接收人"]),
                        objectExplain = DataTypeHelper.GetStringValue(dr["对象描述"]),
                        operationStaffLimit = DataTypeHelper.GetStringValue(dr["操作人员限制"]),
                        selectedRoleMemberId = DataTypeHelper.GetIntValue(dr["指定角色成员ID"]),
                        staffHandling = DataTypeHelper.GetStringValue(dr["经手人信息"]),
                        staffHandled = DataTypeHelper.GetStringValue(dr["曾经经手"])
                    };
                    allowReceiveLinkList.Add(item);
                }

            }
            var data = new { allowReceiveLinks = allowReceiveLinkList.ToArray(), outOfControlLinks = outOfControlLinkList.ToArray(),
                allowReceiveStaffs = BussinessHandler_NextLink_ReceiveStaffs_Private(userId, registId, businessId, DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["环节ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["任务"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["人员ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["操作人员限制"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["指定角色成员ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["经手人信息"]), lastInstanceId, department, secondDepartment, thirdDepartment, registerId) };
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = data;
            return sr;
        }

        public static StatusReport BussinessHandler_NextLink_ReceiveStaffs(string userId, string registId, string businessId, string linkId, string task, string staffId, string operatorLimit, string roleMemberId, string handlerInfo, string lastInstanceId, string department, string secondDepartment, string thirdDepartment, string registerId)
        {
            var data = new { allowReceiveStaffs = BussinessHandler_NextLink_ReceiveStaffs_Private(userId,registId,businessId,linkId,task,staffId,operatorLimit,roleMemberId,handlerInfo,lastInstanceId,department,secondDepartment,thirdDepartment,registerId) };
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = data;
            return sr;
        }


        public static StatusReport BussinessHandler_Save_Notify(string instanceId, string userId, string leaveMessage, string archiving,
           string docTableName, string docTableId, string tableUpdateData, string updateKeys)
        {
            string updateString = GetUpdateString(tableUpdateData, updateKeys);
            //return null;
            string sqlString = "[sp_extcall_业务处理保存_知会]";
            SqlParameter spInstanceId = new SqlParameter("@实例ID", SqlDbType.Int);
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spLeaveMessage = new SqlParameter("@留言", SqlDbType.NVarChar);
            SqlParameter spArchiving = new SqlParameter("@是否归档", SqlDbType.Bit);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);
            SqlParameter spDocTableId = new SqlParameter("@文档表ID", SqlDbType.Int);
            SqlParameter spTableUpdateData = new SqlParameter("@表单更新数据", SqlDbType.NVarChar);

            spInstanceId.Value = Convert.ToInt32(instanceId);
            spUserId.Value = Convert.ToInt32(userId);
            spLeaveMessage.Value = DataTypeHelper.GetDBValue(leaveMessage);
            spArchiving.Value = Convert.ToByte(archiving);
            spDocTableName.Value = DataTypeHelper.GetDBValue(docTableName);
            spDocTableId.Value = Convert.ToInt32(docTableId);
            spTableUpdateData.Value = DataTypeHelper.GetDBValue(updateString);

            StatusReport sr = new StatusReport();
            try
            {
                DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spInstanceId, spUserId, spLeaveMessage, spArchiving, spDocTableName,
                spDocTableId, spTableUpdateData);
                sr.status = "Success";
                sr.result = "成功";
                return sr;
            }
            catch (Exception e)
            {
                sr.status = "Fail";
                sr.result = "知会操作失败：" + e.Message;
                return sr;
            }
        }


        public static StatusReport BussinessHandler_Save_Concurrent(string instanceId, string userId, string leaveMessage, string isDone,
          string docTableName, string docTableId, string tableUpdateData, string updateKeys, string registId,
          string transformConditionAndExplain, string transformStaffId)
        {
            string updateString = GetUpdateString(tableUpdateData, updateKeys);
            //return null;
            string sqlString = "[sp_extcall_业务处理保存_并行]";
            SqlParameter spInstanceId = new SqlParameter("@实例ID", SqlDbType.Int);
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spLeaveMessage = new SqlParameter("@留言", SqlDbType.NVarChar);
            SqlParameter spIsDone = new SqlParameter("@是否归档", SqlDbType.Bit);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);
            SqlParameter spDocTableId = new SqlParameter("@文档表ID", SqlDbType.Int);
            SqlParameter spTableUpdateData = new SqlParameter("@表单更新数据", SqlDbType.NVarChar);
            SqlParameter spTransformConditionAndExplain = new SqlParameter("@传递条件及说明", SqlDbType.NVarChar);
            SqlParameter spTransformStaffId = new SqlParameter("@转移人ID", SqlDbType.Int);

            spInstanceId.Value = Convert.ToInt32(instanceId);
            spUserId.Value = Convert.ToInt32(userId);
            spLeaveMessage.Value = DataTypeHelper.GetDBValue(leaveMessage);
            spIsDone.Value = Convert.ToByte(isDone);
            spDocTableName.Value = DataTypeHelper.GetDBValue(docTableName);
            spDocTableId.Value = Convert.ToInt32(docTableId);
            spTableUpdateData.Value = DataTypeHelper.GetDBValue(updateString);
            spTransformConditionAndExplain.Value = DataTypeHelper.GetDBValue(transformConditionAndExplain);
            spTransformStaffId.Value = Convert.ToInt32(transformStaffId);
            StatusReport sr = new StatusReport();
            try
            {
                DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spInstanceId, spUserId, spLeaveMessage, spIsDone, spDocTableName,
                spDocTableId, spTableUpdateData, spTransformConditionAndExplain,spTransformStaffId);
                sr.status = "Success";
                sr.result = "成功";
                return sr;
            }
            catch (Exception e)
            {
                sr.status = "Fail";
                sr.result = "并行操作失败：" + e.Message;
                return sr;
            }
        }



        public static StatusReport BussinessHandler_Save(string instanceId, string userId, string leaveMessage,
            string docTableName, string docTableId, string tableUpdateData, string updateKeys, string registId,
            string transformConditionAndExplain, string checkResult, string businessId, string tableNumber, string nextControlLinkId,
            string nextControlLink_UserId, string nextOutOfControlLinkIds, string nextOutOfControlLink_UserIds)
        {
            string updateString = GetUpdateString(tableUpdateData, updateKeys);
            //return null;
            string sqlString = "[sp_extcall_业务处理保存]";
            SqlParameter spInstanceId = new SqlParameter("@实例ID", SqlDbType.Int);
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spLeaveMessage = new SqlParameter("@留言", SqlDbType.NVarChar);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);
            SqlParameter spDocTableId = new SqlParameter("@文档表ID", SqlDbType.Int);
            SqlParameter spTableUpdateData = new SqlParameter("@表单更新数据", SqlDbType.NVarChar);
            SqlParameter spRegistId = new SqlParameter("@登记ID", SqlDbType.Int);
            SqlParameter spTransformConditionAndExplain = new SqlParameter("@传递条件及说明", SqlDbType.NVarChar);
            SqlParameter spCheckResult = new SqlParameter("@审核结果", SqlDbType.NVarChar);
            SqlParameter spBusinessId = new SqlParameter("@业务ID", SqlDbType.Int);
            SqlParameter spTableNumber = new SqlParameter("@表单编号", SqlDbType.NVarChar);
            SqlParameter spNextControlLinkId = new SqlParameter("@下一控制环节ID", SqlDbType.Int);
            SqlParameter spNextControlLink_UserId = new SqlParameter("@下一控制环节_用户ID", SqlDbType.Int);
            SqlParameter spNextOutOfControlLinkIds = new SqlParameter("@下一非控制环节IDs", SqlDbType.NVarChar);
            SqlParameter spNextOutOfControlLink_UserIds = new SqlParameter("@下一非控制环节_用户IDs", SqlDbType.NVarChar);

            spInstanceId.Value = Convert.ToInt32(instanceId);
            spUserId.Value = Convert.ToInt32(userId);
            spLeaveMessage.Value = DataTypeHelper.GetDBValue(leaveMessage);
            spDocTableName.Value = DataTypeHelper.GetDBValue(docTableName);
            spDocTableId.Value = Convert.ToInt32(docTableId);
            spTableUpdateData.Value = DataTypeHelper.GetDBValue(updateString);
            spRegistId.Value = Convert.ToInt32(registId);
            spTransformConditionAndExplain.Value = DataTypeHelper.GetDBValue(transformConditionAndExplain);
            spCheckResult.Value = DataTypeHelper.GetDBValue(checkResult);
            spBusinessId.Value = Convert.ToInt32(businessId);
            spTableNumber.Value = DataTypeHelper.GetDBValue(tableNumber);
            //spNextControlLinkId.Value = DBNull.Value;
            //spNextOutOfControlLinkIds.Value = DBNull.Value;
            //spNextOutOfControlLink_UserIds.Value = DBNull.Value;
            spNextControlLinkId.Value = Convert.ToInt32(nextControlLinkId);
            //spNextControlLink_UserId.Value = DataTypeHelper.GetDBValue(nextControlLink_UserId);
            spNextOutOfControlLinkIds.Value = DataTypeHelper.GetDBValue(nextOutOfControlLinkIds);
            spNextOutOfControlLink_UserIds.Value = DataTypeHelper.GetAllowEmptyDBValue(nextOutOfControlLink_UserIds);
            if (Convert.ToInt32(nextControlLink_UserId) == 0)
            {
                spNextControlLink_UserId.Value = DBNull.Value;
            }
            else
            {
                spNextControlLink_UserId.Value = Convert.ToInt32(nextControlLink_UserId);
            }
            //return null;
            StatusReport sr = new StatusReport();
            try
            {
                DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spInstanceId, spUserId, spLeaveMessage, spDocTableName,
                spDocTableId, spTableUpdateData, spRegistId, spTransformConditionAndExplain, spCheckResult, spBusinessId, spTableNumber,
                spNextControlLinkId, spNextControlLink_UserId, spNextOutOfControlLinkIds, spNextOutOfControlLink_UserIds);
                sr.status = "Success";
                sr.result = "成功";
                return sr;
            }
            catch (Exception e)
            {
                sr.status = "Fail";
                sr.result = "传递到下一步失败：" + e.Message;
                return sr;
            }



            //DataTable dtallowReceiveLinks = ds.Tables[0];
            //List<object> allowReceiveLinkList = new List<object>();
            //foreach (DataRow dr in dtallowReceiveLinks.Rows)
            //{
            //    var item = new
            //    {
            //        linkId = DataTypeHelper.GetIntValue(dr["环节ID"]),
            //        task = DataTypeHelper.GetIntValue(dr["任务"]),
            //        staffId = DataTypeHelper.GetIntValue(dr["人员ID"]),
            //        operationStaff = DataTypeHelper.GetStringValue(dr["操作人员"]),
            //        department = DataTypeHelper.GetStringValue(dr["部门"]),
            //        selectExplain = DataTypeHelper.GetStringValue(dr["选择说明"]),
            //        selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]),
            //        assignOperator = DataTypeHelper.GetIntValue(dr["指定具体接收人"]),
            //        objectExplain = DataTypeHelper.GetStringValue(dr["对象描述"]),
            //        operationStaffLimit = DataTypeHelper.GetStringValue(dr["操作人员限制"]),
            //        selectedRoleMemberId = DataTypeHelper.GetIntValue(dr["指定角色成员ID"]),
            //        staffHandling = DataTypeHelper.GetStringValue(dr["经手人信息"]),
            //        staffHandled = DataTypeHelper.GetStringValue(dr["曾经经手"])
            //    };
            //    allowReceiveLinkList.Add(item);
            //}
            //var data = new
            //{
            //    //allowReceiveLinks = allowReceiveLinkList.ToArray(),
            //    //allowReceiveStaffs = BussinessHandler_NextLink_ReceiveStaffs_Private(userId, registId, businessId, DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["环节ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["任务"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["人员ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["操作人员限制"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["指定角色成员ID"]), DataTypeHelper.GetStringValue(dtallowReceiveLinks.Rows[0]["经手人信息"]), lastInstanceId, department, secondDepartment, thirdDepartment, registerId)
            //};

        }


        public static StatusReport BussinessHandler_End(string instanceId, string userId, string leaveMessage, string isEnd,
           string docTableName, string docTableId, string tableUpdateData, string updateDataKeys, string registId,
           string transformConditionAndExplain)
        {
            string updateString = GetUpdateString(tableUpdateData, updateDataKeys);
            //return null;
            string sqlString = "[sp_extcall_业务处理保存_业务结束]";
            SqlParameter spInstanceId = new SqlParameter("@实例ID", SqlDbType.Int);
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spLeaveMessage = new SqlParameter("@留言", SqlDbType.NVarChar);
            SqlParameter spIsEnd = new SqlParameter("@是否结束", SqlDbType.Bit);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);
            SqlParameter spDocTableId = new SqlParameter("@文档表ID", SqlDbType.Int);
            SqlParameter spTableUpdateData = new SqlParameter("@表单更新数据", SqlDbType.NVarChar);
            SqlParameter spRegistId = new SqlParameter("@登记ID", SqlDbType.Int);
            SqlParameter spTransformConditionAndExplain = new SqlParameter("@传递条件及说明", SqlDbType.NVarChar);

            spInstanceId.Value = Convert.ToInt32(instanceId);
            spUserId.Value = Convert.ToInt32(userId);
            spLeaveMessage.Value = DataTypeHelper.GetDBValue(leaveMessage);
            spIsEnd.Value = Convert.ToInt32(isEnd);
            spDocTableName.Value = DataTypeHelper.GetDBValue(docTableName);
            spDocTableId.Value = Convert.ToInt32(docTableId);
            spTableUpdateData.Value = DataTypeHelper.GetDBValue(updateString);
            spRegistId.Value = Convert.ToInt32(registId);
            spTransformConditionAndExplain.Value = DataTypeHelper.GetDBValue(transformConditionAndExplain);

            
            StatusReport sr = new StatusReport();
            try
            {
                DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spInstanceId, spUserId, spLeaveMessage, spIsEnd, spDocTableName,
                spDocTableId, spTableUpdateData, spRegistId, spTransformConditionAndExplain);
                sr.status = "Success";
                sr.result = "成功";
                return sr;
            }
            catch (Exception e)
            {
                sr.status = "Fail";
                sr.result = "传递到下一步失败：" + e.Message;
                return sr;
            }
        }



        private static object[] BussinessHandler_CheckData(int businessId, int transferObjectId, string docTableName, int docTableId)
        {
            string sqlString = "[sp_extcall_业务任务处理_审核数据]";
            SqlParameter spBusinessId = new SqlParameter("@业务ID", SqlDbType.Int);
            SqlParameter spTransferObjectId = new SqlParameter("@传递对象ID", SqlDbType.Int);
            SqlParameter spDocTableName = new SqlParameter("@文档表名称", SqlDbType.NVarChar);

            //spBusinessId.Value = 12;
            //spTransferObjectId.Value = 28;
            //spDocTableName.Value = "业务文档_请款单流程表";
            spBusinessId.Value = businessId;
            spTransferObjectId.Value = transferObjectId;
            spDocTableName.Value = docTableName;

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spBusinessId, spTransferObjectId, spDocTableName);
            DataTable dtDefine = ds.Tables[0];
            List<object> defineList = new List<object>();
            foreach (DataRow dr in dtDefine.Rows)
            {
                string hiddenConditon = DataTypeHelper.GetStringValue(dr["隐藏条件"]);
                if (!string.IsNullOrEmpty(hiddenConditon))
                {
                    string hiddenSqlString = "select ID from " + docTableName + " where ID = " + docTableId + " and (" + hiddenConditon + ")";//根据隐藏条件查询
                    int result = SQLHelper.ExecuteScalar("wyt", hiddenSqlString);
                    if (result > 0)
                    {
                        continue;
                    }
                }
                string selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]);
                bool isSelect = false;
                if (!string.IsNullOrEmpty(selectCondition))
                {
                    string selectSqlString = "select ID from " + docTableName + " where ID = " + docTableId + " and (" + selectCondition + ")";//根据选择条件查询
                    int result = SQLHelper.ExecuteScalar("wyt", selectSqlString);
                    if (result > 0)
                    {
                        isSelect = true;
                    }
                }
                var item = new
                {
                    id = DataTypeHelper.GetIntValue(dr["ID"]),
                    taskExplain = DataTypeHelper.GetStringValue(dr["任务说明"]),
                    checkResult = DataTypeHelper.GetStringValue(dr["审核结果"]),
                    transferObjectId = DataTypeHelper.GetIntValue(dr["传递对象ID"]),
                    receiveLinkId = DataTypeHelper.GetIntValue(dr["接收环节ID"]),
                    receiveLinkIds = DataTypeHelper.GetStringValue(dr["接收环节IDs"]),
                    hiddenCondition = DataTypeHelper.GetStringValue(dr["隐藏条件"]),
                    selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]),
                    isReadOnly = DataTypeHelper.GetBooleanValue(dr["只读性选择"]),
                    transConditionAndExplain = DataTypeHelper.GetStringValue(dr["传递条件及说明"]),
                    isSelect = isSelect
                };
                defineList.Add(item);
            }
            //var data = new { checkDataDefines = defineList.ToArray() };
            //StatusReport sr = new StatusReport();
            //sr.status = "Success";
            //sr.result = "Success";
            //sr.data = data;
            //return sr;
            return defineList.ToArray();
        }

        /// <summary>
        /// 判断该结果是否可选
        /// </summary>
        //public static object[] CheckSelectCondition(string docTableName, int docTableId, string selectCondition)
        //{
        //    bool isSelect = false;
        //    if (!string.IsNullOrEmpty(selectCondition))
        //    {
        //        string selectSqlString = "select ID from " + docTableName + " where ID = " + docTableId + "(" + selectCondition + ")";//根据选择条件查询
        //        int result = SQLHelper.ExecuteScalar("wyt", selectSqlString);
        //        if (result > 0)
        //        {
        //            isSelect = true;
        //        }
        //    }
        //}


        private static object[] BussinessHandler_NextLink_ReceiveStaffs_Private(string userId, string registId, string businessId, string linkId, string task, string staffId, string operatorLimit, string roleMemberId, string handlerInfo, string lastInstanceId, string department, string secondDepartment, string thirdDepartment, string registerId)
        {
            string sqlString = "[sp_extcall_业务任务处理_下一环节数据_接收人]";
            SqlParameter spUserId = new SqlParameter("@用户ID", SqlDbType.Int);
            SqlParameter spRegistId = new SqlParameter("@登记ID", SqlDbType.Int);
            SqlParameter spBusinessId = new SqlParameter("@业务ID", SqlDbType.Int);
            SqlParameter spLinkId = new SqlParameter("@环节ID", SqlDbType.Int);
            SqlParameter spTask = new SqlParameter("@任务", SqlDbType.Int);
            SqlParameter spStaffId = new SqlParameter("@人员ID", SqlDbType.Int);
            SqlParameter spOperatorLimit = new SqlParameter("@操作人员限制", SqlDbType.NVarChar);
            SqlParameter spRoleMemberId = new SqlParameter("@指定角色成员ID", SqlDbType.NVarChar);
            SqlParameter spHandlerInfo = new SqlParameter("@经手人信息", SqlDbType.NVarChar);
            SqlParameter spLastInstanceId = new SqlParameter("@上一实例ID", SqlDbType.Int);
            SqlParameter spDepartment = new SqlParameter("@发起人部门", SqlDbType.NVarChar);
            SqlParameter spSecondDepartment = new SqlParameter("@发起人二级部门", SqlDbType.NVarChar);
            SqlParameter spThirdDepartment = new SqlParameter("@发起人三级部门", SqlDbType.NVarChar);
            SqlParameter spRegisterId = new SqlParameter("@登记人ID", SqlDbType.Int);

            //spUserId.Value = 514;
            //spRegistId.Value = 875;
            //spBusinessId.Value = 12;
            //spLinkId.Value = 196;
            //spTask.Value = 2;
            //spStaffId.Value = 509;
            //spOperatorLimit.Value = DBNull.Value;
            //spRoleMemberId.Value = DBNull.Value;
            //spHandlerInfo.Value = DBNull.Value;
            //spLastInstanceId.Value = 3973;
            //spDepartment.Value = "玖珑花园";
            //spSecondDepartment.Value = "管理处主任";
            //spThirdDepartment.Value = DBNull.Value;
            //spRegisterId.Value = 293;
            spUserId.Value = Convert.ToInt32(userId);
            spRegistId.Value = Convert.ToInt32(registId);
            spBusinessId.Value = Convert.ToInt32(businessId);
            spLinkId.Value = Convert.ToInt32(linkId);
            spTask.Value = Convert.ToInt32(task);
            spStaffId.Value = Convert.ToInt32(staffId);
            spOperatorLimit.Value = DataTypeHelper.GetDBValue(operatorLimit);
            spRoleMemberId.Value = DataTypeHelper.GetDBValue(roleMemberId);
            spHandlerInfo.Value = DataTypeHelper.GetDBValue(handlerInfo);
            spLastInstanceId.Value = Convert.ToInt32(lastInstanceId);
            spDepartment.Value = DataTypeHelper.GetDBValue(department);
            spSecondDepartment.Value = DataTypeHelper.GetDBValue(secondDepartment);
            spThirdDepartment.Value = DataTypeHelper.GetDBValue(thirdDepartment);
            spRegisterId.Value = Convert.ToInt32(registerId);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spUserId, spRegistId, spBusinessId, spLinkId, spTask, spStaffId, spOperatorLimit, spRoleMemberId, spHandlerInfo, spLastInstanceId, spDepartment, spSecondDepartment, spThirdDepartment, spRegisterId);
            DataTable dtallowReceiveStaffs = ds.Tables[0];
            //DataTable dtDefine = ds.Tables[1];
            List<object> allowReceiveStaffList = new List<object>();
            var firstItem = new
            {
                userId = 0,
                user = "不选择",
                userDepartment = "不选择",
                userChecked = false
            };
            allowReceiveStaffList.Add(firstItem);
            foreach (DataRow dr in dtallowReceiveStaffs.Rows)
            {
                var item = new
                {
                    userId = DataTypeHelper.GetIntValue(dr["用户ID"]),
                    user = DataTypeHelper.GetStringValue(dr["用户"]),
                    userDepartment = DataTypeHelper.GetStringValue(dr["用户部门"]),
                    userChecked = false
                };
                allowReceiveStaffList.Add(item);
            }
            return allowReceiveStaffList.ToArray();
            //var data = new { allowReceiveStaffs = allowReceiveStaffList.ToArray() };
            //StatusReport sr = new StatusReport();
            //sr.status = "Success";
            //sr.result = "Success";
            //sr.data = data;
            //return sr;
        }



        private static object[] BussinessHandler_RoleAndUserData(int linkUserId)
        {
            string sqlString = "[sp_extcall_业务任务处理_角色用户数据]";
            SqlParameter spLinkUserId = new SqlParameter("@业务ID", SqlDbType.Int);

            //spBusinessId.Value = 12;
            //spTransferObjectId.Value = 28;
            //spDocTableName.Value = "业务文档_请款单流程表";
            spLinkUserId.Value = linkUserId;
            //spTransferObjectId.Value = transferObjectId;
            //spDocTableName.Value = docTableName;

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spLinkUserId);
            DataTable dtDefine = ds.Tables[0];
            List<object> defineList = new List<object>();
            foreach (DataRow dr in dtDefine.Rows)
            {
                var item = new
                {
                    id = DataTypeHelper.GetIntValue(dr["ID"]),
                    taskExplain = DataTypeHelper.GetStringValue(dr["任务说明"]),
                    checkResult = DataTypeHelper.GetStringValue(dr["审核结果"]),
                    transferObjectId = DataTypeHelper.GetIntValue(dr["传递对象ID"]),
                    receiveLinkId = DataTypeHelper.GetIntValue(dr["接收环节ID"]),
                    receiveLinkIds = DataTypeHelper.GetStringValue(dr["接收环节IDs"]),
                    //hiddenCondition = DataTypeHelper.GetStringValue(dr["隐藏条件"]),
                    //selectCondition = DataTypeHelper.GetStringValue(dr["选择条件"]),
                    isReadOnly = DataTypeHelper.GetBooleanValue(dr["只读性选择"]),
                    //transConditionAndExplain = DataTypeHelper.GetStringValue(dr["传递条件及说明"])
                };
                defineList.Add(item);
            }
            //var data = new { checkDataDefines = defineList.ToArray() };
            //StatusReport sr = new StatusReport();
            //sr.status = "Success";
            //sr.result = "Success";
            //sr.data = data;
            //return sr;
            return defineList.ToArray();
        }


        private static string GetUpdateString(string tableUpdateData, string updateDataKeys)
        {
            if (string.IsNullOrEmpty(updateDataKeys))
            {
                return null;
            }
            JArray jArray = (JArray)JsonConvert.DeserializeObject(updateDataKeys);
            if (jArray.Count == 0)
            {
                return null;
            }
            JObject jObject = (JObject)JsonConvert.DeserializeObject(tableUpdateData);
            string updateString = "";
            for(int i = 0; i < jArray.Count; i++)
            {
                string key = Convert.ToString(jArray[i]);
                string value = Convert.ToString(jObject[key]);
                updateString += "" + key + "=" + "\'" + value + "\'" + "※";
            }
            updateString = updateString.Substring(0, updateString.Length - 1);
            //updateString = "N\'" + updateString + "\'";
            return updateString;
        }

    }
}



//public static StatusReport GetProcessList(string userId)
//{
//    if (string.IsNullOrEmpty(userId))
//    {
//        return null;
//    }
//    string sqlString = "[sp_extcall_业务任务]";
//    SqlParameter sp = new SqlParameter("@用户ID", SqlDbType.Int);
//    sp.Value = 514;
//    DataTable dt = SQLHelper.ExecuteProcedure("wyt", sqlString, sp);
//    //dt.
//    if (dt.Rows.Count == 0)
//    {
//        return null;
//    }
//    ////ArrayList al = dt.Rows
//    List<object> list = new List<object>();
//    //Type t = null;
//    //dt.Columns.Count;



//    foreach (DataRow dr in dt.Rows)
//    {
//        List<object> columnList = new List<object>();
//        foreach (DataColumn dc in dt.Columns)
//        {
//            Dictionary<string, object> dic = new Dictionary<string, object>();
//            string columnName = Convert.ToString(dc.ColumnName);
//            var item = new {title = columnName, content = dr[columnName]};
//            //dic.Add(columnName, dr[columnName]);
//            columnList.Add(item);
//        }
//        list.Add(columnList.ToArray());
//        //t = dr[0].GetType();
//        //object[] arr = dr.ItemArray;
//        ////foreach (DataColumnCollection dcc in dt.Columns)
//        ////{
//        ////    //ArrayList al = dcc.List;
//        ////}
//        //list.Add(arr);
//    }
//    StatusReport sr = new StatusReport();
//    sr.status = "Success";
//    sr.result = "Success";
//    sr.data = list.ToArray();
//    //sr.data = t.;
//    return sr;
//}