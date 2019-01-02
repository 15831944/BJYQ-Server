using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Complain
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 投诉接待日期
        /// </summary>
        public string ReceptionDate { get; set; }
        /// <summary>
        /// 投诉方式
        /// </summary>
        public string Way { get; set; }
        /// <summary>
        /// 投诉人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 投诉内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Classify { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 投诉处理单编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 发单人
        /// </summary>
        public string SendPerson { get; set; }//发单人
        /// <summary>
        /// 接单人
        /// </summary>
        public string ReceivePerson { get; set; }//接单人
        /// <summary>
        /// 处理完成日期
        /// </summary>
        public string FinishDate { get; set; }
        /// <summary>
        /// 处理完成情况
        /// </summary>
        public string FinishStatus { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        public string Registrant { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 投诉前照片
        /// </summary>
        public string[] BeforeImage { get; set; }
        /// <summary>
        /// 处理后照片
        /// </summary>
        public string[] AfterImage { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 是否满意
        /// </summary>
        public string IsSatisfying { get; set; }//是否满意
        /// <summary>
        /// 业主确认完成  
        /// </summary>
        public string AffirmComplete { get; set; }//业主确认完成
        /// <summary>
        /// 确认时间  
        /// </summary>
        public string AffirmCompleteTime { get; set; }//业主确认完成时间
        /// <summary>
        /// 业主评价  
        /// </summary>
        public string AffirmCompleteEvaluation { get; set; }//业主评价
    }




    public class ComplainStatistics
    {
        public string name { get; set; }
        public string countReceive { get; set; }
        public string countValid { get; set; }
        public string countInvalid { get; set; }
        public string countFinished { get; set; }
        public string countUnfinished { get; set; }
        public string countClosed { get; set; }
        public string rateValid { get; set; }
        public string rateInvalid { get; set; }
        public string rateFinished { get; set; }
        public string rateUnfinished { get; set; }
        public string rateClosed { get; set; }
    }

    public class ComplainStatisticsProject
    {
        public string ztName { get; set; }
        public string countReceive { get; set; }
        public string countValid { get; set; }
        public string countInvalid { get; set; }
        public string countFinished { get; set; }
        public string countUnfinished { get; set; }
        public string countClosed { get; set; }
        public string rateValid { get; set; }
        public string rateInvalid { get; set; }
        public string rateFinished { get; set; }
        public string rateUnfinished { get; set; }
        public string rateClosed { get; set; }
        public ComplainStatistics[] complainStatisticsPersonal { get; set; }
    }

    public class ComplainStatisticsCompany
    {

        public string countReceive { get; set; }
        public string countValid { get; set; }
        public string countInvalid { get; set; }
        public string countFinished { get; set; }
        public string countUnfinished { get; set; }
        public string countClosed { get; set; }
        public string rateValid { get; set; }
        public string rateInvalid { get; set; }
        public string rateFinished { get; set; }
        public string rateUnfinished { get; set; }
        public string rateClosed { get; set; }
        public ComplainStatisticsProject[] complainStatisticsProject { get; set; }
    }




    public class ComplainReportStatistics
    {
        public string ztName { get; set; }
        public string countUnfinished { get; set; }
        public string countTimeout { get; set; }
    }

    public class ComplainReportStatisticsCompany
    {
        public string countUnfinished { get; set; }
        public string countTimeout { get; set; }
        public ComplainReportStatistics[] complainReportStatistics { get; set; }
    }

    //public class ComplainReport
    //{
    //    public Complain[] complains { get; set; }
    //}

    public class ComplainReport : Complain
    {

        public string ZTName { get; set; }
        public string ZTCode { get; set; }
        public string type { get; set; }
    }
}