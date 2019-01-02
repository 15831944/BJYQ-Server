using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUserServer.Models;
using HexiUtils;


namespace HexiUserServer.Business
{
    public class RepairDal
    { /// <summary>
      /// 获取满足条件的工单的列表
      /// </summary>
      /// <param name="openId"></param>
      /// <param name="ztcode"></param>
      /// <returns></returns>
        public static StatusReport GetRepairOrder(string name, string phone)
        {
            StatusReport sr = new StatusReport();
            string sqlString =
                "select top 100 " +
                " ID,序号,部门,地址,报修人,联系电话,服务项目,服务类别," +
                "紧急程度,报修说明,报修时间,预约服务时间,谈好上门时间,发单人,接单人,派工时间," +
                "到场时间,操作人,完成时间,材料费,人工费,是否阅读,状态,完成情况及所耗物料," +
                "报修前照片1,报修前照片2,报修前照片3,处理后照片1,处理后照片2,处理后照片3,业主确认完成,业主确认完成时间,业主评价,是否满意 " +
                "from 基础资料_服务任务管理 " +
                "where 报修人 = @报修人 and 联系电话 = @联系电话 " +
                "order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@报修人", name),
                new SqlParameter("@联系电话", phone));

            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到符合条件的报修记录";
            }
            else
            {
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
                    r.MaterialExpense = DataTypeHelper.GetDoubleValue(row["材料费"]);
                    r.LaborExpense = DataTypeHelper.GetDoubleValue(row["人工费"]);
                    r.IsRead = DataTypeHelper.GetIntValue(row["是否阅读"]);
                    r.status = DataTypeHelper.GetStringValue(row["状态"]);
                    r.CompleteStatus = DataTypeHelper.GetStringValue(row["完成情况及所耗物料"]);
                    r.IsSatisfying = DataTypeHelper.GetStringValue(row["是否满意"]);
                    r.AffirmComplete = DataTypeHelper.GetStringValue(row["业主确认完成"]);
                    r.AffirmCompleteEvaluation = DataTypeHelper.GetStringValue(row["业主评价"]);
                    r.AffirmCompleteTime = DataTypeHelper.GetDateStringValue(row["业主确认完成时间"]);
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
            }

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
        public static StatusReport SetRepairOrder(string name, string phone, string address, string content, string time,string classify)
        {
            StatusReport sr = new StatusReport();
            string sqlString =
                "insert into 基础资料_服务任务管理 (报修人,地址,联系电话,服务项目,报修时间,分类,报修来源) " +
                "select @报修人,@地址,@联系电话,@服务项目,@报修时间,@分类,@报修来源 " +
                "SELECT @@IDENTITY";
                
            sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@报修人", name),
                new SqlParameter("@地址", address),
                new SqlParameter("@联系电话", phone),
                new SqlParameter("@服务项目", content),
                new SqlParameter("@报修时间", time),
                new SqlParameter("@分类", classify),
                new SqlParameter("@报修来源", "小程序业主报修"));

            return sr;
        }


        public static StatusReport SetOrderIsRead(string id)
        {
            string sqlstring = "update 基础资料_服务任务管理 set 是否阅读 = 0 where ID = @ID";
            StatusReport sr = SQLHelper.Update("wyt", sqlstring, new SqlParameter("@ID", id));
            return sr;
        }




        public static StatusReport SetRepairImage(string ID, string func, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = func == "before" ? "报修前照片" + index.ToString() : "处理后照片" + index.ToString();
            string sqlString = " update 基础资料_服务任务管理 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }

        public static StatusReport Evaluation(string evaluation, string isSatisfying, string isFinish, string id)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "update 基础资料_服务任务管理 " +
                " set 业主确认完成 = @业主确认完成," +
                " 业主确认完成时间 = @业主确认完成时间, " +
                " 是否满意 = @是否满意, " +
                " 业主评价 = @业主评价 " +
                " where ID = @ID " +
                " select @@identity ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@业主确认完成", isFinish),
                new SqlParameter("@业主确认完成时间", DateTime.Now),
                new SqlParameter("@是否满意", isSatisfying),
                new SqlParameter("@业主评价", evaluation),
                new SqlParameter("@ID", id));
            return sr;
        }
    }
}