using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using HexiServer.Models;
using static HexiUtils.DataTypeHelper;

namespace HexiServer.Business
{
    public class RequestPaymentFlowDal
    {
        public static StatusReport GetRequestPaymentSheet(string userId)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " SELECT ID,登记ID,业务ID,环节ID,文档表ID,人员ID,上一实例ID,创建日期, " +
                " 结束日期,留言,阅读,文档编号,表单变化标识,任务,任务说明,登记说明, " +
                " 登记日期,登记人,登记人部门,状态标记,请款单位,请款人 ,请款日期,收款单位 ,合同编号 ,合同金额, " +
                " 用途 ,支付方式,合同ID,支出性质 ,费用种类,预算年度 ,预算审核员,预算审核员审批时间,预算审核员审批意见, " +
                " 分公司分管财务,分公司分管财务审批时间,分公司分管财务审批意见,分公司运行部,分公司运行部审批时间, " +
                " 分公司运行部审批意见,分公司行政部,分公司行政部审批时间,分公司行政部审批意见, " +
                " 分公司审批,分公司审批时间,分公司审批意见,分公司领导, " +
                " 分公司领导审批时间,分公司领导审批意见, " +
                " 是否采购计划内 ,采购编号, " +
                " 预算处,请款编号,请款月份,预算类别 " +
                " FROM dbo.小程序_请款流程 " +
                " where 结束日期 is null and 人员ID = @人员ID " +
                " order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@人员ID", userId));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }

            List<RequestPaymentSheet> rpsList = new List<RequestPaymentSheet>();
            foreach (DataRow dr in dt.Rows)
            {
                RequestPaymentSheet rps = new RequestPaymentSheet
                {
                    id = GetIntValue(dr["ID"]),
                    registerId = GetIntValue(dr["登记ID"]),
                    businessId = GetIntValue(dr["业务ID"]),
                    linkId = GetIntValue(dr["环节ID"]),
                    documentId = GetIntValue(dr["文档表ID"]),
                    userId = GetIntValue(dr["人员ID"]),
                    lastId = GetIntValue(dr["上一实例ID"]),
                    createTime = GetDateStringValue(dr["创建日期"]),
                    endTime = GetDateStringValue(dr["结束日期"]),
                    leaveWords = GetStringValue(dr["留言"]),
                    isRead = GetIntValue(dr["阅读"]),
                    documentNumber = GetStringValue(dr["文档编号"]),
                    task = GetStringValue(dr["任务"]),
                    taskExplain = GetStringValue(dr["任务说明"]),
                    registerExplain = GetStringValue(dr["登记说明"]),
                    registerTime = GetDateStringValue(dr["登记日期"]),
                    registerMan = GetStringValue(dr["登记人"]),
                    registerDepartment = GetStringValue(dr["登记人部门"]),
                    requestMan = GetStringValue(dr["请款人"]),
                    requestDepartment = GetStringValue(dr["请款单位"]),
                    requestTime = GetDateStringValue(dr["请款日期"]),
                    receiptDepartment = GetStringValue(dr["收款单位"]),
                    compactNumber = GetStringValue(dr["合同编号"]),
                    compactSum = GetDecimalValue(dr["合同金额"]),
                    purpose = GetStringValue(dr["用途"]),
                    payMethod = GetStringValue(dr["支付方式"]),
                    expenseProperty = GetStringValue(dr["支出性质"]),
                    chargeType = GetStringValue(dr["费用种类"]),
                    budgetYear = GetStringValue(dr["预算年度"]),
                    budgeAssessor = GetStringValue(dr["预算审核员"]),
                    budgeAssessorApprovalTime = GetStringValue(dr["预算审核员审批时间"]),
                    budgeAssessorApprovalOpinion = GetStringValue(dr["预算审核员审批意见"]),
                    controlledCorporationAccountant = GetStringValue(dr["分公司分管财务"]),
                    controlledCorporationAccountantApprovalTime = GetDateStringValue(dr["分公司分管财务审批时间"]),
                    controlledCorporationAccountantApprovalOpinion = GetStringValue(dr["分公司分管财务审批意见"]),
                    controlledCorporationOperationDepartment = GetStringValue(dr["分公司运行部"]),
                    controlledCorporationOperationDepartmentApprovalTime = GetDateStringValue(dr["分公司运行部审批时间"]),
                    controlledCorporationOperationDepartmentApprovalOpinion = GetStringValue(dr["分公司运行部审批意见"]),
                    controlledCorporationAdministrativeDepartment = GetStringValue(dr["分公司行政部"]),
                    controlledCorporationAdministrativeDepartmentApprovalTime = GetDateStringValue(dr["分公司行政部审批时间"]),
                    controlledCorporationAdministrativeDepartmentApprovalOpinion = GetStringValue(dr["分公司行政部审批意见"]),
                    controlledCorporationApproval = GetStringValue(dr["分公司审批"]),
                    controlledCorporationApprovalTime = GetDateStringValue(dr["分公司审批时间"]),
                    controlledCorporationApprovalOpinion = GetStringValue(dr["分公司审批意见"]),
                    controlledCorporationLeader = GetStringValue(dr["分公司领导"]),
                    controlledCorporationLeaderApprovalTime = GetDateStringValue(dr["分公司领导审批时间"]),
                    controlledCorporationLeaderApprovalOpinion = GetStringValue(dr["分公司领导审批意见"]),
                    isInPurchasePlan = GetStringValue(dr["是否采购计划内"]),
                    purchaseNumber = GetStringValue(dr["采购编号"]),
                    budgeBureau = GetStringValue(dr["预算处"]),
                    requestNumber = GetStringValue(dr["请款编号"]),
                    requestMonth = GetStringValue(dr["请款月份"]),
                    budgeType = GetStringValue(dr["预算类别"])
                };
                if (GetIntValue(dr["文档表ID"]).HasValue)
                {
                    rps.itemOfExpenditures = GetItemOfExpenditures(GetIntValue(dr["文档表ID"]).Value);
                    rps.requestSum = GetRequestSum(rps.itemOfExpenditures);
                }
                
                rpsList.Add(rps);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = rpsList.ToArray();
            return sr;
        }


        public static StatusReport SetRequestPaymentFlow (string userName, string result, string leaveWord, string registerId, string linkId, string id, string documentId, string documentNumber, string opinion)
        {
            StatusReport sr = new StatusReport();

            if (userName == "李军")//李军接审
            {
                string updateInstanceSqlString = " update 业务实例 set 留言 = @留言, 结束日期 = @结束日期, 阅读 = 1, 表单变化标识 = 1 " +
                    " where ID = @ID" +
                    " select @@identity ";
                sr = SQLHelper.Update("wyt", updateInstanceSqlString,
                    new SqlParameter("@留言", result),
                    new SqlParameter("@结束日期", DateTime.Now),
                    new SqlParameter("@ID",id));
                if (sr.status == "Fail")
                {
                    return sr;
                }

                string insertInstanceSqlString = "";
                if (result == "同意（运营费<5000）")
                {
                    insertInstanceSqlString = " insert into 业务实例 (登记ID,业务ID,环节ID,人员ID,上一实例ID,创建日期,文档表名称,文档表ID,文档编号) " +
                        " select @登记ID,12,213,513,@上一实例ID,@创建日期,'业务文档_请款单流程表',@文档表ID,@文档编号" +
                        " select @@identity ";
                }
                else if (result == "同意，送郑总审批（运营费>=5000）")
                {
                    insertInstanceSqlString = " insert into 业务实例 (登记ID,业务ID,环节ID,人员ID,上一实例ID,创建日期,文档表名称,文档表ID,文档编号) " +
                        " select @登记ID,12,134,354,@上一实例ID,@创建日期,'业务文档_请款单流程表',@文档表ID,@文档编号" +
                        " select @@identity ";
                }
                else
                {
                    insertInstanceSqlString = " insert into 业务实例 (登记ID,业务ID,环节ID,人员ID,上一实例ID,创建日期,文档表名称,文档表ID,文档编号) " +
                        " select @登记ID,12,149,323,@上一实例ID,@创建日期,'业务文档_请款单流程表',@文档表ID,@文档编号" +
                        " select @@identity ";
                }
                sr = SQLHelper.Insert("wyt", insertInstanceSqlString,
                        new SqlParameter("@登记ID", registerId),
                        new SqlParameter("@上一实例ID", id),
                        new SqlParameter("@创建日期", DateTime.Now),
                        new SqlParameter("@文档表ID", documentId),
                        new SqlParameter("@文档编号", documentNumber));
                if (sr.status == "Fail")
                {
                    return sr;
                }

                string updateFlowSqlString = "update 业务文档_请款单流程表 set 分公司审批 = '李军', 分公司审批时间 = @分公司审批时间,分公司审批意见 = @分公司审批意见 " +
                    " where ID = @ID " +
                    " select @@identity ";
                sr = SQLHelper.Update("wyt", updateFlowSqlString, 
                    new SqlParameter("@分公司审批时间", DateTime.Now),
                    new SqlParameter("@分公司审批意见", opinion),
                    new SqlParameter("@ID", documentId));
                //if (sr.status == "Fail")
                //{
                    return sr;
                //}
            }
            else//郑永军接审
            {
                string updateInstanceSqlString = " update 业务实例 set 留言 = @留言, 结束日期 = @结束日期, 阅读 = 1, 表单变化标识 = 1 " +
                    " where ID = @ID" +
                    " select @@identity ";
                sr = SQLHelper.Update("wyt", updateInstanceSqlString,
                    new SqlParameter("@留言", result),
                    new SqlParameter("@结束日期", DateTime.Now),
                    new SqlParameter("@ID", id));
                if (sr.status == "Fail")
                {
                    return sr;
                }

                string insertInstanceSqlString = "";
                if (result == "同意")
                {
                    insertInstanceSqlString = " insert into 业务实例 (登记ID,业务ID,环节ID,人员ID,上一实例ID,创建日期,文档表名称,文档表ID,文档编号) " +
                        " select @登记ID,12,213,513,@上一实例ID,@创建日期,'业务文档_请款单流程表',@文档表ID,@文档编号" +
                        " select @@identity ";
                }
                else
                {
                    insertInstanceSqlString = " insert into 业务实例 (登记ID,业务ID,环节ID,人员ID,上一实例ID,创建日期,文档表名称,文档表ID,文档编号) " +
                        " select @登记ID,12,149,323,@上一实例ID,@创建日期,'业务文档_请款单流程表',@文档表ID,@文档编号" +
                        " select @@identity ";
                }
                sr = SQLHelper.Insert("wyt", insertInstanceSqlString,
                        new SqlParameter("@登记ID", registerId),
                        new SqlParameter("@上一实例ID", id),
                        new SqlParameter("@创建日期", DateTime.Now),
                        new SqlParameter("@文档表ID", documentId),
                        new SqlParameter("@文档编号", documentNumber));
                if (sr.status == "Fail")
                {
                    return sr;
                }

                string updateFlowSqlString = "update 业务文档_请款单流程表 set 分公司领导 = '郑永军', 分公司领导审批时间 = @分公司领导审批时间,分公司领导审批意见 = @分公司领导审批意见 " +
                    " where ID = @ID " +
                    " select @@identity ";
                sr = SQLHelper.Update("wyt", updateFlowSqlString,
                    new SqlParameter("@分公司领导审批时间", DateTime.Now),
                    new SqlParameter("@分公司领导审批意见", opinion),
                    new SqlParameter("@ID", documentId));
                //if (sr.status == "Fail")
                //{
                return sr;
                //}
            }
        }

        private static ItemOfExpenditure[] GetItemOfExpenditures(int pId)
        {
            List<ItemOfExpenditure> ioeList = new List<ItemOfExpenditure>();
            string sqlString = " SELECT 预算ID, 预算编号, 项目名称, 月累计预算金额, 本次付款前本项月累计预算使用金额, " +
                " 本次付款前本项月累计预算余额, 请款金额, 实付金额, 年度预算金额, 年累计预算余额 FROM " +
                " dbo.业务文档_请款单流程表_支出项目 " +
                " where PID = @PID";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@PID", pId));
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            foreach(DataRow dr in dt.Rows)
            {
                ItemOfExpenditure ioe = new ItemOfExpenditure
                {
                    budgeId = GetIntValue(dr["预算ID"]),
                    budgeNumber = GetStringValue(dr["预算编号"]),
                    itemName = GetStringValue(dr["项目名称"]),
                    monthBudgeSum = GetDecimalValue(dr["月累计预算金额"]),
                    budgeUsableSum = GetDecimalValue(dr["本次付款前本项月累计预算使用金额"]),
                    budgeRemainingSum = GetDecimalValue(dr["本次付款前本项月累计预算余额"]),
                    requestSum = GetDecimalValue(dr["请款金额"]),
                    paySum = GetDecimalValue(dr["实付金额"]),
                    yearBudgeSum = GetDecimalValue(dr["年度预算金额"]),
                    yearRemainingSum = GetDecimalValue(dr["年累计预算余额"])
                };
                ioeList.Add(ioe);
            }

            return ioeList.ToArray();
        }

        private static decimal GetRequestSum(ItemOfExpenditure[] ioes)
        {
            decimal requestSum = 0;
            if (ioes.Length != 0)
            {
                for(int i = 0; i <　ioes.Length; i++)
                {
                    if (ioes[i].requestSum.HasValue)
                    {
                        requestSum += ioes[i].requestSum.Value;
                    }
                }
            }
            return requestSum;
        }
    }
}