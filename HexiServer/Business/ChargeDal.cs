using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiServer.Common;
using HexiServer.Models;
using HexiUtils;

namespace HexiServer.Business
{
    public class ChargeDal
    {
        public static StatusReport GetChargedList(string homeNumber, string buildingNumber, string name, string ztcode, string startMonth, string endMonth)
        {
            StatusReport sr = new StatusReport();
            string homeNumberCondition = string.IsNullOrEmpty(homeNumber) ? "" : " and (房产单元编号 like @房产单元编号) ";
            string buildingNumberCondition = string.IsNullOrEmpty(buildingNumber) ? "" : " and (所属楼宇 like @所属楼宇) ";
            string nameCondition = string.IsNullOrEmpty(name) ? "" : " and (占用者名称 like @占用者名称) ";
            string sqlString = "" +
                                "SELECT 房产单元编号, 占用者名称, SUM(应收金额) AS 已收总额, 帐套代码 " +
                                "FROM dbo.小程序_已收查询 " +
                                "WHERE 帐套代码 = @帐套代码" +
                                homeNumberCondition +
                                buildingNumberCondition +
                                nameCondition +
                                " and 计费开始年月 >= @开始年月 " +
                                " and 计费开始年月 <= @结束年月 " + 
                                " GROUP BY 房产单元编号, 占用者名称, 帐套代码 " +
                                " ORDER BY 占用者名称 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, 
                new SqlParameter("@占用者名称","%" + name + "%"),
                new SqlParameter("@房产单元编号", "%" + homeNumber + "%"),
                new SqlParameter("@所属楼宇", "%" + buildingNumber + "%"),
                new SqlParameter("@帐套代码", ztcode),
                new SqlParameter("@开始年月", startMonth),
                new SqlParameter("@结束年月", endMonth));


            //string sqlString = "SELECT 房产单元编号, 占用者名称, SUM(应收金额) AS 已收总额, 帐套代码 " +
            //                    "FROM dbo.小程序_已收查询 " +
            //                    "WHERE 帐套代码 = " + ztcode +
            //                    (string.IsNullOrEmpty(homeNumber) ? "" : "and (房产单元编号 like '%" + homeNumber + "%') ") +
            //                    (string.IsNullOrEmpty(buildingNumber) ? "" : "and (所属楼宇 like '%" + buildingNumber + "%') ") +
            //                    (string.IsNullOrEmpty(name) ? "" : "and (占用者名称 like '%" + name + "%') ") +
            //                    " and 计费开始年月 >= " + startMonth +
            //                    " and 计费开始年月 <= " + endMonth +
            //                    " GROUP BY 房产单元编号, 占用者名称, 帐套代码 " +
            //                    " ORDER BY 占用者名称 ";

            //DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }
            //List<Charged> chargedList = new List<Charged>();

            //foreach (DataRow row in dt.Rows)
            //{
            //    Charged c = new Charged()
            //    {
            //        RoomNumber = DataTypeHelper.GetStringValue(row["房产单元编号"]),
            //        Name = DataTypeHelper.GetStringValue(row["占用者名称"]),
            //        Total = DataTypeHelper.GetDoubleValue(row["已收总额"]),
            //        ZTCode = DataTypeHelper.GetStringValue(row["帐套代码"]),
            //    };
            //    chargedList.Add(c);
            //}
            List<object> chargedList = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                var c = new 
                {
                    RoomNumber = DataTypeHelper.GetStringValue(row["房产单元编号"]),
                    Name = DataTypeHelper.GetStringValue(row["占用者名称"]),
                    Total = DataTypeHelper.GetDoubleValue(row["已收总额"]),
                    ZTCode = DataTypeHelper.GetStringValue(row["帐套代码"]),
                };
                chargedList.Add(c);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = chargedList.ToArray();
            return sr;
        }
        

