using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Equipment
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? ID { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Classify { get; set; }
        /// <summary>
        /// 设备运行编号
        /// </summary>
        public string OperationNumber { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; }
        /// <summary>
        /// 出厂日期
        /// </summary>
        public string ProductionDate { get; set; }
        /// <summary>
        /// 使用日期
        /// </summary>
        public string UseDate { get; set; }
        /// <summary>
        /// 设备价格
        /// </summary>
        public double? price { get; set; }
        /// <summary>
        /// 出厂序号
        /// </summary>
        public string ProductionNumber { get; set; }
        /// <summary>
        /// 设计寿命
        /// </summary>
        public string DesignedLife { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardNumber { get; set; }
        /// <summary>
        /// 安装地点
        /// </summary>
        public string UseAddress { get; set; }
        /// <summary>
        /// 产地
        /// </summary>
        public string ProductionAddress { get; set; }
        /// <summary>
        /// 设备保养管理代号
        /// </summary>
        public string MaintainNumber { get; set; }
        /// <summary>
        /// 设备保养管理内容
        /// </summary>
        public string MaintainContent { get; set; }
        /// <summary>
        /// 设备保养管理日期
        /// </summary>
        public string MaintainDate { get; set; }
        /// <summary>
        /// 工作日期
        /// </summary>
        public string WorkDate { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        public int? IsDone { get; set; }
        /// <summary>
        /// 录入日期
        /// </summary>
        public string InputDate { get; set; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string InputMan { get; set; }
        /// <summary>
        /// 完成说明
        /// </summary>
        public string DoneInfo { get; set; }
        /// <summary>
        /// 保养前照片
        /// </summary>
        public string BeforeImage { get; set; }
        /// <summary>
        /// 保养中照片
        /// </summary>
        public string MiddleImage { get; set; }
        /// <summary>
        /// 保养后照片
        /// </summary>
        public string AfterImage { get; set; }
        /// <summary>
        /// 序次
        /// </summary>
        public string Order { get; set; }
        /// <summary>
        /// 宽限上延天数
        /// </summary>
        public int? BeforeDays { get; set; }
        /// <summary>
        /// 宽限下延天数
        /// </summary>
        public int? AfterDays { get; set; }

    }

    public class EquipmentTrouble
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string classify { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string number { get; set; }
        /// <summary>
        /// 发生时间
        /// </summary>
        public string brokenTime { get; set; }
        /// <summary>
        /// 故障描述
        /// </summary>
        public string brokenInfo { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 维修人
        /// </summary>
        public string repairMan { get; set; }
        /// <summary>
        /// 维修时限
        /// </summary>
        public string repairTimeLimit { get; set; }
        /// <summary>
        /// 接单时间
        /// </summary>
        public string receiveTime { get; set; }
        /// <summary>
        /// 维修说明
        /// </summary>
        public string repairInfo { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string finishTime { get; set; }
        /// <summary>
        /// 维修前照片
        /// </summary>
        public string[] beforeImage { get; set; } //报修前照片1，2，3
        /// <summary>
        /// 处理后照片
        /// </summary>
        public string[] afterImage { get; set; } //报修前照片1，2，3
    }

    public class EquipmentStatistics
    {
        public string ztName { get; set; }
        public string countFinished { get; set; }
        public string countUnfinished { get; set; }
        public string countShouldFinished { get; set; }
        public string rateFinished { get; set; }
        public string rateUnfinished { get; set; }
    }

    public class EquipmentStatisticsCompany
    {
        public string countFinished { get; set; }
        public string countUnfinished { get; set; }
        public string countShouldFinished { get; set; }
        public string rateFinished { get; set; }
        public string rateUnfinished { get; set; }

        public EquipmentStatistics[] equipmentStatisticsProjects { get; set; }
    }

    public class EquipmentReportCompany
    {
        public string ztName { get; set; }
        public string ztCode { get; set; }
        public string countTimeout { get; set; }
    }

    public class EquipmentTroubleStatistics
    {
        public string ztName { get; set; }
        public string countReceive { get; set; }
        public string count8 { get; set; }
        public string count8to24 { get; set; }
        public string count24 { get; set; }
        public string rate8 { get; set; }
        public string rate8to24 { get; set; }
        public string rate24 { get; set; }
    }

    public class EquipmentTroubleStatisticsCompany
    {
        public string countReceive { get; set; }
        public string count8 { get; set; }
        public string count8to24 { get; set; }
        public string count24 { get; set; }
        public string rate8 { get; set; }
        public string rate8to24 { get; set; }
        public string rate24 { get; set; }
        public EquipmentTroubleStatistics[] equipmentTroubleStatisticsProjects { get; set; }
    }

    public class EquipmentTroubleReportCompany
    {
        public string ztName { get; set; }
        public string ztCode { get; set; }
        public string countTimeout { get; set; }
    }
}