using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class RequestPaymentSheet
    {
        public int? id { get; set; }//ID
        public int? registerId { get; set; }//登记ID
        public int? businessId { get; set; }//业务ID
        public int? linkId { get; set; }//环节ID
        public int? documentId { get; set; }//文档表ID
        public int? userId { get; set; }//人员ID
        public int? lastId { get; set; }//上一实例ID
        public string createTime { get; set; }//创建日期
        public string endTime { get; set; }//结束日期
        public string leaveWords { get; set; }//留言
        public int? isRead { get; set; }//阅读
        public string documentNumber { get; set; }//文档编号
        public string task { get; set; }//任务
        public string taskExplain { get; set; }//任务说明
        public string registerExplain { get; set; }//登记说明
        public string registerTime { get; set; }//登记日期
        public string registerMan { get; set; }//登记人
        public string registerDepartment { get; set; }//登记人部门
        public string requestMan { get; set; }//请款人
        public string requestDepartment { get; set; }//请款单位
        public string requestTime { get; set; }//请款日期
        public string receiptDepartment { get; set; }//收款单位 
        public string compactNumber { get; set; }//合同编号
        public decimal? compactSum { get; set; }//合同金额
        public string purpose { get; set; }//用途
        public string payMethod { get; set; }//支付方式
        public int? compactId { get; set; }//合同ID
        public string expenseProperty { get; set; }//支出性质
        public string chargeType { get; set; }//费用种类
        public string budgetYear { get; set; }//预算年度
        public string budgeAssessor { get; set; }//预算审核员
        public string budgeAssessorApprovalTime { get; set; }//预算审核员审批时间
        public string budgeAssessorApprovalOpinion { get; set; }//预算审核员审批意见
        public string controlledCorporationAccountant { get; set; }//分公司分管财务
        public string controlledCorporationAccountantApprovalTime { get; set; }//分公司分管财务审批时间
        public string controlledCorporationAccountantApprovalOpinion { get; set; }//分公司分管财务审批意见
        public string controlledCorporationOperationDepartment { get; set; }//分公司运行部
        public string controlledCorporationOperationDepartmentApprovalTime { get; set; }//分公司运行部审批时间
        public string controlledCorporationOperationDepartmentApprovalOpinion { get; set; }//分公司运行部审批意见
        public string controlledCorporationAdministrativeDepartment { get; set; }//分公司行政部
        public string controlledCorporationAdministrativeDepartmentApprovalTime { get; set; }//分公司行政部审批时间
        public string controlledCorporationAdministrativeDepartmentApprovalOpinion { get; set; }//分公司行政部审批意见
        public string controlledCorporationApproval { get; set; }//分公司审批
        public string controlledCorporationApprovalTime { get; set; }//分公司审批时间
        public string controlledCorporationApprovalOpinion { get; set; }//分公司审批意见 
        public string controlledCorporationLeader { get; set; }//分公司领导
        public string controlledCorporationLeaderApprovalTime { get; set; }//分公司领导审批时间
        public string controlledCorporationLeaderApprovalOpinion { get; set; }//分公司领导审批意见
        public string isInPurchasePlan { get; set; }//是否采购计划内
        public string purchaseNumber { get; set; }//采购编号
        public string budgeBureau { get; set; }//预算处
        public string requestNumber { get; set; }//请款编号
        public string requestMonth { get; set; }//请款月份
        public string budgeType { get; set; }//预算类别
        public decimal? requestSum { get; set; }//请款金额
        public ItemOfExpenditure[] itemOfExpenditures { get; set; }//支出项目
    }

    public class ItemOfExpenditure
    {
        public int? budgeId { get; set; }//预算ID
        public string budgeNumber { get; set; }//预算编号
        public string itemName { get; set; }//项目名称
        public decimal? monthBudgeSum { get; set; }//月累计预算金额
        public decimal? budgeUsableSum { get; set; }//本次付款前本项月累计预算使用金额
        public decimal? budgeRemainingSum { get; set; }//本次付款前本项月累计预算余额
        public decimal? requestSum { get; set; }//请款金额
        public decimal? paySum { get; set; }//实付金额
        public decimal? yearBudgeSum { get; set; }//年度预算金额
        public decimal? yearRemainingSum { get; set; }//年累计预算余额
    }
}