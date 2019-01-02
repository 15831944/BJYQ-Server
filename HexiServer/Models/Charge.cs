using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class ChargeInfo
    {
        public string ZTName { get; set; }//帐套名称
        public string RoomNumber { get; set; }//资源表编号
        public string ProprietorName { get; set; }//占用者名称
        public Charge[] Charges { get; set; }
    }


    public class Charge
    {
        public string ChargeName { get; set; }//费用名称
        public ChargeDetail[] ChargeDetails { get; set; }//应收金额
    }

    public class ChargeDetail
    {
        public int? Id { get; set; }//应收款.ID
        public string ChargeTime { get; set; }//计费年月
        public string ChargeName { get; set; }//费用名称
        public string ChargeInfo { get; set; }//费用说明
        public double? Charge { get; set; }//应收金额
        public string ChargeStatus { get; set; }//收费状态
        public int? ChargedId { get; set; }//收款ID
        public int? ReceiptId { get; set; }//收据ID
    }

    public class ChargeStatisticsBase
    {
        public string chargeType { get; set; }//费用种类
        public string ztCode { get; set; }//帐套代码
        public string ztName { get; set; }//帐套名称
        public string group { get; set; }//所属组团
        public string building { get; set; }//所属楼宇
        public string unit { get; set; }//所属单元
                                        //public decimal? carryOverThisYearNow { get; set; }//结转本年当期
                                        //public decimal? carryOverThisYearAfter { get; set; }//结转本年后期
                                        //public decimal? carryOverAfterYear { get; set; }//结转以后年度
                                        //public decimal? beforeYearCarryOverReceived { get; set; }//上年结转实收合计
                                        //public decimal? recoveredAfter { get; set; }//追缴前期
                                        //public decimal? receivedThisYearNow { get; set; }//实收本年当期
                                        //public decimal? receivedThisYearAfter { get; set; }//实收本年后期
                                        //public decimal? receivedAfterYear { get; set; }//实收以后年度
                                        //public decimal? nowReceived { get; set; }//当期实收合计
                                        /**
                                * 实收当期合计 （已有字段）
                                * 当期应收 （已有字段）
                                * 累计欠费 （累计应收 - 累计实收）
                                * 本期收缴率 （实收当期合计 / 当期应收）
                                * 累计收费率 （累计实收 / 累计应收）
                                * 预收费率 （实收后期应收 / 当期应收）
                                * 
                                * 需查询的字段： 实收当期合计、当期应收、 累计应收、累计实收、实收后期应收
                                **/
        public decimal? receivedNow { get; set; }//实收当期合计
        public decimal? nowShouldReceived { get; set; }//当期应收
        public decimal? addupShouldReceived { get; set; }//累计应收
        public decimal? addupReceived { get; set; }//累计实收
        //public decimal? thisYearAfterShouldReceive { get; set; }//本年后期应收
        public decimal? receivedAfterShouldReceived { get; set; }//实收后期应收
        public decimal addupNotReceived { get; set; }//累计欠费
        public string rateNowReceived { get; set; }//本期收缴率
        public string rateAddupReceived { get; set; }//累计收费率
        public string rateBeforeReceived { get; set; }//预收费率
        public string month { get; set; }//统计月份
        public string date { get; set; }//报送日期
    }

    public class ChargeStatisticsUnit : ChargeStatisticsBase { }

    public class ChargeStatisticsBuilding : ChargeStatisticsBase
    {
        public ChargeStatisticsUnit[] csUnits { get; set; }
    }

    public class ChargeStatisticsGroup : ChargeStatisticsBase
    {
        public ChargeStatisticsBuilding[] csBuildings { get; set; }
    }

    public class ChargeStatisticsProject : ChargeStatisticsBase
    {
        public ChargeStatisticsGroup[] csGroups { get; set; }
    }

    public class ChargeStatisticsCompany : ChargeStatisticsBase
    {
        public ChargeStatisticsProject[] csProjects { get; set; }
    }




}
/*
 * SELECT   dbo.应收款.ID, dbo.应收款.计费年月, dbo.费用项目.费用名称, dbo.应收款.费用说明, dbo.应收款.应收金额, 
                dbo.应收款.收费状态, dbo.应收款.收款ID, dbo.应收款.收据ID
FROM      dbo.应收款 LEFT OUTER JOIN
                dbo.费用项目 ON dbo.应收款.费用项目ID = dbo.费用项目.ID
WHERE   (dbo.应收款.占用者ID = '6') AND (dbo.应收款.收费状态 IS NULL)
 */
