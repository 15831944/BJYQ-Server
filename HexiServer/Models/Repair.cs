using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Repair
    {
        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }//Id
        /// <summary>
        /// 序号
        /// </summary>
        public string SerialNumber { get; set; }//序号
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }//部门
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }//地址
        /// <summary>
        /// 报修人
        /// </summary>
        public string RepairPerson { get; set; }//报修人
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; }//联系电话
        /// <summary>
        /// 服务项目
        /// </summary>
        public string ServiceProject { get; set; }//服务项目
        /// <summary>
        /// 服务类别
        /// </summary>
        public string ServiceCategory { get; set; }//服务类别
        /// <summary>
        /// 紧急程度
        /// </summary>
        public string Level { get; set; }//紧急程度
        /// <summary>
        /// 报修说明
        /// </summary>
        public string RepairExplain { get; set; }//报修说明
        /// <summary>
        /// 报修时间
        /// </summary>
        public string RepairTime { get; set; }//报修时间
        /// <summary>
        /// 预约服务时间
        /// </summary>
        public string OrderTime { get; set; }//预约服务时间
        /// <summary>
        /// 谈好上门时间
        /// </summary>
        public string VisitTime { get; set; }//谈好上门时间
        /// <summary>
        /// 发单人
        /// </summary>
        public string SendPerson { get; set; }//发单人
        /// <summary>
        /// 接单人
        /// </summary>
        public string ReceivePerson { get; set; }//接单人
        /// <summary>
        /// 派工时间
        /// </summary>
        public string DispatchTime { get; set; }//派工时间
        /// <summary>
        /// 到场时间
        /// </summary>
        public string ArriveTime { get; set; }//到场时间
        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatePerson { get; set; }//操作人
        /// <summary>
        /// 是否入户
        /// </summary>
        public string NeedIn { get; set; }//是否入户
        /// <summary>
        /// 完成时间
        /// </summary>
        public string CompleteTime { get; set; }//完成时间
        /// <summary>
        /// 收费类别
        /// </summary>
        public string ChargeType { get; set; }//收费类别
        /// <summary>
        /// 材料费
        /// </summary>
        public double? MaterialExpense { get; set; }//材料费
        /// <summary>
        /// 人工费
        /// </summary>
        public double? LaborExpense { get; set; }//人工费
        /// <summary>
        /// 是否已收
        /// </summary>
        public string IsPaid { get; set; }//是否已收
        /// <summary>
        /// 是否阅读
        /// </summary>
        public int? IsRead { get; set; }//是否阅读
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }//状态
        /// <summary>
        /// 完成情况及所耗物料
        /// </summary>
        public string CompleteStatus { get; set; }//完成情况及所耗物料
        /// <summary>
        /// 报修前照片1，2，3
        /// </summary>
        public string[] BeforeImage { get; set; } //报修前照片1，2，3
        /// <summary>
        /// 处理后照片1，2，3
        /// </summary>
        public string[] AfterImage { get; set; }//处理后照片1，2，3
        /// <summary>
        /// 延期原因
        /// </summary>
        public string LateReason { get; set; }//延期原因
        /// <summary>
        /// 预计延期到
        /// </summary>
        public string LateTime { get; set; }//预计延期到
        /// <summary>
        /// 是否满意
        /// </summary>
        public string IsSatisfying { get; set; }//是否满意
        /// <summary>
        /// 业主确认完成  
        /// </summary>
        public string AffirmComplete { get; set; }//业主确认完成
        /// <summary>
        /// 业主确认完成时间  
        /// </summary>
        public string AffirmCompleteTime { get; set; }//业主确认完成时间
        /// <summary>
        /// 业主评价  
        /// </summary>
        public string AffirmCompleteEvaluation { get; set; }//业主评价
        /// <summary>
        /// 回访人  
        /// </summary>
        public string CallBackPerson { get; set; }//回访人
        /// <summary>
        /// 回访时间  
        /// </summary>
        public string CallBackTime { get; set; }//回访时间
        /// <summary>
        /// 回访意见  
        /// </summary>
        public string CallBackEvaluation { get; set; }//回访意见
        /// <summary>
        /// 身份
        /// </summary>
        public string Identity { get; set; }

        public RepairCaution[] Cautions { get; set; }
    }

    public class RepairCaution
    {
        public string number { get; set; }
        public string content { get; set; }
    }


    public class RepairStatisticsPersonal
    {
        public string name { get; set; }
        public string ztName { get; set; }
        /// <summary>
        /// 接单数
        /// </summary>
        public string countReceive { get; set; }
        /// <summary>
        /// 完成数
        /// </summary>
        public string countFinished { get; set; }
        /// <summary>
        /// 未完成数
        /// </summary>
        public string countUnfinished { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        public string rateFinish { get; set; }
        /// <summary>
        /// 未完成率
        /// </summary>
        public string rateUnfinish { get; set; }
        /// <summary>
        /// 评价总数
        /// </summary>
        public string countEvaluation { get; set; }
        /// <summary>
        /// 非常满意数
        /// </summary>
        public string countVerySatisfy { get; set; }
        /// <summary>
        /// 满意数
        /// </summary>
        public string countSatisfy { get; set; }
        /// <summary>
        /// 不满意数
        /// </summary>
        public string countUnsatisfy { get; set; }
        /// <summary>
        /// 非常满意率
        /// </summary>
        public string rateVerySatisfy { get; set; }
        /// <summary>
        /// 满意率
        /// </summary>
        public string rateSatisfy { get; set; }
        /// <summary>
        /// 不满意率
        /// </summary>
        public string rateUnsatisfy { get; set; }
    }

    public class RepairStatisticsProject
    {
        public RepairStatisticsProject()
        {
            this.countReceive = "0";
            this.countFinished = "0";
            this.countUnfinished = "0";
            this.countVerySatisfy = "0";
            this.countSatisfy = "0";
            this.countUnsatisfy = "0";
        }
        public string ztName { get; set; }
        /// <summary>
        /// 接单数
        /// </summary>
        public string countReceive { get; set; }
        /// <summary>
        /// 完成数
        /// </summary>
        public string countFinished { get; set; }
        /// <summary>
        /// 未完成数
        /// </summary>
        public string countUnfinished { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        public string rateFinish { get; set; }
        /// <summary>
        /// 未完成率
        /// </summary>
        public string rateUnfinish { get; set; }
        /// <summary>
        /// 评价总数
        /// </summary>
        public string countEvaluation { get; set; }
        /// <summary>
        /// 非常满意数
        /// </summary>
        public string countVerySatisfy { get; set; }
        /// <summary>
        /// 满意数
        /// </summary>
        public string countSatisfy { get; set; }
        /// <summary>
        /// 不满意数
        /// </summary>
        public string countUnsatisfy { get; set; }
        /// <summary>
        /// 非常满意率
        /// </summary>
        public string rateVerySatisfy { get; set; }
        /// <summary>
        /// 满意率
        /// </summary>
        public string rateSatisfy { get; set; }
        /// <summary>
        /// 不满意率
        /// </summary>
        public string rateUnsatisfy { get; set; }

        public RepairStatisticsPersonal[] repairStatisticsPersonal { get; set; }
    }


    public class RepairStatisticsCompany
    {
        public string ztName { get; set; }
        /// <summary>
        /// 接单数
        /// </summary>
        public string countReceive { get; set; }
        /// <summary>
        /// 完成数
        /// </summary>
        public string countFinished { get; set; }
        /// <summary>
        /// 未完成数
        /// </summary>
        public string countUnfinished { get; set; }
        /// <summary>
        /// 完成率
        /// </summary>
        public string rateFinish { get; set; }
        /// <summary>
        /// 未完成率
        /// </summary>
        public string rateUnfinish { get; set; }
        /// <summary>
        /// 评价总数
        /// </summary>
        public string countEvaluation { get; set; }
        /// <summary>
        /// 非常满意数
        /// </summary>
        public string countVerySatisfy { get; set; }
        /// <summary>
        /// 满意数
        /// </summary>
        public string countSatisfy { get; set; }
        /// <summary>
        /// 不满意数
        /// </summary>
        public string countUnsatisfy { get; set; }
        /// <summary>
        /// 非常满意率
        /// </summary>
        public string rateVerySatisfy { get; set; }
        /// <summary>
        /// 满意率
        /// </summary>
        public string rateSatisfy { get; set; }
        /// <summary>
        /// 不满意率
        /// </summary>
        public string rateUnsatisfy { get; set; }

        public RepairStatisticsProject[] repairStatisticsProject { get; set; }
    }



    public class RepairReport : Repair
    {
        public string type { get; set; }
    }

    //public class RepairReportCompany
    //{
    //    public string type { get; set; }
    //    public Repair[] repairs { get; set; }
    //}
}