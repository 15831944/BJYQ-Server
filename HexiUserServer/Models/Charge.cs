using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class ChargeInfo
    {
        public string ZTName { get; set; }//帐套名称
        public string RoomNumber { get; set; }//资源表编号
        public string ProprietorName { get; set; }//占用者名称
        public Charge[] Charges { get; set; }
    }


    //public class Charge
    //{
    //    public string ChargeName { get; set; }//费用名称
    //    public ChargeDetail[] ChargeDetails { get; set; }//应收金额
    //}

    public class Charge
    {
        public int?[] Ids { get; set; }
        public ChargeDetail[] ChargeDetail { get; set; }

    }

    public class ChargeDetail
    {
        public string ChargeName { get; set; }//费用名称
        public double? Charge { get; set; }//应收金额
    }

    //public class ChargeDetail
    //{
    //    public int? Id { get; set; }//应收款.ID
    //    public string ChargeTime { get; set; }//计费年月
    //    public string ChargeName { get; set; }//费用名称
    //    public string ChargeInfo { get; set; }//费用说明
    //    public double? Charge { get; set; }//应收金额
    //    public string ChargeStatus { get; set; }//收费状态
    //    public int? ChargedId { get; set; }//收款ID
    //    public int? ReceiptId { get; set; }//收据ID
    //}
}
/*
 * SELECT   dbo.应收款.ID, dbo.应收款.计费年月, dbo.费用项目.费用名称, dbo.应收款.费用说明, dbo.应收款.应收金额, 
                dbo.应收款.收费状态, dbo.应收款.收款ID, dbo.应收款.收据ID
FROM      dbo.应收款 LEFT OUTER JOIN
                dbo.费用项目 ON dbo.应收款.费用项目ID = dbo.费用项目.ID
WHERE   (dbo.应收款.占用者ID = '6') AND (dbo.应收款.收费状态 IS NULL)
 */
