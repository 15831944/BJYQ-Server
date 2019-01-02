 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Decoration
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string classify { get; set; }
        /// <summary>
        /// 房产编号
        /// </summary>
        public string roomNumber { get; set; }
        /// <summary>
        /// 业主姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 业主电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 客服专员
        /// </summary>
        public string attache { get; set; }
        /// <summary>
        /// 装修类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 装修负责人   
        /// </summary>
        public string decorationMan { get; set; }
        /// <summary>
        /// 装修负责人电话
        /// </summary>
        public string decorationPhone { get; set; }
        /// <summary>
        /// 装修公司押金交纳人
        /// </summary>
        public string decorationCompanyChargeMan { get; set; }
        /// <summary>
        /// 装修押金金额
        /// </summary>
        public decimal? decorationCharge { get; set; }
        /// <summary>
        /// 财务收款人
        /// </summary>
        public string decorationChargeReceiver { get; set; }
        /// <summary>
        /// 收款日期
        /// </summary>
        public string decorationChargeReceiveDate { get; set; }
        //public TeamMember[] teamMembers { get; set; }
        /// <summary>
        /// 装修施工证编号
        /// </summary>
        public string certificateNumber { get; set; }
        /// <summary>
        /// 办理时间
        /// </summary>
        public string transactTime { get; set; }
        /// <summary>
        /// 开工时间
        /// </summary>
        public string startTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 装修内容
        /// </summary>
        public DecorationContent[] contents { get; set; }
        /// <summary>
        /// 是否封阳台
        /// </summary>
        public string needSealingBalcony { get; set; }
        /// <summary>
        /// 验收人1
        /// </summary>
        public string checkMan1 { get; set; }
        /// <summary>
        /// 验收时间1
        /// </summary>
        public string checkTime1 { get; set; }
        /// <summary>
        /// 验收结果1
        /// </summary>
        public string checkResult1 { get; set; }
        /// <summary>
        /// 验收结果说明1
        /// </summary>
        public string checkResultExplain1 { get; set; }
        /// <summary>
        /// 验收人2
        /// </summary>
        public string checkMan2 { get; set; }
        /// <summary>
        /// 验收时间2
        /// </summary>
        public string checkTime2 { get; set; }
        /// <summary>
        /// 验收结果2
        /// </summary>
        public string checkResult2 { get; set; }
        /// <summary>
        /// 验收结果说明2
        /// </summary>
        public string checkResultExplain2 { get; set; }
        /// <summary>
        /// 安装单位名称
        /// </summary>
        public string installDepartment { get; set; }
        /// <summary>
        /// 执照号码
        /// </summary>
        public string licenseNumber { get; set; }
        /// <summary>
        /// 负责人姓名
        /// </summary>
        public string leaderName { get; set; }
        /// <summary>
        /// 负责人电话
        /// </summary>
        public string leaderPhone { get; set; }
        /// <summary>
        /// 押金交纳人
        /// </summary>
        public string chargeMan { get; set; }
        /// <summary>
        /// 押金金额
        /// </summary>
        public decimal? charge { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string receiver { get; set; }
        /// <summary>
        /// 退款人
        /// </summary>
        public string refundMan { get; set; }
        /// <summary>
        /// 退款日期
        /// </summary>
        public string refundDate { get; set; }
        /// <summary>
        /// 业主确认
        /// </summary>
        public string proprietorCheck { get; set; }
        /// <summary>
        /// 工程指导人员签字
        /// </summary>
        public string checkEngineer { get; set; }
        /// <summary>
        /// 工程指导人员签字日期
        /// </summary>
        public string checkEngineerSignDate { get; set; }
        /// <summary>
        /// 门岗进场控制签字
        /// </summary>
        public string accessController { get; set; }
        /// <summary>
        /// 门岗进场控制签字日期
        /// </summary>
        public string accessControllerSignDate { get; set; }
        /// <summary>
        /// 工程确认进场签字
        /// </summary>
        public string engineeringCheckAccessMan { get; set; }
        /// <summary>
        /// 工程确认进场签字日期
        /// </summary>
        public string engineeringCheckAccessManSignDate { get; set; }
        /// <summary>
        /// 工程封装巡查签字
        /// </summary>
        public string engineeringPatrolMan { get; set; }
        /// <summary>
        /// 工程封装巡查签字日期
        /// </summary>
        public string engineeringPatrolManSignDate { get; set; }
        /// <summary>
        /// 封装完毕验收签字
        /// </summary>
        public string engineeringCheckMan { get; set; }
        /// <summary>
        /// 封装完毕验收签字日期
        /// </summary>
        public string engineeringCheckManSignDate { get; set; }
        /// <summary>
        /// 工程主管审核
        /// </summary>
        public string engineeringManager { get; set; }
        /// <summary>
        /// 工程主管审核日期
        /// </summary>
        public string engineeringManagerSignDate { get; set; }
    }

    //public class TeamMember
    //{
    //    /// <summary>
    //    /// 姓名
    //    /// </summary>
    //    public string name { get; set; }
    //    /// <summary>
    //    /// 联系电话
    //    /// </summary>
    //    public string phone { get; set; }
    //    /// <summary>
    //    /// 身份证号码
    //    /// </summary>
    //    public string idCard { get; set; }
    //}

    public class DecorationContent
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 装修内容
        /// </summary>
        public string content { get; set; }
    }



    public class DecorationPatrol
    {
        public NeedPatrolDecoration[] needPatrolDecorations { get; set; }//需要巡检的房产

        public PatrolItem[] patrolItems { get; set; }//巡检需要检查的项目

        public string[] disposeWay { get; set; }//处理方式
    }

    public class NeedPatrolDecoration
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string classify { get; set; }
        /// <summary>
        /// 房产编号
        /// </summary>
        public string roomNumber { get; set; }
        /// <summary>
        /// 业主姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 业主电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 客服专员
        /// </summary>
        public string attache { get; set; }
        /// <summary>
        /// 装修负责人   
        /// </summary>
        public string decorationMan { get; set; }
        /// <summary>
        /// 装修负责人电话
        /// </summary>
        public string decorationPhone { get; set; }
        /// <summary>
        /// 装修类型
        /// </summary>
        public string type { get; set; }
    }

    public class PatrolItem
    {
        public string number { get; set; }//序号

        public string item { get; set; }//巡检项目

        public string needReport { get; set; }//是否上报
    }
}