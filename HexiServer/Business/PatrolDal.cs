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


        public static StatusReport GetPatrol(string userCode, string ztcode)
        {

            StatusReport sr = new StatusReport();
            string sqlString =
               " SELECT TOP 100 [ID],[分类],[所属小区] ,[类型] ,[上报人],[上报时间],[联系电话] ," +
               " [上报地址] ,[上报内容],[上报照片1],[上报照片2],[上报照片3],[处理时间],[处理情况],[完成时间]," +
               " [完成情况],[处理人] " +
               " FROM[qwytnet].[dbo].[基础资料_报事管理]" +
               " where 分类 = @帐套代码 and 上报人 = @上报人 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@上报人", userCode),
                new SqlParameter("@帐套代码", ztcode));

            if (dt.Rows.Count == 0)
            {
                return sr.SetFail("未查询到任何数据");
            }


            List<object> patrolList = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                List<string> beforeList = new List<string>();
                //List<string> afterList = new List<string>();
                for(int i = 1; i <= 3; i++)
                {
                    beforeList.Add(DataTypeHelper.GetStringValue(row["上报照片" + i.ToString()]));
                    //afterList.Add(DataTypeHelper.GetStringValue(row["上报照片" + i.ToString()]));
                }
                var patrol = new
                {
                    id = DataTypeHelper.GetIntValue(row["ID"]),
                    classify = DataTypeHelper.GetStringValue(row["分类"]),
                    ztName = DataTypeHelper.GetStringValue(row["所属小区"]),
                    type = DataTypeHelper.GetStringValue(row["类型"]),
                    patrolMan = DataTypeHelper.GetStringValue(row["上报人"]),
                    patrolTime = DataTypeHelper.GetStringValue(row["上报时间"]),
                    phone = DataTypeHelper.GetStringValue(row["联系电话"]),
                    address = DataTypeHelper.GetStringValue(row["上报地址"]),
                    content = DataTypeHelper.GetStringValue(row["上报内容"]),
                    dealTime = DataTypeHelper.GetStringValue(row["处理时间"]),
                    dealContent = DataTypeHelper.GetStringValue(row["处理情况"]),
                    finishTime = DataTypeHelper.GetStringValue(row["完成时间"]),
                    finishContent = DataTypeHelper.GetStringValue(row["完成情况"]),
                    dealMan = DataTypeHelper.GetStringValue(row["处理人"]),
                    beforeImages = beforeList.ToArray()
                };
                patrolList.Add(patrol);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = patrolList.ToArray();

            return sr;
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
    }
}