using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiServer.Models;
using HexiUtils;

namespace HexiServer.Business
{
    public class ComplainDal
    {
        public static StatusReport SetComplaint(string id, string finishDate, string finishStatus)
        {
            //string str = string.IsNullOrEmpty(finishDate) ? "" : " 状态 = '已完成'";
            string sqlString = " update 基础资料_顾客投诉处理登记表 set 处理完成日期 = @处理完成日期, 处理完成情况 = @处理完成情况, 状态 = @状态 " +
                " where ID = @ID " +
                "  SELECT @@IDENTITY ";
            //sqlString += str + "where ID = @ID";
            StatusReport sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@处理完成日期", finishDate),
                new SqlParameter("@处理完成情况", finishStatus),
                new SqlParameter("@状态", string.IsNullOrEmpty(finishDate) ? "" : "已完成"),
                new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport GetComplaintList(string classify, string status)
        {
            StatusReport sr = new StatusReport()
            {
                status = "Success",
                result = "成功",
            };
            string condition = "";
            if (status == "未完成")
            {
                condition = " where 分类 = @分类 and (not(状态 = '已完成') and not(状态 = '无效投诉')) " +
                            " order by ID desc ";
            }
            else
            {
                condition = " where 分类 = @分类 and 状态 = '已完成' " +
                            " order by ID desc ";
            }
            string sqlString = " select top 100 ID,投诉接待时间,投诉方式,投诉人姓名,地址,投诉内容,联系电话,投诉处理单编号,处理完成日期,处理完成情况,登记人,责任部门," +
                               " 投诉前照片1,投诉前照片2,投诉前照片3,处理后照片1,处理后照片2,处理后照片3,状态,业主确认解决,确认时间,是否满意,业主评价 " +
                               " from 基础资料_顾客投诉处理登记表 ";
            sqlString += condition;

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@分类", classify));

            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "查询条件错误或没有数据";
                return sr;
            }

            List<Complain> complainList = new List<Complain>();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                Complain complaint = new Complain()
                {
                    Id = DataTypeHelper.GetIntValue(dr["ID"]),
                    ReceptionDate = DataTypeHelper.GetDateStringValue(dr["投诉接待时间"]),
                    Way = DataTypeHelper.GetStringValue(dr["投诉方式"]),
                    Name = DataTypeHelper.GetStringValue(dr["投诉人姓名"]),
                    Address = DataTypeHelper.GetStringValue(dr["地址"]),
                    Content = DataTypeHelper.GetStringValue(dr["投诉内容"]),
                    Phone = DataTypeHelper.GetStringValue(dr["联系电话"]),
                    Number = DataTypeHelper.GetStringValue(dr["投诉处理单编号"]),
                    FinishDate = DataTypeHelper.GetDateStringValue(dr["处理完成日期"]),
                    FinishStatus = DataTypeHelper.GetStringValue(dr["处理完成情况"]),
                    Registrant = DataTypeHelper.GetStringValue(dr["登记人"]),
                    Department = DataTypeHelper.GetStringValue(dr["责任部门"]),
                    Status = DataTypeHelper.GetStringValue(dr["状态"]),
                    IsSatisfying = DataTypeHelper.GetStringValue(dr["是否满意"]),
                    AffirmComplete = DataTypeHelper.GetStringValue(dr["业主确认解决"]),
                    AffirmCompleteTime = DataTypeHelper.GetDateStringValue(dr["确认时间"]),
                    AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(dr["业主评价"])

                };
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片3"]));
                complaint.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片3"]));
                complaint.AfterImage = afterList.ToArray();
                complainList.Add(complaint);
            }

            sr.data = complainList.ToArray();
            return sr;
        }

