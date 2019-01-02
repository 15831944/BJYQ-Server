using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;

namespace HexiServer.Common
{
    public class UserHelper
    {
        /// <summary>
        /// 通过openId获取用户在物业通中的UserCode
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static string GetUser(string openid)
        {
            string sqlString =
                "select UserCode " +
                "from 用户 " +
                "where ID in (select 用户表Id from 基础资料_微信员工绑定表 where OpenId = @OpenId)";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@OpenId", openid));
            DataRow dr = dt.Rows[0];

            return (string)dr["UserCode"];
        }
    }
}