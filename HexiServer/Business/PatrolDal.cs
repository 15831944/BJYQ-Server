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
    public class PatrolDal
    {


        public static StatusReport SetPatrol(string classify, string ztName, string name, string phone, string address, string content)
        {
            StatusReport sr = new StatusReport();
            string sqlString =
               " insert into 基础资料_报事管理 (分类,所属小区,上报人,上报时间,联系电话,上报地址,上报内容) " +
               " select @分类,@所属小区,@上报人,@上报时间,@联系电话,@上报地址,@上报内容 " +
               " select @@identity ";
            sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@分类", GetDBValue(classify)),
                new SqlParameter("@所属小区", GetDBValue(ztName)),
                new SqlParameter("@上报人", GetDBValue(name)),
                new SqlParameter("@上报时间", DateTime.Now),
                new SqlParameter("@联系电话", GetDBValue(phone)),
                new SqlParameter("@上报地址", GetDBValue(address)),
                new SqlParameter("@上报内容", GetDBValue(content)));
            return sr;
        }

        public static StatusReport SetPatrolImage(string id, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = "上报照片" + index.ToString();
            string sqlString = " update 基础资料_报事管理 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", id));
            sr.parameters = index;
            return sr;
        }


        //public static StatusReport GetPatrol(string userCode, string ztcode)
        //{
        //    string orderStatusCondition = status == "未完成" ? " and not(isnull(状态,'') = '已完成') " : " and 状态 = '已完成' ";
        //    StatusReport sr = new StatusReport();
        //    string sqlString =
        //        " SELECT TOP 100 " +
        //        " ID,部门,序号,地址,报修人,联系电话,服务项目,发单人,接单人,报修时间,派工时间,预约服务时间, " +
        //        " 完成情况及所耗物料,操作人,完成时间,材料费,人工费,合计,收费类别,主管意见,服务台签字,客户意见, " +
        //        " 目录显示,回访时间,回访意见,回访人,到场时间,状态,是否阅读,报修前照片1,报修前照片2,报修前照片3, " +
        //        " 处理后照片1,处理后照片2,处理后照片3,报修来源,报修处理时间,报修处理ID,网上报修时间,服务类别,紧急程度, " +
        //        " 报修说明,谈好上门时间,帐套代码,帐套名称 " +
        //    " FROM 小程序_工单管理 where 帐套代码 = @帐套代码 ";
        //    //" FROM 小程序_工单管理 where 接单人 = @接单人 and 帐套代码 = @帐套代码 ";
        //    sqlString += orderStatusCondition;
        //    sqlString += (" order by " + orderType + " desc");
        //    //sqlString += orderType == "已完成" ? " order by 完成时间 desc " : " order by ID desc ";
        //    DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
        //        new SqlParameter("@接单人", userCode),
        //        new SqlParameter("@帐套代码", ztcode),
        //        new SqlParameter("@状态", status));

        //    if (dt.Rows.Count == 0)
        //    {
        //        sr.status = "Fail";
        //        sr.result = "未查询到任何数据";
        //        return sr;
        //    }

        //    //string sqlStr = " select 序号,内容 from 基础资料_服务任务管理设置_入户维修注意事项 where left(DefClass,2) = @分类 ";
        //    //DataTable dtCaution = SQLHelper.ExecuteQuery("wyt", sqlStr, new SqlParameter("@分类", ztcode));
        //    //List<RepairCaution> rcList = new List<RepairCaution>();
        //    //if (dtCaution.Rows.Count != 0)
        //    //{
        //    //    foreach (DataRow drCaution in dtCaution.Rows)
        //    //    {
        //    //        RepairCaution rc = new RepairCaution();
        //    //        rc.number = DataTypeHelper.GetStringValue(drCaution["序号"]);
        //    //        rc.content = DataTypeHelper.GetStringValue(drCaution["内容"]);
        //    //        rcList.Add(rc);
        //    //    }
        //    //}

        //    List<Repair> repairList = new List<Repair>();
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        List<string> beforeList = new List<string>();
        //        List<string> afterList = new List<string>();
        //        Repair r = new Repair
        //        {
        //            Id = DataTypeHelper.GetIntValue(row["ID"]),
        //            SerialNumber = DataTypeHelper.GetStringValue(row["序号"]),
        //            Department = DataTypeHelper.GetStringValue(row["部门"]),
        //            Address = DataTypeHelper.GetStringValue(row["地址"]),
        //            RepairPerson = DataTypeHelper.GetStringValue(row["报修人"]),
        //            PhoneNumber = DataTypeHelper.GetStringValue(row["联系电话"]),
        //            ServiceProject = DataTypeHelper.GetStringValue(row["服务项目"]),
        //            ServiceCategory = DataTypeHelper.GetStringValue(row["服务类别"]),
        //            Level = DataTypeHelper.GetStringValue(row["紧急程度"]),
        //            //Identity = DataTypeHelper.GetStringValue(row["身份"]),
        //            //NeedIn = DataTypeHelper.GetStringValue(row["是否入户"]),
        //            RepairExplain = DataTypeHelper.GetStringValue(row["报修说明"]),
        //            RepairTime = DataTypeHelper.GetDateStringValue(row["报修时间"]),
        //            OrderTime = DataTypeHelper.GetDateStringValue(row["预约服务时间"]),
        //            VisitTime = DataTypeHelper.GetDateStringValue(row["谈好上门时间"]),
        //            SendPerson = DataTypeHelper.GetStringValue(row["发单人"]),
        //            ReceivePerson = DataTypeHelper.GetStringValue(row["接单人"]),
        //            DispatchTime = DataTypeHelper.GetDateStringValue(row["派工时间"]),
        //            ArriveTime = DataTypeHelper.GetDateStringValue(row["到场时间"]),
        //            OperatePerson = DataTypeHelper.GetStringValue(row["操作人"]),
        //            CompleteTime = DataTypeHelper.GetDateStringValue(row["完成时间"]),
        //            ChargeType = DataTypeHelper.GetStringValue(row["收费类别"]),
        //            MaterialExpense = DataTypeHelper.GetDoubleValue(row["材料费"]),
        //            LaborExpense = DataTypeHelper.GetDoubleValue(row["人工费"]),
        //            //r.IsPaid = DataTypeHelper.GetStringValue(row["是否已收"]);
        //            IsRead = DataTypeHelper.GetIntValue(row["是否阅读"]),
        //            //r.AffirmComplete = DataTypeHelper.GetStringValue(row["业主确认完成"]);
        //            //r.AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(row["业主评价"]);
        //            //r.AffirmCompleteTime = DataTypeHelper.GetDateStringValue(row["业主确认完成时间"]);
        //            //r.IsSatisfying = DataTypeHelper.GetStringValue(row["是否满意"]);
        //            CallBackEvaluation = DataTypeHelper.GetStringValue(row["回访意见"]),
        //            CallBackPerson = DataTypeHelper.GetStringValue(row["回访人"]),
        //            CallBackTime = DataTypeHelper.GetDateStringValue(row["回访时间"]),
        //            status = DataTypeHelper.GetStringValue(row["状态"])
        //        };
        //        //r.status = string.IsNullOrEmpty(r.AffirmComplete) ? r.status : "业主已确认";
        //        //r.status = string.IsNullOrEmpty(r.CallBackPerson) ? r.status : "已回访";
        //        r.CompleteStatus = DataTypeHelper.GetStringValue(row["完成情况及所耗物料"]);
        //        //r.LateTime = DataTypeHelper.GetDateStringValue(row["预计延期到"]);
        //        //r.LateReason = DataTypeHelper.GetStringValue(row["延期原因"]);
        //        beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片1"]));
        //        beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片2"]));
        //        beforeList.Add(DataTypeHelper.GetStringValue(row["报修前照片3"]));
        //        r.BeforeImage = beforeList.ToArray();
        //        afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片1"]));
        //        afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片2"]));
        //        afterList.Add(DataTypeHelper.GetStringValue(row["处理后照片3"]));
        //        r.AfterImage = afterList.ToArray();
        //        //r.Cautions = rcList.ToArray();
        //        repairList.Add(r);
        //    }
        //    sr.status = "Success";
        //    sr.result = "成功";
        //    sr.data = repairList.ToArray();

        //    return sr;
        //}





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
    }
}