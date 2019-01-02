using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using HexiUserServer.Models;

namespace HexiUserServer.Business
{
    public class ChargeDal
    {
        public static StatusReport GetCharges(string ztCode, string roomNumber, string userName)
        {
            StatusReport sr = new StatusReport()
            {
                status = "Success",
                result = "成功"
            };
            Charge charge = new Charge();
            string sqlString = " select 应收款ID as ID " +
                                " from weixin.dbo.应收款APP" +
                                " where 帐套代码 = @帐套代码 " +
                                " and 房号 = @房号 " +
                                " and 占用者名称 = @占用者名称 " +
                                " and 收费状态 IS NULL";

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@占用者名称", userName),
                new SqlParameter("@房号", roomNumber),
                new SqlParameter("@帐套代码", ztCode));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到符合条件的记录";
                return sr;
            }
            List<int?> idList = new List<int?>();
            foreach (DataRow row in dt.Rows)
            {

                int? id = DataTypeHelper.GetIntValue(row["ID"]);
                idList.Add(id);
            }
            charge.Ids = idList.ToArray();
            
            sqlString = "SELECT 资源编号, 占用者名称, SUM(应收金额) AS 应收总额, MIN(计费年月) + '-' + MAX(计费年月) + 费用名称 AS 费用名称 " +
                "FROM weixin.dbo.应收款APP " +
                " where 帐套代码 = @帐套代码 " +
                " and 房号 = @房号 " +
                " and 占用者名称 = @占用者名称 " +
                " and 收费状态 IS NULL " +
                "GROUP BY 资源编号, 占用者名称, 费用名称 ";
            dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@占用者名称", userName),
                new SqlParameter("@房号", roomNumber),
                new SqlParameter("@帐套代码", ztCode));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到符合条件的记录";
                return sr;
            }
            List<ChargeDetail> chargeList = new List<ChargeDetail>();
            foreach(DataRow dr in dt.Rows)
            {
                ChargeDetail chargeDetail = new ChargeDetail()
                {
                    ChargeName = DataTypeHelper.GetStringValue(dr["费用名称"]),
                    Charge = DataTypeHelper.GetDoubleValue(dr["应收总额"])
                };
                chargeList.Add(chargeDetail);
            }
            charge.ChargeDetail = chargeList.ToArray();
            sr.data = charge;
            return sr;
        }
        public static StatusReport SetCharges(string datetime, string proprietorName, string tradeNumber, string[] chargeIds)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "update weixin.dbo.应收款APP " +
                                "set 收费状态 = '已收费', " +
                                " 收费日期 = @收费日期, " +
                                " 付款方式 = @付款方式, " +
                                " 收款人 = @收款人, " +
                                " 备注 = @备注 " +
                                " where 应收款ID in (";
            foreach (string ID in chargeIds)
            {
                sqlString += ID + "," ;
            }
            sqlString = sqlString.Substring(0, sqlString.Length - 1) + ")";

            sr = SQLHelper.Update("wyt", sqlString, 
                new SqlParameter("@收费日期", datetime), 
                new SqlParameter("@收款人", proprietorName),
                new SqlParameter("@备注", tradeNumber),
                new SqlParameter("@付款方式","小程序微信支付"));
            sr.parameters = sqlString;
            return sr;
        }

        //    public static StatusReport SetCharges(string datetime, string totalCharge, string roomName, string proprietorName, string[] chargeIds)
        //    {
        //        StatusReport sr = new StatusReport();


        //        //如果没有接收到ID列表，直接返回错误信息
        //        if (chargeIds.Length == 0 || datetime == null || totalCharge == null || roomName == null || proprietorName == null)
        //        {
        //            sr.status = "Fail";
        //            sr.result = "收费信息不完整";
        //            return sr;
        //        }
        //        string date = datetime.Split(' ')[0];
        //        int cId = Convert.ToInt32(chargeIds[0]);
        //        //向数据表“收款情况”中写入数据
        //        string sqlString = "  insert into 收款情况 (资源表名称,资源表ID,收费日期,收费金额,收费方式,付款方式,收款人,帐套代码,当日日期,收费时间) " +
        //            " select 资源表名称,资源表ID,@收费日期,@收费金额,@收费方式,@付款方式,@收款人,帐套代码,@当日日期,@收费时间 from 应收款 where ID = @ID " +
        //            " select @@IDENTITY";

        //        sr = SQLHelper.Insert("wyt", sqlString,
        //            new SqlParameter("@收费日期", date),
        //            new SqlParameter("@收费金额", totalCharge),
        //            new SqlParameter("@收费方式", "小程序支付"),
        //            new SqlParameter("@付款方式", "微信支付"),
        //            new SqlParameter("@收款人", "admin"),
        //            new SqlParameter("@当日日期", date),
        //            new SqlParameter("@收费时间", datetime),
        //            new SqlParameter("@ID", cId));
        //        if (sr.data == null)
        //        {
        //            return sr;
        //        }
        //        int situationId = Convert.ToInt32(sr.data);//收款情况中新插入列的ID
        //        //向数据表“收据”中写入数据
        //        sqlString = "insert into 收据 (收款ID,收款人,收费金额,收费大写金额,付款方式,打印场所,收费日期,收费资源编号,交费人,帐套代码) " +
        //            " select ID, 收款人, 收费金额, @收费大写金额, 付款方式, 收费方式, 收费日期, @收费资源编号, @交费人, 帐套代码 from 收款情况 where ID = @ID " +
        //            " select @@IDENTITY";
        //        sr = SQLHelper.Insert("wyt", sqlString,
        //           new SqlParameter("@收费大写金额", "零零七"),
        //            new SqlParameter("@收费资源编号", roomName),
        //            new SqlParameter("@交费人", proprietorName),
        //            new SqlParameter("@ID", situationId));
        //        if (sr.data == null)
        //        {
        //            return sr;
        //        }
        //        int receiptId = Convert.ToInt32(sr.data);//收据中新插入列的ID
        //        string updateString = "update 应收款 set 收款ID = @收款ID, 收据ID = @收据ID, 收费状态 = @收费状态 where ID in (";
        //        foreach (string chargeId in chargeIds)
        //        {
        //            //int id = Convert.ToInt32(chargeId);
        //            updateString += (chargeId + ",");
        //        }
        //        updateString = updateString.Remove(updateString.Length - 1, 1) + ")";

        //        sr = SQLHelper.Update("wyt", updateString,
        //             new SqlParameter("@收款ID", situationId),
        //            new SqlParameter("@收据ID", receiptId),
        //            new SqlParameter("@收费状态", "已收费"));
        //        return sr;
        //    }

        //    private static void GetChargeInfo(int id)
        //    {

        //    }
        //}
    }
}