        public static StatusReport GetChargedDetail(string homeNumber, string name, string ztcode, string startMonth, string endMonth)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "SELECT 应收金额, 计费年月, 付款方式, 费用名称, 计费开始年月, 计费截至年月, 收款人, 收费日期 " +
                "FROM dbo.小程序_已收查询 " +
                "WHERE 房产单元编号 = @房产单元编号 and 占用者名称 = @占用者名称 and 帐套代码 = @帐套代码 " +
                "and 计费开始年月 >= @计费开始年月 and 计费开始年月 <= @计费截止年月 " +
                "ORDER BY 计费年月,费用名称 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@房产单元编号", homeNumber),
                new SqlParameter("@占用者名称", name),
                new SqlParameter("@帐套代码", ztcode),
                new SqlParameter("@计费开始年月", startMonth),
                new SqlParameter("@计费截止年月", endMonth));

            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }
            List<ChargedDetail> cdList = new List<ChargedDetail>();
            foreach (DataRow row in dt.Rows)
            {
                ChargedDetail cd = new ChargedDetail()
                {
                    AmountReceivable = DataTypeHelper.GetDoubleValue(row["应收金额"]),
                    AmountMonth = string.IsNullOrEmpty(DataTypeHelper.GetStringValue(row["计费年月"])) ? "其他费用" : DataTypeHelper.GetStringValue(row["计费年月"]),
                    ChargeName = DataTypeHelper.GetStringValue(row["费用名称"]),
                    ChargeDate = FormatDate(DataTypeHelper.GetDateStringValue(row["收费日期"])),
                    StartMonth = DataTypeHelper.GetStringValue(row["计费开始年月"]),
                    EndMonth = DataTypeHelper.GetStringValue(row["计费截至年月"]),
                    Cashier = DataTypeHelper.GetStringValue(row["收款人"]),
                    ChargeWay = DataTypeHelper.GetStringValue(row["付款方式"])
                };
                cdList.Add(cd);
            }
            ChargedDetail[] cdArray = cdList.ToArray();
            if (cdArray.Length == 0)
            {
                return null;
            }
            string month = cdArray[0].AmountMonth;

            List<ChargedResult> resList = new List<ChargedResult>();

            int i = 0;
            while (i < cdArray.Length)
            {
                ChargedResult cr = new ChargedResult();
                cr.AmountMonth = month;
                List<ChargedDetail> list = new List<ChargedDetail>();

                for (int j = i; j < cdArray.Length; j++)
                {
                    if (cdArray[j].AmountMonth == month)
                    {
                        list.Add(cdArray[j]);
                        if (j == cdArray.Length - 1)
                        {
                            cr.Detail = list.ToArray();
                            i = cdArray.Length;
                            break;
                        }
                    }
                    else
                    {
                        cr.Detail = list.ToArray();
                        month = cdArray[j].AmountMonth;
                        i = j;
                        break;
                    }
                }
                resList.Add(cr);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = resList.ToArray();
            return sr;
        }

        public static StatusReport GetChargeList(string homeNumber, string name, string ztcode, string buildingNumber)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "SELECT 资源编号, 占用者名称, SUM(应收金额) AS 已收总额, 帐套代码 " +
                                "FROM 应收款APP " +
                                "WHERE 帐套代码 = " + ztcode +
                                " and 收费状态 is null " +
                                (string.IsNullOrEmpty(homeNumber) ? "" : "and (资源编号 like '%" + homeNumber + "%') ") +
                                (string.IsNullOrEmpty(buildingNumber) ? "" : "and (所属楼宇 like '%" + buildingNumber + "%') ") +
                                (string.IsNullOrEmpty(name) ? "" : "and (占用者名称 like '%" + name + "%') ") +
                                "GROUP BY 资源编号, 占用者名称, 帐套代码 " +
                                "ORDER BY 占用者名称";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                sr.parameters = sqlString;
                return sr;
            }
            List<Charged> chargedList = new List<Charged>();

            foreach (DataRow row in dt.Rows)
            {
                Charged c = new Charged()
                {
                    RoomNumber = DataTypeHelper.GetStringValue(row["资源编号"]),
                    Name = DataTypeHelper.GetStringValue(row["占用者名称"]),
                    Total = DataTypeHelper.GetDoubleValue(row["已收总额"]),
                    ZTCode = DataTypeHelper.GetStringValue(row["帐套代码"]),
                };
                chargedList.Add(c);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = chargedList.ToArray();
            return sr;
        }
        public static StatusReport GetCharges(string ztCode, string roomNumber, string userName)
        {

            //string sqlString = 
            //    " SELECT dbo.应收款.ID, dbo.应收款.计费年月, dbo.费用项目.费用名称, dbo.应收款.费用说明," +
            //    " dbo.应收款.应收金额,dbo.应收款.收费状态, dbo.应收款.收款ID, dbo.应收款.收据ID, dbo.资源帐套表.帐套名称 " +
            //    " FROM dbo.应收款 " +
            //    " LEFT OUTER JOIN dbo.资源帐套表 ON dbo.应收款.帐套代码 = dbo.资源帐套表.帐套代码 " +
            //    " LEFT OUTER JOIN dbo.费用项目 ON dbo.应收款.费用项目ID = dbo.费用项目.ID " +
            //    " WHERE (dbo.应收款.占用者ID = @占用者ID) " +
            //    " AND (dbo.应收款.房产单元ID = @房产单元ID) " +
            //    " AND (dbo.应收款.帐套代码 = @帐套代码) " +
            //    " AND (dbo.应收款.收费状态 IS NULL)";

            StatusReport sr = new StatusReport();

            string sqlString = " select 应收款ID as ID,计费年月,费用名称,费用说明,应收金额,收费状态 " +
                                " from 应收款APP" +
                                " where 帐套代码 = @帐套代码 " +
                                " and 房号 = @房号 " +
                                " and 占用者名称 = @占用者名称 " +
                                " and 收费状态 IS NULL " +
                                " order by 费用名称";

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@占用者名称", userName),
                new SqlParameter("@房号", roomNumber),
                new SqlParameter("@帐套代码", ztCode));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }

            List<ChargeDetail> cdList = new List<ChargeDetail>();

            foreach (DataRow row in dt.Rows)
            {
                ChargeDetail cd = new ChargeDetail()
                {
                    Id = DataTypeHelper.GetIntValue(row["ID"]),
                    ChargeTime = DataTypeHelper.GetStringValue(row["计费年月"]),
                    ChargeName = DataTypeHelper.GetStringValue(row["费用名称"]),
                    ChargeInfo = DataTypeHelper.GetStringValue(row["费用说明"]),
                    Charge = DataTypeHelper.GetDoubleValue(row["应收金额"]),
                    ChargeStatus = DataTypeHelper.GetStringValue(row["收费状态"])
                };
                cdList.Add(cd);
            }

            ChargeDetail[] cdArray = cdList.ToArray();

            if (cdArray.Length == 0)
            {
                return null;
            }
            /////////////////////////////////////////////////
            string chargeName = cdArray[0].ChargeName;

            List<Charge> chargeList = new List<Charge>();

            int i = 0;
            while (i < cdArray.Length)
            {
                //string chargeTime = cdArray[0].ChargeTime;
                //int k = 0;
                //while (k < cdArray.Length)
                //{
                //    ChargeResult chargeResult = new ChargeResult();

                //}

                Charge charge = new Charge();
                charge.ChargeName = chargeName;
                List<ChargeDetail> list = new List<ChargeDetail>();

                for (int j = i; j < cdArray.Length; j++)
                {
                    if (cdArray[j].ChargeName == chargeName)
                    {
                        list.Add(cdArray[j]);
                        if (j == cdArray.Length - 1)
                        {
                            charge.ChargeDetails = list.ToArray();
                            i = cdArray.Length;
                            break;
                        }
                    }
                    else
                    {
                        charge.ChargeDetails = list.ToArray();
                        chargeName = cdArray[j].ChargeName;
                        i = j;
                        break;
                    }
                }
                chargeList.Add(charge);
            }
            //ChargeResult chargeResult = new ChargeResult();
            //chargeResult.Charges = chargeList.ToArray();

            sr.status = "Success";
            sr.result = "成功";
            sr.data = chargeList.ToArray();
            return sr;
            ///////////////////////////////////////////////////
        }

        public static StatusReport SetCharges(string datetime, string name, string[] chargeIds, string paymentMethod)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "update 应收款APP " +
                                "set 收费状态 = '已收费', " +
                                " 收费日期 = @收费日期, " +
                                " 付款方式 = @付款方式, " +
                                " 收款人 = @收款人 " +

                                " where 应收款ID in (";
            foreach (string ID in chargeIds)
            {
                sqlString += ID + ",";
            }
            sqlString = sqlString.Substring(0, sqlString.Length - 1) + ")";

            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@收费日期", datetime),
                new SqlParameter("@收款人", name),
                new SqlParameter("@付款方式", paymentMethod));
            sr.parameters = sqlString;
            return sr;
        }

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        /// <summary>
        /// 小程序_报表_考核_物业管理费绩效考核_全公司
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static StatusReport GetChargeStatisticsCompany(string month)
        {
            string sqlString = "[dbo].[小程序_报表_考核_物业管理费绩效考核_全公司]";
            SqlParameter spMonth = SQLHelper.setParameterValue("@统计月份", month, SqlDbType.NVarChar);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spMonth);
            if (ds.Tables.Count < 2)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            DataTable dtCompany = ds.Tables[0];
            if (dtCompany.Rows.Count == 0)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            DataRow drCompany = dtCompany.Rows[0];
            var itemCompany = new
            {
                receivableYear = DataTypeHelper.GetStringValue(drCompany["本年应收"]),
                receiptYear = DataTypeHelper.GetStringValue(drCompany["本年实收"]),
                earlierStage = DataTypeHelper.GetStringValue(drCompany["追缴前期"]),
                collectionRate = DataTypeHelper.GetStringValue(drCompany["本年收缴率"])
            };

            List<object> list = new List<object>();
            DataTable dtProject = ds.Tables[1];
            if (dtProject.Rows.Count != 0)
            {
                foreach (DataRow drProject in dtProject.Rows)
                {
                    var itemProject = new
                    {
                        ztCode = DataTypeHelper.GetStringValue(drProject["帐套代码"]),
                        ztName = DataTypeHelper.GetStringValue(drProject["帐套名称"]),
                        receivableYear = DataTypeHelper.GetStringValue(drProject["本年应收"]),
                        receiptYear = DataTypeHelper.GetStringValue(drProject["本年实收"]),
                        earlierStage = DataTypeHelper.GetStringValue(drProject["追缴前期"]),
                        collectionRate = DataTypeHelper.GetStringValue(drProject["本年收缴率"])
                    };
                    list.Add(itemProject);
                }
            }
            
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = new { company = itemCompany, project = list.ToArray() };
            return sr;
        }

        /// <summary>
        /// 小程序_报表_考核_物业管理费绩效考核_管理处
        /// </summary>
        /// <param name="month"></param>
        /// <param name="ztCode"></param>
        /// <returns></returns>
        public static StatusReport GetChargeStatisticsProject(string month, string ztCode)
        {
            string sqlString = "[dbo].[小程序_报表_考核_物业管理费绩效考核_管理处]";
            SqlParameter spMonth = SQLHelper.setParameterValue("@统计月份", month, SqlDbType.NVarChar);
            SqlParameter spZtcode = SQLHelper.setParameterValue("@帐套代码", ztCode, SqlDbType.NVarChar);

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spMonth,spZtcode);
            if (ds.Tables.Count < 1)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            DataRow dr = dt.Rows[0];
            var item = new
            {
                receivableYear = DataTypeHelper.GetStringValue(dr["本年应收"]),
                receiptYear = DataTypeHelper.GetStringValue(dr["本年实收"]),
                earlierStage = DataTypeHelper.GetStringValue(dr["追缴前期"]),
                collectionRate = DataTypeHelper.GetStringValue(dr["本年收缴率"])
            };
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = new { project = item};
            return sr;
        }

        public static StatusReport GetMonthChargeStatistics(string ztcode, string level, string startTime)
        {
            string sqlString = "[dbo].[小程序_报表_财务_月收费统计_管理处]";
            SqlParameter spStartTime = SQLHelper.setParameterValue("@统计开始时间", Convert.ToDateTime(startTime), SqlDbType.Date);
            SqlParameter spZTCode = SQLHelper.setParameterValue("@帐套代码", ztcode, SqlDbType.NVarChar);
            //SqlParameter spStartTime = new SqlParameter("@统计开始时间",SqlDbType.DateTime)

            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString, spStartTime, spZTCode);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            List<object> list = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                var item = new
                {
                    chargeName = DataTypeHelper.GetStringValue(dr["费用名称"]),
                    cashFooting = DataTypeHelper.GetNotNullDecimalValue(dr["现金实收合计"]),
                    checkFooting = DataTypeHelper.GetNotNullDecimalValue(dr["支票实收合计"]),
                    reduceDepositFooting = DataTypeHelper.GetNotNullDecimalValue(dr["预存扣减实收合计"]),
                    otherFooting = DataTypeHelper.GetNotNullDecimalValue(dr["其他实收合计"]),
                    overdueFineFooting = DataTypeHelper.GetNotNullDecimalValue(dr["滞纳金实收合计"]),
                    allFooting = DataTypeHelper.GetNotNullDecimalValue(dr["实收总计"]),
                    ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                    type = DataTypeHelper.GetStringValue(dr["费用种类"]),
                };
                list.Add(item);
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = list.ToArray();
            return sr;
        }
        

        public static StatusReport GetArrearageStatisticsProject(string ztcode, string month, string type)
        {
            string sqlString = "[dbo].[小程序_报表_财务_未收_欠款情况汇总_管理处]";
            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString,
                SQLHelper.setParameterValue("@统计月份", month, SqlDbType.NVarChar),
                SQLHelper.setParameterValue("@帐套代码", ztcode, SqlDbType.NVarChar),
                 SQLHelper.setParameterValue("@占用者类别", type, SqlDbType.NVarChar));
            if (ds.Tables.Count < 2)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            List<object> listCount = new List<object>();
            List<object> listDetail = new List<object>();
            DataTable dtCount = ds.Tables[0];
            if (dtCount.Rows.Count == 0)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            //foreach (DataRow dr in dtCount.Rows)
            //{
            DataRow drCount = dtCount.Rows[0];
                var itemCount = new
                {
                    countMonthArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本月欠费合计"]),
                    countCurrentArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本期欠费合计"]),
                    countYearArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本年欠费合计"]),
                    countTotalArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["累计欠费合计"]),
                    ztName = DataTypeHelper.GetStringValue(drCount["帐套名称"]),
                    type = DataTypeHelper.GetStringValue(drCount["占用者身份类别"])
                };
                //listCount.Add(item);
            //}
            DataTable dtDetail = ds.Tables[1];
            if (dtDetail.Rows.Count > 0)
            {
                foreach (DataRow dr in dtDetail.Rows)
                {
                    var item = new
                    {
                        countMonthArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本月欠费合计"]),
                        countCurrentArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本期欠费合计"]),
                        countYearArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本年欠费合计"]),
                        countTotalArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["累计欠费合计"]),
                        feeName = DataTypeHelper.GetStringValue(dr["费用名称"]),
                        feeType = DataTypeHelper.GetStringValue(dr["费用种类"])
                    };
                    listDetail.Add(item);
                }
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = new { count = itemCount, detail = listDetail.ToArray() };
            return sr;
        }

        public static StatusReport GetArrearageStatisticsCompany(string month, string type)
        {
            string sqlString = "[dbo].[小程序_报表_财务_未收_欠款情况汇总_全公司]";
            DataSet ds = SQLHelper.ExecuteProcedure("wyt", sqlString,
                SQLHelper.setParameterValue("@统计月份", month, SqlDbType.NVarChar),
                 SQLHelper.setParameterValue("@占用者类别", type, SqlDbType.NVarChar));
            if (ds.Tables.Count < 3)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            List<object> listProject = new List<object>();
            List<object> listDetail = new List<object>();
            DataTable dtCount = ds.Tables[0];
            if (dtCount.Rows.Count == 0)
            {
                return new StatusReport().SetFail("该时段没有数据，请更改时间段后再试");
            }
            //foreach (DataRow dr in dtCount.Rows)
            //{
            DataRow drCount = dtCount.Rows[0];
                var itemCount = new
                {
                    countMonthArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本月欠费合计"]),
                    countCurrentArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本期欠费合计"]),
                    countYearArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["本年欠费合计"]),
                    countTotalArrearage = DataTypeHelper.GetNotNullDecimalValue(drCount["累计欠费合计"]),
                    type = DataTypeHelper.GetStringValue(drCount["占用者身份类别"])
                };
                //listCount.Add(item);
            //}
            DataTable dtDetail = ds.Tables[1];
            if (dtDetail.Rows.Count > 0)
            {
                foreach (DataRow dr in dtDetail.Rows)
                {
                    var item = new
                    {
                        countMonthArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本月欠费合计"]),
                        countCurrentArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本期欠费合计"]),
                        countYearArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["本年欠费合计"]),
                        countTotalArrearage = DataTypeHelper.GetNotNullDecimalValue(dr["累计欠费合计"]),
                        feeName = DataTypeHelper.GetStringValue(dr["费用名称"]),
                        feeType = DataTypeHelper.GetStringValue(dr["费用种类"])
                    };
                    listDetail.Add(item);
                }
            }
            DataTable dtProject = ds.Tables[2];
            if (dtDetail.Rows.Count > 0)
            {
                foreach (DataRow dr in dtProject.Rows)
                {
                    var item = new
                    {
                        ZTCode = DataTypeHelper.GetStringValue(dr["帐套代码"]),
                        ZTName = DataTypeHelper.GetStringValue(dr["帐套名称"])
                    };
                    listProject.Add(item);
                }
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = new { count = itemCount, detail = listDetail.ToArray(), projects = listProject.ToArray() };
            return sr;
        }


        //public static StatusReport GetChargeStatistics(string ztCode, string level, string userCode, string month)
        //{
        //    StatusReport sr = new StatusReport();
        //    sr.status = "Fail";
        //    sr.result = "未查询到任何数据";
        //    switch (level)
        //    {
        //        case "一线":
        //            {
        //                string sqlString = " SELECT 所属组团, 实收当期合计," +
        //                " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                " FROM dbo.视图_物业管理费绩效考核统计表_项目所有区 " +
        //                " WHERE (帐套代码 = @帐套代码) AND (所属组团 = @所属组团) AND (统计月份 = @统计月份) " +
        //                " ORDER BY 所属组团 ";
        //                DataTable dtGroup = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@帐套代码", ztCode),
        //                    new SqlParameter("@所属组团", userCode),
        //                    new SqlParameter("@统计月份", month));
        //                if (dtGroup.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                DataRow dr = dtGroup.Rows[0];
        //                ChargeStatisticsGroup csg = new ChargeStatisticsGroup();
        //                csg.group = DataTypeHelper.GetStringValue(dr["所属组团"]);
        //                csg.receivedNow = DataTypeHelper.GetDecimalValue(dr["实收当期合计"]);
        //                csg.nowShouldReceived = DataTypeHelper.GetDecimalValue(dr["当期应收"]);
        //                csg.addupShouldReceived = DataTypeHelper.GetDecimalValue(dr["累计应收"]);
        //                csg.addupReceived = DataTypeHelper.GetDecimalValue(dr["累计实收"]);
        //                csg.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(dr["实收后期应收"]);
        //                csg.addupNotReceived = (csg.addupShouldReceived.HasValue ? csg.addupShouldReceived.Value : 0) - (csg.addupReceived.HasValue ? csg.addupReceived.Value : 0);
        //                csg.rateNowReceived = GetPercent(csg.receivedNow, csg.nowShouldReceived);
        //                csg.rateAddupReceived = GetPercent(csg.addupReceived, csg.addupShouldReceived);
        //                csg.rateBeforeReceived = GetPercent(csg.receivedAfterShouldReceived, csg.nowShouldReceived);
        //                sqlString = " SELECT 所属组团,所属楼宇,实收当期合计," +
        //                    " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                    " FROM dbo.视图_物业管理费绩效考核统计表_区中所有楼宇 " +
        //                    " WHERE (帐套代码 = @帐套代码) AND (所属组团 = @所属组团) AND (统计月份 = @统计月份) " +
        //                    " ORDER BY 所属组团,所属楼宇 ";
        //                DataTable dtbd = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@帐套代码", ztCode),
        //                    new SqlParameter("@所属组团", userCode),
        //                    new SqlParameter("@统计月份", month));
        //                if (dtbd.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                List<ChargeStatisticsBuilding> csbdList = new List<ChargeStatisticsBuilding>();
        //                for (int i = 0; i < dtbd.Rows.Count; i++)
        //                {
        //                    DataRow drbd = dtbd.Rows[i];
        //                    ChargeStatisticsBuilding csbd = new ChargeStatisticsBuilding();
        //                    csbd.group = DataTypeHelper.GetStringValue(drbd["所属组团"]);
        //                    csbd.building = DataTypeHelper.GetStringValue(drbd["所属楼宇"]);
        //                    csbd.receivedNow = DataTypeHelper.GetDecimalValue(drbd["实收当期合计"]);
        //                    csbd.nowShouldReceived = DataTypeHelper.GetDecimalValue(drbd["当期应收"]);
        //                    csbd.addupShouldReceived = DataTypeHelper.GetDecimalValue(drbd["累计应收"]);
        //                    csbd.addupReceived = DataTypeHelper.GetDecimalValue(drbd["累计实收"]);
        //                    csbd.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drbd["实收后期应收"]);
        //                    csbd.addupNotReceived = (csbd.addupShouldReceived.HasValue ? csbd.addupShouldReceived.Value : 0) - (csbd.addupReceived.HasValue ? csbd.addupReceived.Value : 0);
        //                    csbd.rateNowReceived = GetPercent(csbd.receivedNow, csbd.nowShouldReceived);
        //                    csbd.rateAddupReceived = GetPercent(csbd.addupReceived, csbd.addupShouldReceived);
        //                    csbd.rateBeforeReceived = GetPercent(csbd.receivedAfterShouldReceived, csbd.nowShouldReceived);
        //                    sqlString = " SELECT 所属组团,所属楼宇,所属单元,实收当期合计," +
        //                    " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                    " FROM dbo.视图_物业管理费绩效考核统计表_楼宇中所有单元 " +
        //                    " WHERE (帐套代码 = @帐套代码) AND (所属组团 = @所属组团) AND (所属楼宇 = @所属楼宇) AND (统计月份 = @统计月份) " +
        //                    " ORDER BY 所属组团,所属楼宇,所属单元 ";
        //                    DataTable dtUnit = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                        new SqlParameter("@帐套代码", ztCode),
        //                        new SqlParameter("@所属组团", userCode),
        //                        new SqlParameter("@所属楼宇", csbd.building),
        //                        new SqlParameter("@统计月份", month));
        //                    if (dtUnit.Rows.Count == 0)
        //                    {
        //                        return sr;
        //                    }
        //                    List<ChargeStatisticsUnit> csuList = new List<ChargeStatisticsUnit>();
        //                    for (int j = 0; j < dtUnit.Rows.Count; j++)
        //                    {
        //                        DataRow drUnit = dtUnit.Rows[j];
        //                        ChargeStatisticsUnit csu = new ChargeStatisticsUnit();
        //                        csu.group = DataTypeHelper.GetStringValue(drUnit["所属组团"]);
        //                        csu.building = DataTypeHelper.GetStringValue(drUnit["所属楼宇"]);
        //                        csu.unit = DataTypeHelper.GetStringValue(drUnit["所属单元"]);
        //                        csu.receivedNow = DataTypeHelper.GetDecimalValue(drUnit["实收当期合计"]);
        //                        csu.nowShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["当期应收"]);
        //                        csu.addupShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["累计应收"]);
        //                        csu.addupReceived = DataTypeHelper.GetDecimalValue(drUnit["累计实收"]);
        //                        csu.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["实收后期应收"]);
        //                        csu.addupNotReceived = (csu.addupShouldReceived.HasValue ? csu.addupShouldReceived.Value : 0) - (csu.addupReceived.HasValue ? csu.addupReceived.Value : 0);
        //                        csu.rateNowReceived = GetPercent(csu.receivedNow, csu.nowShouldReceived);
        //                        csu.rateAddupReceived = GetPercent(csu.addupReceived, csu.addupShouldReceived);
        //                        csu.rateBeforeReceived = GetPercent(csu.receivedAfterShouldReceived, csu.nowShouldReceived);
        //                        csuList.Add(csu);
        //                    }
        //                    csbd.csUnits = csuList.ToArray();
        //                    csbdList.Add(csbd);
        //                }
        //                csg.csBuildings = csbdList.ToArray();
        //                sr.status = "Success";
        //                sr.result = "成功";
        //                sr.data = csg;
        //            }
        //            break;
        //        case "助理":
        //        case "项目经理":
        //            {
        //                string sqlString = " SELECT 帐套名称, 实收当期合计," +
        //               " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //               " FROM dbo.视图_物业管理费绩效考核统计表_所有项目 " +
        //               " WHERE (帐套代码 = @帐套代码) AND (统计月份 = @统计月份) " +
        //               " ORDER BY 帐套代码 ";
        //                DataTable dtzt = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@帐套代码", ztCode),
        //                    new SqlParameter("@统计月份", month));
        //                if (dtzt.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                DataRow dr = dtzt.Rows[0];
        //                ChargeStatisticsProject csp = new ChargeStatisticsProject();
        //                csp.ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]);
        //                csp.receivedNow = DataTypeHelper.GetDecimalValue(dr["实收当期合计"]);
        //                csp.nowShouldReceived = DataTypeHelper.GetDecimalValue(dr["当期应收"]);
        //                csp.addupShouldReceived = DataTypeHelper.GetDecimalValue(dr["累计应收"]);
        //                csp.addupReceived = DataTypeHelper.GetDecimalValue(dr["累计实收"]);
        //                csp.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(dr["实收后期应收"]);
        //                csp.addupNotReceived = (csp.addupShouldReceived.HasValue ? csp.addupShouldReceived.Value : 0) - (csp.addupReceived.HasValue ? csp.addupReceived.Value : 0);
        //                csp.rateNowReceived = GetPercent(csp.receivedNow, csp.nowShouldReceived);
        //                csp.rateAddupReceived = GetPercent(csp.addupReceived, csp.addupShouldReceived);
        //                csp.rateBeforeReceived = GetPercent(csp.receivedAfterShouldReceived, csp.nowShouldReceived);

        //                sqlString = " SELECT 所属组团, 实收当期合计," +
        //                " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                " FROM dbo.视图_物业管理费绩效考核统计表_项目所有区 " +
        //                " WHERE (帐套代码 = @帐套代码) AND (统计月份 = @统计月份) " +
        //                " ORDER BY 所属组团 ";
        //                DataTable dtGroup = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@帐套代码", ztCode),
        //                    new SqlParameter("@统计月份", month));
        //                if (dtGroup.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                List<ChargeStatisticsGroup> csgList = new List<ChargeStatisticsGroup>();
        //                for(int k = 0; k < dtGroup.Rows.Count; k++)
        //                {
        //                    DataRow drGroup = dtGroup.Rows[k];
        //                    ChargeStatisticsGroup csg = new ChargeStatisticsGroup();
        //                    csg.group = DataTypeHelper.GetStringValue(drGroup["所属组团"]);
        //                    csg.receivedNow = DataTypeHelper.GetDecimalValue(drGroup["实收当期合计"]);
        //                    csg.nowShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["当期应收"]);
        //                    csg.addupShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["累计应收"]);
        //                    csg.addupReceived = DataTypeHelper.GetDecimalValue(drGroup["累计实收"]);
        //                    csg.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["实收后期应收"]);
        //                    csg.addupNotReceived = (csg.addupShouldReceived.HasValue ? csg.addupShouldReceived.Value : 0) - (csg.addupReceived.HasValue ? csg.addupReceived.Value : 0);
        //                    csg.rateNowReceived = GetPercent(csg.receivedNow, csg.nowShouldReceived);
        //                    csg.rateAddupReceived = GetPercent(csg.addupReceived, csg.addupShouldReceived);
        //                    csg.rateBeforeReceived = GetPercent(csg.receivedAfterShouldReceived, csg.nowShouldReceived);

        //                    sqlString = " SELECT 所属组团,所属楼宇,实收当期合计," +
        //                        " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                        " FROM dbo.视图_物业管理费绩效考核统计表_区中所有楼宇 " +
        //                        " WHERE (帐套代码 = @帐套代码) AND (所属组团 = @所属组团) AND (统计月份 = @统计月份) " +
        //                        " ORDER BY 所属组团,所属楼宇 ";
        //                    DataTable dtbd = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                        new SqlParameter("@帐套代码", ztCode),
        //                        new SqlParameter("@所属组团", csg.group),
        //                        new SqlParameter("@统计月份", month));
        //                    if (dtbd.Rows.Count == 0)
        //                    {
        //                        return sr;
        //                    }
        //                    List<ChargeStatisticsBuilding> csbdList = new List<ChargeStatisticsBuilding>();
        //                    for (int i = 0; i < dtbd.Rows.Count; i++)
        //                    {
        //                        DataRow drbd = dtbd.Rows[i];
        //                        ChargeStatisticsBuilding csbd = new ChargeStatisticsBuilding();
        //                        csbd.group = DataTypeHelper.GetStringValue(drbd["所属组团"]);
        //                        csbd.building = DataTypeHelper.GetStringValue(drbd["所属楼宇"]);
        //                        csbd.receivedNow = DataTypeHelper.GetDecimalValue(drbd["实收当期合计"]);
        //                        csbd.nowShouldReceived = DataTypeHelper.GetDecimalValue(drbd["当期应收"]);
        //                        csbd.addupShouldReceived = DataTypeHelper.GetDecimalValue(drbd["累计应收"]);
        //                        csbd.addupReceived = DataTypeHelper.GetDecimalValue(drbd["累计实收"]);
        //                        csbd.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drbd["实收后期应收"]);
        //                        csbd.addupNotReceived = (csbd.addupShouldReceived.HasValue ? csbd.addupShouldReceived.Value : 0) - (csbd.addupReceived.HasValue ? csbd.addupReceived.Value : 0);
        //                        csbd.rateNowReceived = GetPercent(csbd.receivedNow, csbd.nowShouldReceived);
        //                        csbd.rateAddupReceived = GetPercent(csbd.addupReceived, csbd.addupShouldReceived);
        //                        csbd.rateBeforeReceived = GetPercent(csbd.receivedAfterShouldReceived, csbd.nowShouldReceived);
        //                        sqlString = " SELECT 所属组团,所属楼宇,isnull(所属单元,'1') as 所属单元,实收当期合计," +
        //                        " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                        " FROM dbo.视图_物业管理费绩效考核统计表_楼宇中所有单元 " +
        //                        " WHERE (帐套代码 = @帐套代码) AND (所属组团 = @所属组团) AND (所属楼宇 = @所属楼宇) AND (统计月份 = @统计月份) " +
        //                        " ORDER BY 所属组团,所属楼宇,所属单元 ";
        //                        DataTable dtUnit = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                            new SqlParameter("@帐套代码", ztCode),
        //                            new SqlParameter("@所属组团", csbd.group),
        //                            new SqlParameter("@所属楼宇", csbd.building),
        //                            new SqlParameter("@统计月份", month));
        //                        if (dtUnit.Rows.Count == 0)
        //                        {
        //                            return sr;
        //                        }
        //                        List<ChargeStatisticsUnit> csuList = new List<ChargeStatisticsUnit>();
        //                        for (int j = 0; j < dtUnit.Rows.Count; j++)
        //                        {
        //                            DataRow drUnit = dtUnit.Rows[j];
        //                            ChargeStatisticsUnit csu = new ChargeStatisticsUnit();
        //                            csu.group = DataTypeHelper.GetStringValue(drUnit["所属组团"]);
        //                            csu.building = DataTypeHelper.GetStringValue(drUnit["所属楼宇"]);
        //                            csu.unit = DataTypeHelper.GetStringValue(drUnit["所属单元"]);
        //                            csu.receivedNow = DataTypeHelper.GetDecimalValue(drUnit["实收当期合计"]);
        //                            csu.nowShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["当期应收"]);
        //                            csu.addupShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["累计应收"]);
        //                            csu.addupReceived = DataTypeHelper.GetDecimalValue(drUnit["累计实收"]);
        //                            csu.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drUnit["实收后期应收"]);
        //                            csu.addupNotReceived = (csu.addupShouldReceived.HasValue ? csu.addupShouldReceived.Value : 0) - (csu.addupReceived.HasValue ? csu.addupReceived.Value : 0);
        //                            csu.rateNowReceived = GetPercent(csu.receivedNow, csu.nowShouldReceived);
        //                            csu.rateAddupReceived = GetPercent(csu.addupReceived, csu.addupShouldReceived);
        //                            csu.rateBeforeReceived = GetPercent(csu.receivedAfterShouldReceived, csu.nowShouldReceived);
        //                            csuList.Add(csu);
        //                        }
        //                        csbd.csUnits = csuList.ToArray();
        //                        csbdList.Add(csbd);
        //                    }
        //                    csg.csBuildings = csbdList.ToArray();
        //                    csgList.Add(csg);
        //                }
        //                csp.csGroups = csgList.ToArray();
        //                sr.status = "Success";
        //                sr.result = "成功";
        //                sr.data = csp;
        //            }
        //            break;
        //        case "公司":
        //            {
        //                string sqlString = " SELECT 实收当期合计," +
        //                " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                " FROM dbo.视图_物业管理费绩效考核统计表_公司 " +
        //                " WHERE (统计月份 = @统计月份) ";
        //                DataTable dtCompany = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@统计月份", month));
        //                if (dtCompany.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                DataRow dr = dtCompany.Rows[0];
        //                ChargeStatisticsCompany csc = new ChargeStatisticsCompany();
        //                csc.receivedNow = DataTypeHelper.GetDecimalValue(dr["实收当期合计"]);
        //                csc.nowShouldReceived = DataTypeHelper.GetDecimalValue(dr["当期应收"]);
        //                csc.addupShouldReceived = DataTypeHelper.GetDecimalValue(dr["累计应收"]);
        //                csc.addupReceived = DataTypeHelper.GetDecimalValue(dr["累计实收"]);
        //                csc.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(dr["实收后期应收"]);
        //                csc.addupNotReceived = (csc.addupShouldReceived.HasValue ? csc.addupShouldReceived.Value : 0) - (csc.addupReceived.HasValue ? csc.addupReceived.Value : 0);
        //                csc.rateNowReceived = GetPercent(csc.receivedNow, csc.nowShouldReceived);
        //                csc.rateAddupReceived = GetPercent(csc.addupReceived, csc.addupShouldReceived);
        //                csc.rateBeforeReceived = GetPercent(csc.receivedAfterShouldReceived, csc.nowShouldReceived);
        //                sqlString = " SELECT 帐套代码,帐套名称,实收当期合计," +
        //                    " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                    " FROM dbo.视图_物业管理费绩效考核统计表_所有项目 " +
        //                    " WHERE (统计月份 = @统计月份) " +
        //                    " ORDER BY 帐套代码";
        //                DataTable dtProject = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                    new SqlParameter("@统计月份", month));
        //                if (dtProject.Rows.Count == 0)
        //                {
        //                    return sr;
        //                }
        //                List<ChargeStatisticsProject> cspList = new List<ChargeStatisticsProject>();
        //                for (int i = 0; i < dtProject.Rows.Count; i++)
        //                {
        //                    DataRow drProject = dtProject.Rows[i];
        //                    ChargeStatisticsProject csp = new ChargeStatisticsProject();
        //                    csp.ztCode = DataTypeHelper.GetStringValue(drProject["帐套代码"]);
        //                    csp.ztName = DataTypeHelper.GetStringValue(drProject["帐套名称"]);
        //                    csp.receivedNow = DataTypeHelper.GetDecimalValue(drProject["实收当期合计"]);
        //                    csp.nowShouldReceived = DataTypeHelper.GetDecimalValue(drProject["当期应收"]);
        //                    csp.addupShouldReceived = DataTypeHelper.GetDecimalValue(drProject["累计应收"]);
        //                    csp.addupReceived = DataTypeHelper.GetDecimalValue(drProject["累计实收"]);
        //                    csp.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drProject["实收后期应收"]);
        //                    csp.addupNotReceived = (csp.addupShouldReceived.HasValue ? csp.addupShouldReceived.Value : 0) - (csp.addupReceived.HasValue ? csp.addupReceived.Value : 0);
        //                    csp.rateNowReceived = GetPercent(csp.receivedNow, csp.nowShouldReceived);
        //                    csp.rateAddupReceived = GetPercent(csp.addupReceived, csp.addupShouldReceived);
        //                    csp.rateBeforeReceived = GetPercent(csp.receivedAfterShouldReceived, csp.nowShouldReceived);

        //                    sqlString = " SELECT 帐套名称,所属组团,实收当期合计," +
        //                    " 当期应收,累计应收,累计实收,实收后期应收,统计月份 " +
        //                    " FROM dbo.视图_物业管理费绩效考核统计表_项目所有区 " +
        //                    " WHERE (帐套代码 = @帐套代码) AND (统计月份 = @统计月份) " +
        //                    " ORDER BY 所属组团 ";
        //                    DataTable dtGroup = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                        new SqlParameter("@帐套代码", csp.ztCode),
        //                        new SqlParameter("@统计月份", month));
        //                    if (dtGroup.Rows.Count == 0)
        //                    {
        //                        return sr;
        //                    }
        //                    List<ChargeStatisticsGroup> csgList = new List<ChargeStatisticsGroup>();
        //                    for (int j = 0; j < dtGroup.Rows.Count; j++)
        //                    {
        //                        DataRow drGroup = dtGroup.Rows[j];
        //                        ChargeStatisticsGroup csg = new ChargeStatisticsGroup();
        //                        csg.group = DataTypeHelper.GetStringValue(drGroup["所属组团"]);
        //                        csg.ztName = DataTypeHelper.GetStringValue(drGroup["帐套名称"]);
        //                        csg.receivedNow = DataTypeHelper.GetDecimalValue(drGroup["实收当期合计"]);
        //                        csg.nowShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["当期应收"]);
        //                        csg.addupShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["累计应收"]);
        //                        csg.addupReceived = DataTypeHelper.GetDecimalValue(drGroup["累计实收"]);
        //                        csg.receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(drGroup["实收后期应收"]);
        //                        csg.addupNotReceived = (csg.addupShouldReceived.HasValue ? csg.addupShouldReceived.Value : 0) - (csg.addupReceived.HasValue ? csg.addupReceived.Value : 0);
        //                        csg.rateNowReceived = GetPercent(csg.receivedNow, csg.nowShouldReceived);
        //                        csg.rateAddupReceived = GetPercent(csg.addupReceived, csg.addupShouldReceived);
        //                        csg.rateBeforeReceived = GetPercent(csg.receivedAfterShouldReceived, csg.nowShouldReceived);
        //                        csgList.Add(csg);
        //                    }
        //                    csp.csGroups = csgList.ToArray();
        //                    cspList.Add(csp);
        //                }
        //                csc.csProjects = cspList.ToArray();
        //                sr.status = "Success";
        //                sr.result = "成功";
        //                sr.data = csc;
        //            }
        //            break;
        //    }
        //    return sr;
        //}



        //public static StatusReport GetGroupChargeStatistics(string ztCode, string userCode, string month)
        //{
        //    StatusReport sr = new StatusReport();
        //    DataTable dt = new DataTable();
        //    ChargeStatisticsBase[] csbs = null;
        //    sr.status = "Fail";
        //    sr.result = "未查询到任何数据";
        //    string sqlString = " SELECT 费用种类,帐套代码,帐套名称,所属组团,所属楼宇,所属单元,实收当期合计," +
        //                       " 当期应收,累计应收,累计实收,实收后期应收,统计月份,报送日期 " +
        //                       " FROM 视图_物业管理费绩效考核统计表_楼宇中所有单元 " +
        //                       " WHERE(帐套代码 = @帐套代码) AND(所属组团 = @所属组团) AND(统计月份 = @统计月份) " +
        //                       " ORDER BY 帐套代码,所属组团,所属楼宇,所属单元 ";
        //    dt = SQLHelper.ExecuteQuery("weixin", sqlString,
        //                new SqlParameter("@帐套代码", ztCode),
        //                new SqlParameter("@所属组团", userCode),
        //                new SqlParameter("@统计月份", month));
        //    csbs = GetCsbs(dt);
        //    if (csbs.Length == 0)
        //    {
        //        return sr;
        //    }

        //    return sr;
        //}


        private static string GetPercent(decimal? value1, decimal? value2)
        {
            decimal result = 0;
            if ((!value1.HasValue) || (!value2.HasValue))
            {
                return "0%";
            }
            if (value1 == 0 || value2 == 0)
            {
                return "0%";
            }
            else
            {
                result = value1.Value / value2.Value;
                return result.ToString("p2");
            }
        }



        private static ChargeStatisticsBase[] GetCsbs(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return null;
            }
            List<ChargeStatisticsBase> csbList = new List<ChargeStatisticsBase>();
            foreach (DataRow dr in dt.Rows)
            {
                ChargeStatisticsBase csb = new ChargeStatisticsBase()
                {
                    chargeType = DataTypeHelper.GetStringValue(dr["费用种类"]),
                    ztCode = DataTypeHelper.GetStringValue(dr["帐套代码"]),
                    ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                    group = DataTypeHelper.GetStringValue(dr["所属组团"]),
                    building = DataTypeHelper.GetStringValue(dr["所属楼宇"]),
                    unit = DataTypeHelper.GetStringValue(dr["所属单元"]),
                    receivedNow = DataTypeHelper.GetDecimalValue(dr["实收当期合计"]),
                    nowShouldReceived = DataTypeHelper.GetDecimalValue(dr["当期应收"]),
                    addupShouldReceived = DataTypeHelper.GetDecimalValue(dr["累计应收"]),
                    addupReceived = DataTypeHelper.GetDecimalValue(dr["累计实收"]),
                    receivedAfterShouldReceived = DataTypeHelper.GetDecimalValue(dr["实收后期应收"]),
                    month = DataTypeHelper.GetStringValue(dr["统计月份"]),
                    date = DataTypeHelper.GetDateStringValue(dr["报送日期"])
                };
                csbList.Add(csb);
            }
            return csbList.ToArray();
        }

        private static string FormatDate(string date)
        {
            return date.Split(' ')[0];
        }


        //public static Charged[] GetChargedList(string homeNumber, string name, string ztcode, string startMonth, string endMonth)
        //{
        //    string sqlString = "SELECT   房产单元编号, 占用者名称, 应收金额, 计费年月, 费用名称, 计费开始年月, 计费截至年月, 帐套代码, 付款方式 " +
        //                        "FROM dbo.小程序_已收查询 " +
        //                        "WHERE 帐套代码 = " + ztcode +
        //                        (string.IsNullOrEmpty(homeNumber) ? "" : "and (房产单元编号 like '%" + homeNumber + "%') ") +
        //                        (string.IsNullOrEmpty(name) ? "" : "and (占用者名称 like '%" + name + "%') ");

        //    DataTable dt = SQLHelper.ExecuteQuery(sqlString);

        //    List<Charged> chargeList = new List<Charged>();
        //    DataRow dr = null;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        if (row != dr)
        //        {
        //            dr = row;
        //        }

        //    }
        //}
        
    }
}


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







//case "助理":
//case "项目经理":
//    sqlString +=
//        " WHERE (帐套代码 = @帐套代码) AND (统计月份 = @统计月份) " +
//        " ORDER BY 帐套代码,所属组团,所属楼宇,所属单元 ";
//    dt = SQLHelper.ExecuteQuery("weixin", sqlString,
//        new SqlParameter("@帐套代码", ztCode),
//        new SqlParameter("@所属组团", userCode),
//        new SqlParameter("@统计月份", month));
//    csbs = GetCsbs(dt);
//    if (csbs.Length == 0)
//    {
//        return sr;
//    }
//    break;
//case "公司":
//    sqlString +=
//        " WHERE (统计月份 = @统计月份) " +
//        " ORDER BY 帐套代码,所属组团,所属楼宇,所属单元 ";
//    dt = SQLHelper.ExecuteQuery("weixin", sqlString,
//        new SqlParameter("@帐套代码", ztCode),
//        new SqlParameter("@所属组团", userCode),
//        new SqlParameter("@统计月份", month));
//    csbs = GetCsbs(dt);
//    if (csbs.Length == 0)
//    {
//        return sr;
//    }
//    break;