        public static StatusReport SetComplainImage(string ID, string func, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = func == "before" ? "投诉前照片" + index.ToString() : "处理后照片" + index.ToString();
            string sqlString = " update 基础资料_顾客投诉处理登记表 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }

        public static StatusReport Evaluation(string evaluation, string isSatisfying, string id)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "update 基础资料_顾客投诉处理登记表 set 是否满意 = @是否满意, 业主评价 = @业主评价 where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@是否满意", isSatisfying),
                new SqlParameter("@业主评价", evaluation),
                new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport GetComplainStatistics(string ztCode, string level, string before)
        {
            StatusReport sr = new StatusReport();
            if (level == "助理" || level == "项目经理")
            {
                string sqlString = "SELECT " +
                    " 帐套名称, " +
                    " 接单人, " +
                    " COUNT(dbo.基础资料_顾客投诉处理登记表.ID) AS 投诉数, " +
                    " COUNT(CASE WHEN 状态 = '无效投诉' THEN ID ELSE NULL END) AS 无效投诉数, " +
                    " COUNT(CASE WHEN isnull(状态, '') <> '无效投诉' THEN ID ELSE NULL END) AS 有效投诉数, " +
                    " COUNT(CASE WHEN 状态 = '已完成' THEN ID ELSE NULL END) AS 已解决投诉数, " +
                    " COUNT(CASE WHEN 状态 <> '已完成' AND isnull(状态, '') <> '无效投诉' THEN ID ELSE NULL END) AS 未解决投诉数, " +
                    " COUNT(CASE WHEN 状态 = '关闭' THEN ID ELSE NULL END) AS 关闭数 " +
                    " FROM dbo.基础资料_顾客投诉处理登记表 " +
                    " LEFT OUTER JOIN dbo.资源帐套表 ON LEFT(dbo.基础资料_顾客投诉处理登记表.分类, 2) = dbo.资源帐套表.帐套代码 " +
                    " WHERE 帐套代码 = @帐套代码 and left(CONVERT(varchar(10),投诉接待时间,112),6) >=left(CONVERT(varchar(10),dateadd(month,@before,GETDATE()),112),6) " +
                    " GROUP BY dbo.资源帐套表.帐套名称,接单人 " +
                    " ORDER BY dbo.资源帐套表.帐套名称,接单人 ";
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                    new SqlParameter("@帐套代码", ztCode),
                    new SqlParameter("@before", -Convert.ToInt32(before)));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到任何记录";
                    return sr;
                }
                ComplainStatisticsProject csp = new ComplainStatisticsProject();
                List<ComplainStatistics> csList = new List<ComplainStatistics>();
                foreach (DataRow dr in dt.Rows)
                {
                    ComplainStatistics cs = new ComplainStatistics();
                    //cs.ztName = Convert.ToString(dr["帐套名称"]);
                    cs.name = Convert.ToString(dr["接单人"]);
                    cs.countReceive = Convert.ToString(dr["投诉数"]);
                    cs.countValid = Convert.ToString(dr["有效投诉数"]);
                    cs.countInvalid = Convert.ToString(dr["无效投诉数"]);
                    cs.countFinished = Convert.ToString(dr["已解决投诉数"]);
                    cs.countUnfinished = Convert.ToString(dr["未解决投诉数"]);
                    cs.countClosed = Convert.ToString(dr["关闭数"]);
                    cs.rateValid = GetPercent(cs.countValid, cs.countReceive);
                    cs.rateInvalid = GetPercent(cs.countInvalid, cs.countReceive);
                    cs.rateFinished = GetPercent(cs.countFinished, cs.countValid);
                    cs.rateUnfinished = GetPercent(cs.countUnfinished, cs.countValid);
                    cs.rateClosed = GetPercent(cs.countClosed, cs.countValid);
                    csList.Add(cs);
                    csp.ztName = Convert.ToString(dr["帐套名称"]);
                    csp.countReceive = Convert.ToString(Convert.ToDecimal(csp.countReceive) + Convert.ToDecimal(cs.countReceive));
                    csp.countValid = Convert.ToString(Convert.ToDecimal(csp.countValid) + Convert.ToDecimal(cs.countValid));
                    csp.countInvalid = Convert.ToString(Convert.ToDecimal(csp.countInvalid) + Convert.ToDecimal(cs.countInvalid));
                    csp.countFinished = Convert.ToString(Convert.ToDecimal(csp.countFinished) + Convert.ToDecimal(cs.countFinished));
                    csp.countUnfinished = Convert.ToString(Convert.ToDecimal(csp.countUnfinished) + Convert.ToDecimal(cs.countUnfinished));
                    csp.countClosed = Convert.ToString(Convert.ToDecimal(csp.countClosed) + Convert.ToDecimal(cs.countClosed));
                }
                csp.rateValid = GetPercent(csp.countValid, csp.countReceive);
                csp.rateInvalid = GetPercent(csp.countInvalid, csp.countReceive);
                csp.rateFinished = GetPercent(csp.countFinished, csp.countValid);
                csp.rateUnfinished = GetPercent(csp.countUnfinished, csp.countValid);
                csp.rateClosed = GetPercent(csp.countClosed, csp.countValid);
                csp.complainStatisticsPersonal = csList.ToArray();
                sr.status = "Success";
                sr.result = "成功";
                sr.data = csp;
                return sr;
            }
            else
            {
                string sqlString = "SELECT " +
                    " 帐套名称, " +
                    " COUNT(dbo.基础资料_顾客投诉处理登记表.ID) AS 投诉数, " +
                    " COUNT(CASE WHEN 状态 = '无效投诉' THEN ID ELSE NULL END) AS 无效投诉数, " +
                    " COUNT(CASE WHEN isnull(状态, '') <> '无效投诉' THEN ID ELSE NULL END) AS 有效投诉数, " +
                    " COUNT(CASE WHEN 状态 = '已完成' THEN ID ELSE NULL END) AS 已解决投诉数, " +
                    " COUNT(CASE WHEN 状态 <> '已完成' AND isnull(状态, '') <> '无效投诉' THEN ID ELSE NULL END) AS 未解决投诉数, " +
                    " COUNT(CASE WHEN 状态 = '关闭' THEN ID ELSE NULL END) AS 关闭数 " +
                    " FROM dbo.基础资料_顾客投诉处理登记表 " +
                    " LEFT OUTER JOIN dbo.资源帐套表 ON LEFT(dbo.基础资料_顾客投诉处理登记表.分类, 2) = dbo.资源帐套表.帐套代码 " +
                    " WHERE left(CONVERT(varchar(10),投诉接待时间,112),6) >=left(CONVERT(varchar(10),dateadd(month,@before,GETDATE()),112),6) " +
                    " GROUP BY dbo.资源帐套表.帐套名称 " +
                    " ORDER BY dbo.资源帐套表.帐套名称 ";
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                    new SqlParameter("@before", -Convert.ToInt32(before)));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到任何记录";
                    return sr;
                }
                ComplainStatisticsCompany csc = new ComplainStatisticsCompany();
                List<ComplainStatisticsProject> csList = new List<ComplainStatisticsProject>();
                foreach (DataRow dr in dt.Rows)
                {
                    ComplainStatisticsProject cs = new ComplainStatisticsProject();
                    cs.ztName = Convert.ToString(dr["帐套名称"]);
                    cs.countReceive = Convert.ToString(dr["投诉数"]);
                    cs.countValid = Convert.ToString(dr["有效投诉数"]);
                    cs.countInvalid = Convert.ToString(dr["无效投诉数"]);
                    cs.countFinished = Convert.ToString(dr["已解决投诉数"]);
                    cs.countUnfinished = Convert.ToString(dr["未解决投诉数"]);
                    cs.countClosed = Convert.ToString(dr["关闭数"]);
                    cs.rateValid = GetPercent(cs.countValid, cs.countReceive);
                    cs.rateInvalid = GetPercent(cs.countInvalid, cs.countReceive);
                    cs.rateFinished = GetPercent(cs.countFinished, cs.countValid);
                    cs.rateUnfinished = GetPercent(cs.countUnfinished, cs.countValid);
                    cs.rateClosed = GetPercent(cs.countClosed, cs.countValid);
                    csList.Add(cs);
                    csc.countReceive = Convert.ToString(Convert.ToDecimal(csc.countReceive) + Convert.ToDecimal(cs.countReceive));
                    csc.countValid = Convert.ToString(Convert.ToDecimal(csc.countValid) + Convert.ToDecimal(cs.countValid));
                    csc.countInvalid = Convert.ToString(Convert.ToDecimal(csc.countInvalid) + Convert.ToDecimal(cs.countInvalid));
                    csc.countFinished = Convert.ToString(Convert.ToDecimal(csc.countFinished) + Convert.ToDecimal(cs.countFinished));
                    csc.countUnfinished = Convert.ToString(Convert.ToDecimal(csc.countUnfinished) + Convert.ToDecimal(cs.countUnfinished));
                    csc.countClosed = Convert.ToString(Convert.ToDecimal(csc.countClosed) + Convert.ToDecimal(cs.countClosed));
                }
                csc.rateValid = GetPercent(csc.countValid, csc.countReceive);
                csc.rateInvalid = GetPercent(csc.countInvalid, csc.countReceive);
                csc.rateFinished = GetPercent(csc.countFinished, csc.countValid);
                csc.rateUnfinished = GetPercent(csc.countUnfinished, csc.countValid);
                csc.rateClosed = GetPercent(csc.countClosed, csc.countValid);
                csc.complainStatisticsProject = csList.ToArray();
                sr.status = "Success";
                sr.result = "成功";
                sr.data = csc;
                return sr;
            }
        }

        public static StatusReport GetComplainReportStatistics(string level)
        {
            StatusReport sr = new StatusReport();
            if (level == "公司")
            {
                string sqlString =
                    " SELECT " +
                    " COUNT(CASE WHEN 业主确认解决 = '否' THEN ID ELSE NULL END) AS 业主确认未解决数, " +
                    " COUNT(CASE WHEN DATEDIFF(hour, 接单时间, getdate()) >= 48 AND 状态 <> '已完成' AND  状态 <> '无效投诉' THEN ID ELSE NULL END) AS 超时数, " +
                    " dbo.资源帐套表.帐套代码, " +
                    " dbo.资源帐套表.帐套名称 " +
                    " FROM dbo.基础资料_顾客投诉处理登记表 " +
                    " LEFT OUTER JOIN dbo.资源帐套表 " +
                    " ON LEFT(dbo.基础资料_顾客投诉处理登记表.分类, 2) = dbo.资源帐套表.帐套代码 " +
                    " GROUP BY dbo.资源帐套表.帐套代码, dbo.资源帐套表.帐套名称 ";
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString);
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到任何数据";
                    return sr;
                }
                ComplainReportStatisticsCompany crsc = new ComplainReportStatisticsCompany();
                List<ComplainReportStatistics> crsList = new List<ComplainReportStatistics>();
                foreach (DataRow dr in dt.Rows)
                {
                    ComplainReportStatistics crs = new ComplainReportStatistics();
                    crs.ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]);
                    crs.countTimeout = DataTypeHelper.GetStringValue(dr["超时数"]);
                    crs.countUnfinished = DataTypeHelper.GetStringValue(dr["业主确认未解决数"]);
                    crsc.countTimeout = Convert.ToString(Convert.ToDecimal(crsc.countTimeout) + Convert.ToDecimal(crs.countTimeout));
                    crsc.countUnfinished = Convert.ToString(Convert.ToDecimal(crsc.countUnfinished) + Convert.ToDecimal(crs.countUnfinished));
                    crsList.Add(crs);
                }
                crsc.complainReportStatistics = crsList.ToArray();
                sr.status = "Success";
                sr.result = "成功";
                sr.data = crsc;
                return sr;
            }
            else
            {
                sr.status = "Fail";
                sr.result = "没有此权限";
                return sr;
            }
        }

        public static StatusReport GetComplainReport()
        {
            StatusReport sr = new StatusReport();
            ComplainReport[] crcArr = null;
            DataTable dt = null;
            string sqlString = " SELECT ID, 投诉接待时间, 投诉方式, 投诉人姓名, 地址, 投诉内容, " +
                " 联系电话, 投诉处理单编号, 处理完成日期, 处理完成情况, 登记人, 责任部门, 投诉前照片1, 投诉前照片2, 投诉前照片3, " +
                " 处理后照片1, 处理后照片2, 处理后照片3, 状态, 业主确认解决, 确认时间,  是否满意, 业主评价, 未解决原因, 发单人, " +
                " 接单人, 不受理原因, dbo.资源帐套表.帐套代码,  dbo.资源帐套表.帐套名称," +
                " case when 业主确认解决 = '未解决'then '业主确认未解决' when DATEDIFF(hour, 接单时间, getdate()) >= 48 AND 状态 <> '已完成' AND  状态 <> '无效投诉' then '处理超时' end as 上报原因 " +
                " FROM dbo.基础资料_顾客投诉处理登记表 " +
                " LEFT OUTER JOIN dbo.资源帐套表 ON LEFT(分类, 2) = dbo.资源帐套表.帐套代码 " +
                " where (业主确认解决 = '未解决' ) or ( DATEDIFF(hour, 接单时间, getdate()) >= 48 AND 状态 <> '已完成' AND  状态 <> '无效投诉') " +
                " order by 帐套代码,上报原因 ";
            dt = SQLHelper.ExecuteQuery("wyt", sqlString);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "查询条件错误或没有数据";
                return sr;
            }
            crcArr = GetComplains(dt);
            sr.status = "Success";
            sr.result = "成功";
            sr.data = crcArr;
            return sr;
        }

        private static ComplainReport[] GetComplains(DataTable dt)
        {
            List<ComplainReport> complainList = new List<ComplainReport>();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                ComplainReport complaint = new ComplainReport()
                {
                    Id = DataTypeHelper.GetIntValue(dr["ID"]),
                    ReceptionDate = DataTypeHelper.GetDateStringValue(dr["投诉接待时间"]),
                    Way = DataTypeHelper.GetStringValue(dr["投诉方式"]),
                    Name = DataTypeHelper.GetStringValue(dr["投诉人姓名"]),
                    Address = DataTypeHelper.GetStringValue(dr["地址"]),
                    Content = DataTypeHelper.GetStringValue(dr["投诉内容"]),
                    Phone = DataTypeHelper.GetStringValue(dr["联系电话"]),
                    Number = DataTypeHelper.GetStringValue(dr["投诉处理单编号"]),
                    FinishDate = DataTypeHelper.GetDateStringValue(dr["处理完成日期"]),
                    FinishStatus = DataTypeHelper.GetStringValue(dr["处理完成情况"]),
                    Registrant = DataTypeHelper.GetStringValue(dr["登记人"]),
                    Department = DataTypeHelper.GetStringValue(dr["责任部门"]),
                    Status = DataTypeHelper.GetStringValue(dr["状态"]),
                    IsSatisfying = DataTypeHelper.GetStringValue(dr["是否满意"]),
                    AffirmComplete = DataTypeHelper.GetStringValue(dr["业主确认解决"]),
                    AffirmCompleteTime = DataTypeHelper.GetDateStringValue(dr["确认时间"]),
                    AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(dr["业主评价"]),
                    SendPerson = DataTypeHelper.GetStringValue(dr["发单人"]),
                    ReceivePerson = DataTypeHelper.GetStringValue(dr["接单人"]),
                    ZTCode = DataTypeHelper.GetStringValue(dr["帐套代码"]),
                    ZTName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                    type = DataTypeHelper.GetStringValue(dr["上报原因"])
                };
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片3"]));
                complaint.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片3"]));
                complaint.AfterImage = afterList.ToArray();
                complainList.Add(complaint);
            }
            return complainList.ToArray();
        }



        private static string GetPercent(string value1, string value2)
        {
            decimal number1 = Convert.ToDecimal(value1);
            decimal number2 = Convert.ToDecimal(value2);
            decimal result = 0;
            if (number2 == 0)
            {
                return "0%";
            }
            else
            {
                result = number1 / number2;
                return result.ToString("p2");
            }
        }
    }
}