/*
 * SELECT   dbo.应收款.ID, dbo.应收款.计费年月, dbo.费用项目.费用名称, dbo.应收款.费用说明, dbo.应收款.应收金额, 
                dbo.应收款.收费状态, dbo.应收款.收款ID, dbo.应收款.收据ID
FROM      dbo.应收款 LEFT OUTER JOIN
                dbo.费用项目 ON dbo.应收款.费用项目ID = dbo.费用项目.ID
WHERE   (dbo.应收款.占用者ID = '6') AND (dbo.应收款.收费状态 IS NULL)
 */






//public static Charge[] GetCharges(string ztCode, string roomNumber, string userName)
//{
//    string sqlString = " select 应收款ID as ID,计费年月,费用名称,费用说明,应收金额,收费状态,帐套名称 " +
//                        " from weixin.dbo.应收款APP" +
//                        " where 帐套代码 = @帐套代码 " +
//                        " and 房号 = @房号 " +
//                        " and 占用者名称 = @占用者名称 " +
//                        " and 收费状态 IS NULL";

//    DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
//        new SqlParameter("@占用者名称", userName),
//        new SqlParameter("@房号", roomNumber),
//        new SqlParameter("@帐套代码", ztCode));


//    List<ChargeDetail> cdList = new List<ChargeDetail>();

//    foreach (DataRow row in dt.Rows)
//    {
//        ChargeDetail cd = new ChargeDetail()
//        {
//            Id = DataTypeHelper.GetIntValue(row["ID"]),
//            ChargeTime = DataTypeHelper.GetStringValue(row["计费年月"]),
//            ChargeName = DataTypeHelper.GetStringValue(row["费用名称"]),
//            ChargeInfo = DataTypeHelper.GetStringValue(row["费用说明"]),
//            Charge = DataTypeHelper.GetDoubleValue(row["应收金额"]),
//            ChargeStatus = DataTypeHelper.GetStringValue(row["收费状态"])
//        };
//        cdList.Add(cd);
//    }

//    ChargeDetail[] cdArray = cdList.ToArray();

//    if (cdArray.Length == 0)
//    {
//        return null;
//    }
//    ///////////////////////////////////////////////
//    string chargeName = cdArray[0].ChargeName;

//    List<Charge> chargeList = new List<Charge>();

//    int i = 0;
//    while (i < cdArray.Length)
//    {
//        string chargeTime = cdArray[0].ChargeTime;
//        int k = 0;
//        while (k < cdArray.Length)
//        {
//            ChargeResult chargeResult = new ChargeResult();

//        }

//        Charge charge = new Charge();
//        charge.ChargeName = chargeName;
//        List<ChargeDetail> list = new List<ChargeDetail>();

//        for (int j = i; j < cdArray.Length; j++)
//        {
//            if (cdArray[j].ChargeName == chargeName)
//            {
//                list.Add(cdArray[j]);
//                if (j == cdArray.Length - 1)
//                {
//                    charge.ChargeDetails = list.ToArray();
//                    i = cdArray.Length;
//                    break;
//                }
//            }
//            else
//            {
//                charge.ChargeDetails = list.ToArray();
//                chargeName = cdArray[j].ChargeName;
//                i = j;
//                break;
//            }
//        }
//        chargeList.Add(charge);
//    }
//    ChargeResult chargeResult = new ChargeResult();
//    chargeResult.Charges = chargeList.ToArray();
//    return chargeList.ToArray();
//    /////////////////////////////////////////////////
//}