using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiServer.Models;
using HexiServer.Common;
using HexiUtils;

namespace HexiServer.Business
{
    public class RepairDal
    {
        /// <summary>
        /// 获取满足条件的工单的列表
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="ztcode"></param>
        /// <returns></returns>
        public static StatusReport GetRepairOrder(string userCode, string ztcode, string status, string orderType)
        {
            //string receivePerson = UserHelper.GetUser(openId);
            // select   top   y   *   from   表   where   主键   not   in(select   top   (x-1)*y   主键   from   表) 
            string orderStatusCondition = status == "未完成" ? " and not(isnull(状态,'') = '已完成') " : " and 状态 = '已完成' ";
            StatusReport sr = new StatusReport();
            string sqlString =
                " select top 100 " +
                " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
                " 紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
                " 到场时间,操作人,完成时间,收费类别,材料费,人工费,是否已收,是否阅读,状态,完成情况及所耗物料,报修前照片1," +
                " 报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,延期原因,预计延期到,回访人,回访意见,回访时间, " +
                " 是否满意,业主确认完成,业主确认完成时间,是否满意,业主评价,是否入户,接单人, case when 所属组团 = @接单人 then '客服专员' else '维修工' end as 身份 " +
                " from 小程序_工单管理 " +
                " where 接单人 is not null and (接单人 = @接单人 or 所属组团 = @接单人) and left(分类,2) = @分类";
            sqlString += orderStatusCondition;
            sqlString += (" order by " + orderType + " desc");
            //sqlString += orderType == "已完成" ? " order by 完成时间 desc " : " order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@接单人", userCode),
                new SqlParameter("@分类", ztcode),
                new SqlParameter("@状态", status));

            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }

            //string sqlStr = " select 序号,内容 from 基础资料_服务任务管理设置_入户维修注意事项 where left(DefClass,2) = @分类 ";
            //DataTable dtCaution = SQLHelper.ExecuteQuery("wyt", sqlStr, new SqlParameter("@分类", ztcode));
            //List<RepairCaution> rcList = new List<RepairCaution>();
            //if (dtCaution.Rows.Count != 0)
            //{
            //    foreach (DataRow drCaution in dtCaution.Rows)
            //    {
            //        RepairCaution rc = new RepairCaution();
            //        rc.number = DataTypeHelper.GetStringValue(drCaution["序号"]);
            //        rc.content = DataTypeHelper.GetStringValue(drCaution["内容"]);
            //        rcList.Add(rc);
            //    }
            //}
            
            List<Repair> repairList = new List<Repair>();
            foreach (DataRow row in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                Repair r = new Repair();
                r.Id = DataTypeHelper.GetIntValue(row["ID"]);
                r.SerialNumber = DataTypeHelper.GetStringValue(row["序号"]);
                r.Department = DataTypeHelper.GetStringValue(row["部门"]);
                r.Address = DataTypeHelper.GetStringValue(row["地址"]);
                r.RepairPerson = DataTypeHelper.GetStringValue(row["报修人"]);
                r.PhoneNumber = DataTypeHelper.GetStringValue(row["联系电话"]);
                r.ServiceProject = DataTypeHelper.GetStringValue(row["服务项目"]);
                r.ServiceCategory = DataTypeHelper.GetStringValue(row["服务类别"]);
                r.Level = DataTypeHelper.GetStringValue(row["紧急程度"]);
                r.Identity = DataTypeHelper.GetStringValue(row["身份"]);
                r.NeedIn = DataTypeHelper.GetStringValue(row["是否入户"]);
                r.RepairExplain = DataTypeHelper.GetStringValue(row["报修说明"]);
                r.RepairTime = DataTypeHelper.GetDateStringValue(row["报修时间"]);
                r.OrderTime = DataTypeHelper.GetDateStringValue(row["预约服务时间"]);
                r.VisitTime = DataTypeHelper.GetDateStringValue(row["谈好上门时间"]);
                r.SendPerson = DataTypeHelper.GetStringValue(row["发单人"]);
                r.ReceivePerson = DataTypeHelper.GetStringValue(row["接单人"]);
                r.DispatchTime = DataTypeHelper.GetDateStringValue(row["派工时间"]);
                r.ArriveTime = DataTypeHelper.GetDateStringValue(row["到场时间"]);
                r.OperatePerson = DataTypeHelper.GetStringValue(row["操作人"]);
                r.CompleteTime = DataTypeHelper.GetDateStringValue(row["完成时间"]);
                r.ChargeType = DataTypeHelper.GetStringValue(row["收费类别"]);
                r.MaterialExpense = DataTypeHelper.GetDoubleValue(row["材料费"]);
                r.LaborExpense = DataTypeHelper.GetDoubleValue(row["人工费"]);
                r.IsPaid = DataTypeHelper.GetStringValue(row["是否已收"]);
                r.IsRead = DataTypeHelper.GetIntValue(row["是否阅读"]);
                r.AffirmComplete = DataTypeHelper.GetStringValue(row["业主确认完成"]);
                r.AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(row["业主评价"]);
                r.AffirmCompleteTime = DataTypeHelper.GetDateStringValue(row["业主确认完成时间"]);
                r.IsSatisfying = DataTypeHelper.GetStringValue(row["是否满意"]);
                r.CallBackEvaluation = DataTypeHelper.GetStringValue(row["回访意见"]);
                r.CallBackPerson = DataTypeHelper.GetStringValue(row["回访人"]);
                r.CallBackTime = DataTypeHelper.GetDateStringValue(row["回访时间"]);
                r.status = DataTypeHelper.GetStringValue(row["状态"]);
                r.status = string.IsNullOrEmpty(r.AffirmComplete) ? r.status : "业主已确认";
                r.status = string.IsNullOrEmpty(r.CallBackPerson) ? r.status : "已回访";
                r.CompleteStatus = DataTypeHelper.GetStringValue(row["完成情况及所耗物料"]);
                r.LateTime = DataTypeHelper.GetDateStringValue(row["预计延期到"]);
                r.LateReason = DataTypeHelper.GetStringValue(row["延期原因"]);
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片3"]));
                r.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片3"]));
                r.AfterImage = afterList.ToArray();
                //r.Cautions = rcList.ToArray();
                repairList.Add(r);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = repairList.ToArray();

            return sr;
        }

