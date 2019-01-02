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
    public class MyServerDal
    {
        public static StatusReport GetServerInfo(string serverName, string ztName)
        {
            StatusReport sr = new StatusReport();
            string sqlString = "select 姓名, 主管内容, 照片, 联系电话 " +
                                "from 基础资料_客服专员 " +
                                "where 姓名 = @姓名 and 帐套名称 = @帐套名称";
            DataTable dt  = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@姓名", serverName),
                new SqlParameter("@帐套名称", ztName));
            if(dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }
            DataRow dr = dt.Rows[0];
            MyServer myServer = new MyServer()
            {
                name = DataTypeHelper.GetStringValue(dr["姓名"]),
                responsibility = DataTypeHelper.GetStringValue(dr["主管内容"]),
                picture = DataTypeHelper.GetStringValue(dr["照片"]),
                phoneNumber = DataTypeHelper.GetStringValue(dr["联系电话"])
            };
            sr.status = "Success";
            sr.result = "成功";
            sr.data = myServer;
            return sr;
        }
    }
}