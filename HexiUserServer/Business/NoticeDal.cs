using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using static HexiUtils.DataTypeHelper;

namespace HexiUserServer.Business
{
    public class NoticeDal
    {
        public static StatusReport GetNotice(string ztcode)
        {
            StatusReport sr = new StatusReport();
            //string sqlString = " select 发布类型,标题,发布内容,相关文件,发布时间 where 是否发布 = '发布' and 分类 = @分类";
            string sqlString = " select 发布类型,标题,发布内容,相关文件,发布时间 from 基础资料_业主通告管理 where 是否发布 = '发布' and 发布类型 in ('社区动态','物业公告','业委公告')";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@分类", ztcode));
            if (dt.Rows.Count == 0)
            {
                return sr.SetFail("不存在通知公告");
            }
            List<object> noticeList = new List<object>();
            foreach(DataRow dr in dt.Rows)
            {
                var notice = new
                {
                    type = GetStringValue(dr["发布类型"]),
                    title = GetStringValue(dr["标题"]),
                    content = GetStringValue(dr["发布内容"]),
                    files = GetStringValue(dr["相关文件"]),
                    date = GetDateStringValue(dr["发布时间"])
                };
                noticeList.Add(notice);
            }
            return sr.SetSuccess(noticeList.ToArray());
        }



        public static StatusReport GetLaw(string ztcode)
        {
            StatusReport sr = new StatusReport();
            //string sqlString = " select 发布类型,标题,发布内容,相关文件,发布时间 where 是否发布 = '发布' and 分类 = @分类";
            string sqlString = " select 发布类型,标题,发布内容,相关文件,发布时间 from 基础资料_业主通告管理 where 是否发布 = '发布' and 发布类型 in ('法规','知识')";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@分类", ztcode));
            if (dt.Rows.Count == 0)
            {
                return sr.SetFail("不存在法律法规");
            }
            List<object> noticeList = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                var notice = new
                {
                    type = GetStringValue(dr["发布类型"]),
                    title = GetStringValue(dr["标题"]),
                    content = GetStringValue(dr["发布内容"]),
                    files = GetStringValue(dr["相关文件"]),
                    date = GetDateStringValue(dr["发布时间"])
                };
                noticeList.Add(notice);
            }
            return sr.SetSuccess(noticeList.ToArray());
        }
    }
}