        /// <summary>
        /// 更新库中的工单记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="arriveTime"></param>
        /// <param name="completeTime"></param>
        /// <param name="completeStatus"></param>
        /// <param name="laborExpense"></param>
        /// <param name="materialExpense"></param>
        /// /// <param name="status"></param>
        /// <returns></returns>
        public static StatusReport SetRepairOrder(string id, string arriveTime, string completeTime, string completeStatus, string chargeType, string laborExpense, string materialExpense,string status, string lateReason,string lateTime, string isPaid)
        {
            StatusReport sr = new StatusReport();
            string sqlString = 
                "update 基础资料_服务任务管理 " +
                "set " +
                "到场时间 = @到场时间, " +
                "完成时间 = @完成时间, " +
                "完成情况及所耗物料 = @完成情况及所耗物料, " +
                "状态 = @状态, " +
                "收费类别 = @收费类别, " +
                "人工费 = @人工费, " +
                "材料费 = @材料费, " +
                "合计 = @合计, " +
                "是否已收 = @是否已收, " +
                "延期原因 = @延期原因, " +
                "预计延期到 = @预计延期到 " +
                "where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@到场时间", GetDBValue(arriveTime)),
                new SqlParameter("@完成时间", GetDBValue(completeTime)),
                new SqlParameter("@完成情况及所耗物料", GetDBValue(completeStatus)),
                new SqlParameter("@状态", GetDBValue(status)),
                new SqlParameter("@延期原因", GetDBValue(lateReason)),
                new SqlParameter("@预计延期到", GetDBValue(lateTime)),
                new SqlParameter("@收费类别", GetDBValue(chargeType)),
                new SqlParameter("@人工费",  laborExpense == "" ? 0 : Convert.ToDecimal(laborExpense)),
                new SqlParameter("@材料费", materialExpense == "" ? 0 : Convert.ToDecimal(materialExpense)),
                new SqlParameter("@合计", laborExpense == "" ? 0 : Convert.ToDecimal(laborExpense) + materialExpense == "" ? 0 : Convert.ToDecimal(materialExpense)),
                new SqlParameter("@是否已收", GetDBValue(isPaid)),
                new SqlParameter("@ID", Convert.ToInt32(id)));
                
            return sr;
        }

        /// <summary>
        /// 设置工单状态为已读
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static StatusReport SetOrderIsRead (string id)
        {
            string sqlstring = "update 基础资料_服务任务管理 set 是否阅读 = 1 where ID = @ID";
            StatusReport sr = SQLHelper.Update("wyt", sqlstring, new SqlParameter("@ID", id));
            return sr;
        }

        /// <summary>
        /// 将小程序发送的报事信息添加到库中
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <param name="detail"></param>
        /// <param name="classify"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static StatusReport SetPatrol (string name, string address, string detail, string classify, string time)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "insert into 基础资料_服务任务管理 (报修人,地址,服务项目,分类,报修时间,状态,报修来源) " +
                " select @报修人,@地址,@服务项目,@分类,@报修时间,@状态,@报修来源 " +
                " SELECT @@IDENTITY";
            sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@报修人", GetDBValue(name)),
                new SqlParameter("@地址", GetDBValue(address)),
                new SqlParameter("@服务项目", GetDBValue(detail)),
                new SqlParameter("@分类", GetDBValue(classify)),
                new SqlParameter("@报修时间", GetDBValue(time)),
                new SqlParameter("@状态", "未受理"),
                new SqlParameter("@报修来源", "小程序报事"));
            return sr;
        }

        /// <summary>
        /// 获取报事信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classify"></param>
        /// <returns></returns>
        public static StatusReport GetPatrol (string name, string classify)
        {
            StatusReport sr = new StatusReport();
            string sqlString =
                " select top 10 " +
                " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
                " 紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
                " 到场时间,操作人,完成时间,收费类别,材料费,人工费,是否已收,是否阅读,状态,完成情况及所耗物料,报修前照片1," +
                " 报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,延期原因,预计延期到,回访人,回访意见,回访时间, " +
                " 是否满意,业主确认完成,业主确认完成时间,是否满意,业主评价 " +
                " from 小程序_工单管理 " +
                " where 报修人 = @报修人 and left(分类,2) = @分类 and 报修来源 = @报修来源 " +
                " order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@报修人", name),
                new SqlParameter("@分类", classify),
                new SqlParameter("@报修来源", "小程序报事"));

            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }

            List<Repair> repairList = new List<Repair>();
            foreach (DataRow row in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                Repair r = new Repair();
                r.Id = DataTypeHelper.GetIntValue(row["ID"]);
                r.SerialNumber = DataTypeHelper.GetStringValue(row["序号"]);
                r.Department = DataTypeHelper.GetStringValue(row["部门"]);
                r.Address = DataTypeHelper.GetStringValue(row["地址"]);
                r.RepairPerson = DataTypeHelper.GetStringValue(row["报修人"]);
                r.PhoneNumber = DataTypeHelper.GetStringValue(row["联系电话"]);
                r.ServiceProject = DataTypeHelper.GetStringValue(row["服务项目"]);
                r.ServiceCategory = DataTypeHelper.GetStringValue(row["服务类别"]);
                r.Level = DataTypeHelper.GetStringValue(row["紧急程度"]);
                r.RepairExplain = DataTypeHelper.GetStringValue(row["报修说明"]);
                r.RepairTime = DataTypeHelper.GetDateStringValue(row["报修时间"]);
                r.OrderTime = DataTypeHelper.GetDateStringValue(row["预约服务时间"]);
                r.VisitTime = DataTypeHelper.GetDateStringValue(row["谈好上门时间"]);
                r.SendPerson = DataTypeHelper.GetStringValue(row["发单人"]);
                r.ReceivePerson = DataTypeHelper.GetStringValue(row["接单人"]);
                r.DispatchTime = DataTypeHelper.GetDateStringValue(row["派工时间"]);
                r.ArriveTime = DataTypeHelper.GetDateStringValue(row["到场时间"]);
                r.OperatePerson = DataTypeHelper.GetStringValue(row["操作人"]);
                r.CompleteTime = DataTypeHelper.GetDateStringValue(row["完成时间"]);
                r.ChargeType = DataTypeHelper.GetStringValue(row["收费类别"]);
                r.MaterialExpense = DataTypeHelper.GetDoubleValue(row["材料费"]);
                r.LaborExpense = DataTypeHelper.GetDoubleValue(row["人工费"]);
                r.IsPaid = DataTypeHelper.GetStringValue(row["是否已收"]);
                r.IsRead = DataTypeHelper.GetIntValue(row["是否阅读"]);
                r.AffirmComplete = DataTypeHelper.GetStringValue(row["业主确认完成"]);
                r.AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(row["是否满意"]);
                r.AffirmCompleteTime = DataTypeHelper.GetDateStringValue(row["业主确认完成时间"]);
                r.IsSatisfying = DataTypeHelper.GetStringValue(row["是否满意"]);
                r.CallBackEvaluation = DataTypeHelper.GetStringValue(row["回访意见"]);
                r.CallBackPerson = DataTypeHelper.GetStringValue(row["回访人"]);
                r.CallBackTime = DataTypeHelper.GetDateStringValue(row["回访时间"]);
                r.status = DataTypeHelper.GetStringValue(row["状态"]);
                r.status = string.IsNullOrEmpty(r.AffirmComplete) ? r.status : "业主已确认";
                r.status = string.IsNullOrEmpty(r.CallBackPerson) ? r.status : "已回访";
                r.CompleteStatus = DataTypeHelper.GetStringValue(row["完成情况及所耗物料"]);
                r.LateTime = DataTypeHelper.GetDateStringValue(row["预计延期到"]);
                r.LateReason = DataTypeHelper.GetStringValue(row["延期原因"]);
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片3"]));
                r.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片3"]));
                r.AfterImage = afterList.ToArray();
                repairList.Add(r);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = repairList.ToArray();
            return sr;
        }

        /// <summary>
        /// 设置工单上传的照片
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="func"></param>
        /// <param name="index"></param>
        /// <param name="sqlImagePath"></param>
        /// <returns></returns>
        public static StatusReport SetRepairImage(string ID, string func, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = func == "before" ? "报修前照片" + index.ToString() : "处理后照片" + index.ToString() ;
            string sqlString = " update 基础资料_服务任务管理 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }


        /// <summary>
        /// 设置报事上传的图片
        /// </summary>
        /// <param name="func"></param>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="sqlImagePath"></param>
        /// <returns></returns>
        public static StatusReport SetPatrolImage(string func,string id, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = "报修前照片" + index.ToString();
            string sqlString = " update 基础资料_服务任务管理 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", id));
            sr.parameters = index;
            return sr;
        }

        /// <summary>
        /// 设置业主评价信息
        /// </summary>
        /// <param name="evaluation"></param>
        /// <param name="isSatisfying"></param>
        /// <param name="id"></param>
        /// <param name="evaluationTime"></param>
        /// <param name="evaluationer"></param>
        /// <returns></returns>
        //public static StatusReport Evaluation(string evaluation, string isSatisfying, string id, string evaluationTime, string evaluationer)
        //{
        //    StatusReport sr = new StatusReport();
        //    string sqlString = "update 基础资料_服务任务管理 " +
        //        " set 是否满意 = @是否满意," +
        //        " 回访意见 = @回访意见 " +
        //        " 回访人 = @回访人 " +
        //        " 回访时间 = @回访时间 " +
        //        "where ID = @ID";
        //    sr = SQLHelper.Update("wyt", sqlString,
        //        new SqlParameter("@是否满意", isSatisfying),
        //        new SqlParameter("@回访意见", evaluation),
        //        new SqlParameter("@回访人", evaluation),
        //        new SqlParameter("@回访时间", evaluation),
        //        new SqlParameter("@ID", id));
        //    return sr;
        //}


        public static StatusReport GetRepairStatistics(string ztcode, string level, string userName,string before)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " SELECT " +
                               " 接单人, " +
                               " 帐套名称, " +
                               " COUNT(CASE WHEN isnull(状态, '') = '' THEN NULL ELSE ID END) AS 接单数, " +
                               " COUNT(CASE WHEN isnull(状态, '') = '已完成' THEN ID ELSE NULL END) AS 完成数, " +
                               " COUNT(CASE WHEN isnull(状态, '') <> '' AND isnull(状态, '') <> '已完成' THEN ID ELSE NULL END) AS 未完成数, " +
                               " COUNT(CASE WHEN 是否满意 = '非常满意' THEN ID ELSE NULL END) AS 非常满意数, " +
                               " COUNT(CASE WHEN 是否满意 = '满意' THEN ID ELSE NULL END) AS 满意数, " +
                               " COUNT(CASE WHEN 是否满意 = '不满意' THEN ID ELSE NULL END) AS 不满意数, " +
                               " COUNT(是否满意) AS 评价总数 " +
                               " FROM dbo.小程序_工单管理 ";
            string condition = "";
            string group = "";
            string order = "";
            switch (level)
            {
                case "一线":
                    {
                        condition = "where 接单人 = @接单人 and 帐套代码 = @帐套代码 " +
                                    " and left(CONVERT(varchar(10),报修时间,112),6) >= left(CONVERT(varchar(10),dateadd(month,@before,GETDATE()),112),6) ";
                        group = " group by 帐套名称,接单人 ";
                        order = " order by 帐套名称,接单人 ";
                        sqlString += condition + group + order;
                        DataTable dt = null;
                        dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                            new SqlParameter("@接单人", userName),
                            new SqlParameter("@帐套代码", ztcode),
                            new SqlParameter("@before", -Convert.ToInt32(before)));
                        if (dt.Rows.Count == 0)
                        {
                            sr.status = "fail";
                            sr.result = "未查询到任何数据";
                            return sr;
                        }
                        DataRow dr = dt.Rows[0];
                        RepairStatisticsPersonal rs = new RepairStatisticsPersonal()
                        {
                            name = DataTypeHelper.GetStringValue(dr["接单人"]),
                            ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                            countReceive = Convert.ToString(dr["接单数"]),
                            countFinished = Convert.ToString(dr["完成数"]),
                            countUnfinished = Convert.ToString(dr["未完成数"]),
                            countVerySatisfy = Convert.ToString(dr["非常满意数"]),
                            countSatisfy = Convert.ToString(dr["满意数"]),
                            countUnsatisfy = Convert.ToString(dr["不满意数"]),
                            countEvaluation = Convert.ToString(dr["评价总数"])
                    };
                        //string countEvaluation = Convert.ToString(dr["评价总数"]);
                        rs.rateFinish = GetPercent(rs.countFinished, rs.countReceive);
                        rs.rateUnfinish = GetPercent(rs.countUnfinished, rs.countReceive);
                        rs.rateVerySatisfy = GetPercent(rs.countVerySatisfy, rs.countEvaluation);
                        rs.rateSatisfy = GetPercent(rs.countSatisfy, rs.countEvaluation);
                        rs.rateUnsatisfy = GetPercent(rs.countUnsatisfy, rs.countEvaluation);
                        sr.status = "Success";
                        sr.result = "成功";
                        sr.data = rs;
                        sr.parameters = sqlString;
                    }
                    break;
                case "助理":
                case "项目经理":
                    {
                        condition = "where 帐套代码 = @帐套代码 " +
                                   " and left(CONVERT(varchar(10),报修时间,112),6) >= left(CONVERT(varchar(10),dateadd(month,@before,GETDATE()),112),6) ";
                        group = " group by 帐套名称,接单人 ";
                        order = " order by 帐套名称,接单人 ";
                        sqlString += condition + group + order;
                        DataTable dt = null;
                        dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                            new SqlParameter("@帐套代码", ztcode),
                            new SqlParameter("@before", -Convert.ToInt32(before)));
                        if (dt.Rows.Count == 0)
                        {
                            sr.status = "fail";
                            sr.result = "未查询到任何数据";
                            return sr;
                        }
                        //string count = "0";
                        RepairStatisticsProject rsproject = new RepairStatisticsProject();
                        List<RepairStatisticsPersonal> rspList = new List<RepairStatisticsPersonal>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            RepairStatisticsPersonal rs = new RepairStatisticsPersonal()
                            {
                                name = DataTypeHelper.GetStringValue(dr["接单人"]),
                                ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                                countReceive = Convert.ToString(dr["接单数"]),
                                countFinished = Convert.ToString(dr["完成数"]),
                                countUnfinished = Convert.ToString(dr["未完成数"]),
                                countVerySatisfy = Convert.ToString(dr["非常满意数"]),
                                countSatisfy = Convert.ToString(dr["满意数"]),
                                countUnsatisfy = Convert.ToString(dr["不满意数"]),
                                countEvaluation = Convert.ToString(dr["评价总数"])
                            };
                            //string countEvaluation = Convert.ToString(dr["评价总数"]);
                            rs.rateFinish = GetPercent(rs.countFinished, rs.countReceive);
                            rs.rateUnfinish = GetPercent(rs.countUnfinished, rs.countReceive);
                            rs.rateVerySatisfy = GetPercent(rs.countVerySatisfy, rs.countEvaluation);
                            rs.rateSatisfy = GetPercent(rs.countSatisfy, rs.countEvaluation);
                            rs.rateUnsatisfy = GetPercent(rs.countUnsatisfy, rs.countEvaluation);
                            rspList.Add(rs);
                            rsproject.ztName = rs.ztName;
                            rsproject.countReceive = Convert.ToString(Convert.ToDecimal(rsproject.countReceive) + Convert.ToDecimal(rs.countReceive));
                            rsproject.countFinished = Convert.ToString(Convert.ToDecimal(rsproject.countFinished) + Convert.ToDecimal(rs.countFinished));
                            rsproject.countUnfinished = Convert.ToString(Convert.ToDecimal(rsproject.countUnfinished) + Convert.ToDecimal(rs.countUnfinished));
                            rsproject.countVerySatisfy = Convert.ToString(Convert.ToDecimal(rsproject.countVerySatisfy) + Convert.ToDecimal(rs.countVerySatisfy));
                            rsproject.countSatisfy = Convert.ToString(Convert.ToDecimal(rsproject.countSatisfy) + Convert.ToDecimal(rs.countSatisfy));
                            rsproject.countUnsatisfy = Convert.ToString(Convert.ToDecimal(rsproject.countUnsatisfy) + Convert.ToDecimal(rs.countUnsatisfy));
                            rsproject.countEvaluation = Convert.ToString(Convert.ToDecimal(rsproject.countEvaluation) + Convert.ToDecimal(rs.countEvaluation));
                        }
                        rsproject.rateFinish = GetPercent(rsproject.countFinished, rsproject.countReceive);
                        rsproject.rateUnfinish = GetPercent(rsproject.countUnfinished, rsproject.countReceive);
                        rsproject.rateVerySatisfy = GetPercent(rsproject.countVerySatisfy, rsproject.countEvaluation);
                        rsproject.rateSatisfy = GetPercent(rsproject.countSatisfy, rsproject.countEvaluation);
                        rsproject.rateUnsatisfy = GetPercent(rsproject.countUnsatisfy, rsproject.countEvaluation);
                        rsproject.repairStatisticsPersonal = rspList.ToArray();
                        sr.status = "Success";
                        sr.result = "成功";
                        sr.data = rsproject;
                        sr.parameters = sqlString;
                    }
                    break;
                case "公司":
                    {
                        sqlString = " SELECT " +
                                  " 帐套名称, " +
                                  " COUNT(CASE WHEN isnull(状态, '') = '' THEN NULL ELSE ID END) AS 接单数, " +
                                  " COUNT(CASE WHEN isnull(状态, '') = '已完成' THEN ID ELSE NULL END) AS 完成数, " +
                                  " COUNT(CASE WHEN isnull(状态, '') <> '' AND isnull(状态, '') <> '已完成' THEN ID ELSE NULL END) AS 未完成数, " +
                                  " COUNT(CASE WHEN 是否满意 = '非常满意' THEN ID ELSE NULL END) AS 非常满意数, " +
                                  " COUNT(CASE WHEN 是否满意 = '满意' THEN ID ELSE NULL END) AS 满意数, " +
                                  " COUNT(CASE WHEN 是否满意 = '不满意' THEN ID ELSE NULL END) AS 不满意数, " +
                                  " COUNT(是否满意) AS 评价总数 " +
                                  " FROM dbo.小程序_工单管理 ";
                        condition = " where left(CONVERT(varchar(10),报修时间,112),6) >=left(CONVERT(varchar(10),dateadd(month,@before,GETDATE()),112),6) ";
                        group = " group by 帐套名称 ";
                        order = " order by 帐套名称 ";
                        sqlString += condition + group + order;
                        DataTable dt = null;
                        dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                            new SqlParameter("@before", -Convert.ToInt32(before)));
                        if (dt.Rows.Count == 0)
                        {
                            sr.status = "fail";
                            sr.result = "未查询到任何数据";
                            return sr;
                        }
                        //string count = "0";
                        RepairStatisticsCompany rsc = new RepairStatisticsCompany();
                        List<RepairStatisticsProject> rspList = new List<RepairStatisticsProject>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            RepairStatisticsProject rsp = new RepairStatisticsProject()
                            {
                                ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]),
                                countReceive = Convert.ToString(dr["接单数"]),
                                countFinished = Convert.ToString(dr["完成数"]),
                                countUnfinished = Convert.ToString(dr["未完成数"]),
                                countVerySatisfy = Convert.ToString(dr["非常满意数"]),
                                countSatisfy = Convert.ToString(dr["满意数"]),
                                countUnsatisfy = Convert.ToString(dr["不满意数"]),
                                countEvaluation = Convert.ToString(dr["评价总数"])
                        };
                            //string countEvaluation = Convert.ToString(dr["评价总数"]);
                            rsp.rateFinish = GetPercent(rsp.countFinished, rsp.countReceive);
                            rsp.rateUnfinish = GetPercent(rsp.countUnfinished, rsp.countReceive);
                            rsp.rateVerySatisfy = GetPercent(rsp.countVerySatisfy, rsp.countEvaluation);
                            rsp.rateSatisfy = GetPercent(rsp.countSatisfy, rsp.countEvaluation);
                            rsp.rateUnsatisfy = GetPercent(rsp.countUnsatisfy, rsp.countEvaluation);
                            rspList.Add(rsp);
                            rsc.ztName = rsp.ztName;
                            rsc.countReceive = Convert.ToString(Convert.ToDecimal(rsc.countReceive) + Convert.ToDecimal(rsp.countReceive));
                            rsc.countFinished = Convert.ToString(Convert.ToDecimal(rsc.countFinished) + Convert.ToDecimal(rsp.countFinished));
                            rsc.countUnfinished = Convert.ToString(Convert.ToDecimal(rsc.countUnfinished) + Convert.ToDecimal(rsp.countUnfinished));
                            rsc.countVerySatisfy = Convert.ToString(Convert.ToDecimal(rsc.countVerySatisfy) + Convert.ToDecimal(rsp.countVerySatisfy));
                            rsc.countSatisfy = Convert.ToString(Convert.ToDecimal(rsc.countSatisfy) + Convert.ToDecimal(rsp.countSatisfy));
                            rsc.countUnsatisfy = Convert.ToString(Convert.ToDecimal(rsc.countUnsatisfy) + Convert.ToDecimal(rsp.countUnsatisfy));
                            rsc.countEvaluation = Convert.ToString(Convert.ToDecimal(rsc.countEvaluation) + Convert.ToDecimal(rsp.countEvaluation));
                        }
                        rsc.rateFinish = GetPercent(rsc.countFinished, rsc.countReceive);
                        rsc.rateUnfinish = GetPercent(rsc.countUnfinished, rsc.countReceive);
                        rsc.rateVerySatisfy = GetPercent(rsc.countVerySatisfy, rsc.countEvaluation);
                        rsc.rateSatisfy = GetPercent(rsc.countSatisfy, rsc.countEvaluation);
                        rsc.rateUnsatisfy = GetPercent(rsc.countUnsatisfy, rsc.countEvaluation);
                        rsc.repairStatisticsProject = rspList.ToArray();
                        sr.status = "Success";
                        sr.result = "成功";
                        sr.data = rsc;
                        sr.parameters = sqlString;
                    }
                    break;
            }

            return sr;
             
        }
        public static StatusReport GetRepairReport(string ztcode, string level)
        {
            StatusReport sr = new StatusReport();

            string sqlString = "";
            string condition = "";
            if (level == "助理")
            {
                DataTable dt = null;
                List<RepairReport> repairList = null;
                //List<RepairReportProject> rrpList = new List<RepairReportProject>();
                //RepairReportProject rrp = null;
                sqlString =
                " select " +
                " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
                " 紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
                " 到场时间,操作人,完成时间,收费类别,材料费,人工费,是否已收,是否阅读,状态,完成情况及所耗物料,报修前照片1," +
                " 报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,延期原因,预计延期到,回访人,回访意见,回访时间, " +
                " 是否满意,业主确认完成,业主确认完成时间,是否满意,业主评价,是否入户, " +
                " case " +
                " when isnull(完成时间,'') = '' and left(CONVERT(varchar(10),getdate(),112),8) = left(CONVERT(varchar(10),报修时间,112),8)  and cast(left(CONVERT(varchar(20),getdate(),114),2) as int) >= 17 then '当日未完成' " +
                " when  ((预估完成时间 is not null and isnull(完成时间,'') = '' and DATEDIFF (hour, 预估完成时间, getdate()) >= 0) or (预估完成时间 is not null  and DATEDIFF (hour, 预估完成时间, 完成时间) > 0 and DATEDIFF (hour, 预估完成时间, getdate()) >= 0)) then '超过预估完成时间' " +
                " end " +
                " as 上报原因 " +
                " from 小程序_工单管理 where ((isnull(完成时间,'') = '' " +
                " and left(CONVERT(varchar(10),getdate(),112),8) = left(CONVERT(varchar(10),报修时间,112),8) " +
                " and cast(left(CONVERT(varchar(20),getdate(),114),2) as int) >= 17 ) " +
                " or (((预估完成时间 is not null and isnull(完成时间,'') = '' " +
                " and DATEDIFF (hour, 预估完成时间, getdate()) >= 0) " +
                " or (预估完成时间 is not null " +
                " and DATEDIFF (hour, 预估完成时间, 完成时间) > 0 " +
                " and DATEDIFF (hour, 预估完成时间, getdate()) >= 0)))) " +
                " and 帐套代码 = @帐套代码";
                dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到任何记录";
                    return sr;
                }
                repairList = GetRepairReport(dt);
                sr.status = "Success";
                sr.result = "成功";
                sr.data = repairList.ToArray();
                return sr;
            }
            else
            {
                sqlString =
                " select " +
                " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
                " 紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
                " 到场时间,操作人,完成时间,收费类别,材料费,人工费,是否已收,是否阅读,状态,完成情况及所耗物料,报修前照片1," +
                " 报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,延期原因,预计延期到,回访人,回访意见,回访时间, " +
                " 是否满意,业主确认完成,业主确认完成时间,是否满意,业主评价,是否入户, " +
                " case when 是否满意 = '不满意' then '业主确认不满意' end as 上报原因 " +
                " from 小程序_工单管理 " +
                " where 是否满意 = '不满意' " +
                " and 帐套代码 = @帐套代码";
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                                new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到任何数据";
                    return sr;
                }
                List<RepairReport> repairList = GetRepairReport(dt);
                sr.status = "Success";
                sr.result = "成功";
                sr.data = repairList.ToArray();
                return sr;
            }
        }

        //public static StatusReport GetRepairReport1(string ztcode, string level)
        //{
        //    StatusReport sr = new StatusReport();
        //    string sqlStr =
        //        " select " +
        //        " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
        //        " 紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
        //        " 到场时间,操作人,完成时间,收费类别,材料费,人工费,是否已收,是否阅读,状态,完成情况及所耗物料,报修前照片1," +
        //        " 报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,延期原因,预计延期到,回访人,回访意见,回访时间, " +
        //        " 是否满意,业主确认完成,业主确认完成时间,是否满意,业主评价,是否入户 " +
        //        " from 小程序_工单管理 ";
        //    string sqlString = "";
        //    string condition = "";
        //    if (level == "助理")
        //    {
        //        DataTable dt = null;
        //        List<Repair> repairList = null;
        //        List<RepairReportProject> rrpList = new List<RepairReportProject>();
        //        RepairReportProject rrp = null;
        //        condition = " where 帐套代码 = @帐套代码 and isnull(完成时间,'') = '' " +
        //            " and left(CONVERT(varchar(10),getdate(),112),8) = left(CONVERT(varchar(10),报修时间,112),8) " +
        //            " and cast(left(CONVERT(varchar(20),getdate(),114),2) as int) >= 17 ";//当日未完工
        //        sqlString = sqlStr + condition;
        //        dt = SQLHelper.ExecuteQuery("wyt", sqlString,
        //        new SqlParameter("@帐套代码", ztcode));

        //        rrp = new RepairReportProject();
        //        rrp.type = "当日未完成";
        //        if (dt.Rows.Count != 0)
        //        {
        //            rrp = new RepairReportProject();
        //            rrp.type = "当日未完成";
        //            repairList = GetRepair(dt);
        //            rrp.repairs = repairList.ToArray();
                   
        //        }
        //        rrpList.Add(rrp);

        //        condition = " where " +
        //            "  帐套代码 = @帐套代码 and ((预估完成时间 is not null " +
        //            " and isnull(完成时间,'') = '' " +
        //            " and DATEDIFF (hour, 预估完成时间, getdate()) >= 0) " +
        //            " or (预估完成时间 is not null " +
        //            " and DATEDIFF (hour, 预估完成时间, 完成时间) > 0" +
        //            " and DATEDIFF (hour, 预估完成时间, getdate()) >= 0)) ";//超过预估完成时间
        //        sqlString = sqlStr + condition;
        //        dt = SQLHelper.ExecuteQuery("wyt", sqlString,
        //        new SqlParameter("@帐套代码", ztcode));
        //        rrp = new RepairReportProject();
        //        rrp.type = "超过预估完成时间";
        //        if (dt.Rows.Count != 0)
        //        {
        //            repairList = GetRepair(dt);
        //            rrp.repairs = repairList.ToArray();
        //        }
        //        rrpList.Add(rrp);
        //        sr.status = "success";
        //        sr.result = "成功";
        //        sr.data = rrpList.ToArray();
        //        return sr;
        //    }
        //    else
        //    {
        //        condition = " where  帐套代码 = @帐套代码 and  是否满意 = '不满意' ";
        //        RepairReportCompany rrc = new RepairReportCompany();
        //        rrc.type = "业主不满意";
        //        sqlString = sqlStr + condition;
        //        DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
        //                        new SqlParameter("@帐套代码", ztcode));
        //        if (dt.Rows.Count == 0)
        //        {
        //            sr.status = "Fail";
        //            sr.result = "未查询到任何数据";
        //            return sr;
        //        }
        //        List<Repair> repairList = GetRepair(dt);
        //        rrc.repairs = repairList.ToArray();
        //        sr.status = "Success";
        //        sr.result = "成功";
        //        sr.data = rrc;
        //        return sr;
        //    }
        //}

        private static List<RepairReport> GetRepairReport(DataTable dt)
        {
            List<RepairReport> repairList = new List<RepairReport>();
            foreach (DataRow row in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                RepairReport r = new RepairReport();
                r.Id = DataTypeHelper.GetIntValue(row["ID"]);
                r.SerialNumber = DataTypeHelper.GetStringValue(row["序号"]);
                r.Department = DataTypeHelper.GetStringValue(row["部门"]);
                r.Address = DataTypeHelper.GetStringValue(row["地址"]);
                r.RepairPerson = DataTypeHelper.GetStringValue(row["报修人"]);
                r.PhoneNumber = DataTypeHelper.GetStringValue(row["联系电话"]);
                r.ServiceProject = DataTypeHelper.GetStringValue(row["服务项目"]);
                r.ServiceCategory = DataTypeHelper.GetStringValue(row["服务类别"]);
                r.Level = DataTypeHelper.GetStringValue(row["紧急程度"]);
                //r.Identity = DataTypeHelper.GetStringValue(row["身份"]);
                r.NeedIn = DataTypeHelper.GetStringValue(row["是否入户"]);
                r.RepairExplain = DataTypeHelper.GetStringValue(row["报修说明"]);
                r.RepairTime = DataTypeHelper.GetDateStringValue(row["报修时间"]);
                r.OrderTime = DataTypeHelper.GetDateStringValue(row["预约服务时间"]);
                r.VisitTime = DataTypeHelper.GetDateStringValue(row["谈好上门时间"]);
                r.SendPerson = DataTypeHelper.GetStringValue(row["发单人"]);
                r.ReceivePerson = DataTypeHelper.GetStringValue(row["接单人"]);
                r.DispatchTime = DataTypeHelper.GetDateStringValue(row["派工时间"]);
                r.ArriveTime = DataTypeHelper.GetDateStringValue(row["到场时间"]);
                r.OperatePerson = DataTypeHelper.GetStringValue(row["操作人"]);
                r.CompleteTime = DataTypeHelper.GetDateStringValue(row["完成时间"]);
                r.ChargeType = DataTypeHelper.GetStringValue(row["收费类别"]);
                r.MaterialExpense = DataTypeHelper.GetDoubleValue(row["材料费"]);
                r.LaborExpense = DataTypeHelper.GetDoubleValue(row["人工费"]);
                r.IsPaid = DataTypeHelper.GetStringValue(row["是否已收"]);
                r.IsRead = DataTypeHelper.GetIntValue(row["是否阅读"]);
                r.AffirmComplete = DataTypeHelper.GetStringValue(row["业主确认完成"]);
                r.AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(row["业主评价"]);
                r.AffirmCompleteTime = DataTypeHelper.GetDateStringValue(row["业主确认完成时间"]);
                r.IsSatisfying = DataTypeHelper.GetStringValue(row["是否满意"]);
                r.CallBackEvaluation = DataTypeHelper.GetStringValue(row["回访意见"]);
                r.CallBackPerson = DataTypeHelper.GetStringValue(row["回访人"]);
                r.CallBackTime = DataTypeHelper.GetDateStringValue(row["回访时间"]);
                r.status = DataTypeHelper.GetStringValue(row["状态"]);
                r.status = string.IsNullOrEmpty(r.AffirmComplete) ? r.status : "业主已确认";
                r.status = string.IsNullOrEmpty(r.CallBackPerson) ? r.status : "已回访";
                r.CompleteStatus = DataTypeHelper.GetStringValue(row["完成情况及所耗物料"]);
                r.LateTime = DataTypeHelper.GetDateStringValue(row["预计延期到"]);
                r.type = DataTypeHelper.GetStringValue(row["上报原因"]);
                r.LateReason = DataTypeHelper.GetStringValue(row["延期原因"]);
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片3"]));
                r.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片3"]));
                r.AfterImage = afterList.ToArray();
                repairList.Add(r);
            }

            return repairList;
        }

        private static Object GetDBValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
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

        //private static string GetUser(string openid)
        //{
        //    string sqlString =
        //        "select UserCode " +
        //        "from 用户 " +
        //        "where ID in (select 用户表Id from 基础资料_微信员工绑定表 where OpenId = @OpenId)";
        //    DataTable dt = SQLHelper.ExecuteQuery(sqlString,
        //        new SqlParameter("@OpenId", openid));
        //    DataRow dr = dt.Rows[0];

        //    return (string)dr["UserCode"];
        //}
    }
}






//sqlString = "SELECT " +
//"接单人, " +
//"帐套名称, " +
//"COUNT(CASE WHEN isnull(状态, '') = '' THEN NULL ELSE ID END) AS 接单数, " +
//"COUNT(CASE WHEN isnull(状态, '')  = '已完成' THEN ID ELSE NULL END) AS 完成数, " +
//"COUNT(CASE WHEN isnull(状态, '') <> '' AND isnull(状态, '') <> '已完成' THEN ID ELSE NULL END) AS 未完成数, " +
//"COUNT(CASE WHEN 是否满意 = '非常满意' THEN ID ELSE NULL END) AS 非常满意数, " +
//"COUNT(CASE WHEN 是否满意 = '满意' THEN ID ELSE NULL END) AS 满意数, " +
//"COUNT(CASE WHEN 是否满意 = '不满意' THEN ID ELSE NULL END) AS 不满意数, " +
//"COUNT(是否满意) AS 评价总数 " +
//"FROM dbo.小程序_工单管理 " +
//"where 帐套代码 = '01' and left(CONVERT(varchar(10),报修时间,112),6) = '201807' " +
//"group by 帐套名称,接单人 " +
//"order by 帐套名称,接单人 ";