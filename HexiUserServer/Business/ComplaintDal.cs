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
    public class ComplaintDal
    {
        public static StatusReport SetComplaint (string receptionDate, string name, string address, string content, string classify, string phone)
        {
            string sqlString = " insert into 基础资料_顾客投诉处理登记表 (投诉接待时间, 投诉方式, 投诉人姓名, 地址, 投诉内容, 联系电话, 分类) " +
                               " select @投诉接待时间, @投诉方式, @投诉人姓名, @地址, @投诉内容, @联系电话, @分类 " +
                               " SELECT SCOPE_IDENTITY() ";
            StatusReport sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@投诉接待时间", receptionDate),
                new SqlParameter("@投诉方式", "小程序投诉"),
                new SqlParameter("@投诉人姓名", name),
                new SqlParameter("@地址", address),
                new SqlParameter("@投诉内容", content),
                new SqlParameter("@联系电话", phone),
                new SqlParameter("@分类", classify));
            return sr;
        }

        public static StatusReport GetComplaintList(string classify, string name, string phone)
        {
            StatusReport sr = new StatusReport()
            {
                status = "Success",
                result = "成功",
            };
            string sqlString = " select ID,投诉接待时间,投诉方式,投诉人姓名,地址,投诉内容,联系电话,投诉处理单编号,处理完成日期,处理完成情况,登记人,责任部门," +
                               " 投诉前照片1,投诉前照片2,投诉前照片3,处理后照片1,处理后照片2,处理后照片3,状态,业主确认解决,确认时间,是否满意,业主评价,不受理原因 " +
                               " from 基础资料_顾客投诉处理登记表 " +
                               " where 分类 = @分类 and 投诉人姓名 = @投诉人姓名 and 联系电话 = @联系电话 " +
                               " order by ID desc ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@分类", classify),
                new SqlParameter("@投诉人姓名", name),
                new SqlParameter("@联系电话", phone));
            
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "查询条件错误或没有数据";
                return sr;
            }

            List<Complaint> complaintList = new List<Complaint>();
            foreach (DataRow dr in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                Complaint complaint = new Complaint()
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
                    CaseDeclinedReason = DataTypeHelper.GetStringValue(dr["不受理原因"])
                };
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["投诉前照片3"]));
                complaint.BeforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片3"]));
                complaint.AfterImage = afterList.ToArray();
                complaintList.Add(complaint);
            }

            sr.data = complaintList.ToArray();
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
            sr.parameters = itemName + ID.ToString() ;
            return sr;
        }


        public static StatusReport Evaluation(string evaluation, string isSatisfying, string isFinish, string id)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "update 基础资料_顾客投诉处理登记表 " +
                " set 是否满意 = @是否满意, " +
                " 业主评价 = @业主评价, " +
                " 业主确认解决 = @业主确认解决," +
                " 确认时间 = @确认时间 " +
                " where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@业主确认解决", isFinish),
                new SqlParameter("@确认时间", DateTime.Now),
                new SqlParameter("@是否满意", isSatisfying),
                new SqlParameter("@业主评价", evaluation),
                new SqlParameter("@ID", id));
            return sr;
        }
    }
}

