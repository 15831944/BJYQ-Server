using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using HexiPicture.Models;


namespace HexiPicture.Business
{
    public class PictureDal
    {
        public static StatusReport GetPeriodsInfo()
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "SELECT TOP 1 ID, 期数, 主题, 主题描述, 开始时间, 结束时间,主办单位,参赛规则,评奖方式,注意事项 " +
                " FROM dbo.基础资料_摄影比赛设置 " +
                " WHERE (状态 = N'进行中') " +
                " ORDER BY ID DESC";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }

            DataRow dr = dt.Rows[0];
            PeriodsInfo pi = new PeriodsInfo();
            pi.id = DataTypeHelper.GetIntValue(dr["ID"]);
            pi.periods = DataTypeHelper.GetStringValue(dr["期数"]);
            pi.theme = DataTypeHelper.GetStringValue(dr["主题"]);
            pi.themeContent = DataTypeHelper.GetStringValue(dr["主题描述"]);
            pi.startTime = DataTypeHelper.GetDateStringValue(dr["开始时间"]);
            pi.endTime = DataTypeHelper.GetDateStringValue(dr["结束时间"]);
            pi.sponsor = DataTypeHelper.GetStringValue(dr["主办单位"]);
            pi.roles = DataTypeHelper.GetStringValue(dr["参赛规则"]);
            pi.appraiseWay = DataTypeHelper.GetStringValue(dr["评奖方式"]);
            pi.mattersNeedAttention = DataTypeHelper.GetStringValue(dr["注意事项"]);

            sqlstring = "select 奖项,奖品,人数 from 基础资料_摄影比赛设置_奖项设置 where PID = @PID";
            dt = SQLHelper.ExecuteQuery("wyt", sqlstring, new SqlParameter("@PID", pi.id));
            if (dt.Rows.Count != 0)
            {
                List<Awards> awardList = new List<Awards>();
                foreach(DataRow row in dt.Rows)
                {
                    Awards awards = new Awards();
                    awards.award = DataTypeHelper.GetStringValue(row["奖项"]);
                    awards.prize = DataTypeHelper.GetStringValue(row["奖品"]);
                    awards.number = DataTypeHelper.GetIntValue(row["人数"]);
                    awardList.Add(awards);
                }
                pi.awardSetting = awardList.ToArray();
            }

            sr.status = "Success";
            sr.result = "成功";
            sr.data = pi;
            return sr;
        }

        public static StatusReport SetPicture(string openid, string nackname, string phone, string picName, string periodId, string periods, string theme,string picPath,string submitTime,string description)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = " insert into 基础资料_摄影比赛 (openid,昵称,手机号,照片名,上传时间,得票数,期数ID,照片,期数,主题,描述,点击率) " +
                " select @openid,@昵称,@手机号,@照片名,@上传时间,@得票数,@期数ID,@照片,@期数,@主题,@描述,@点击率 ";
            sr = SQLHelper.Insert("wyt", sqlstring,
                new SqlParameter("@openid", openid),
                new SqlParameter("@昵称", nackname),
                new SqlParameter("@手机号", phone),
                new SqlParameter("@照片名", picName),
                new SqlParameter("@上传时间", submitTime),
                new SqlParameter("@得票数", "0"),
                new SqlParameter("@期数ID", periodId),
                new SqlParameter("@照片", picPath),
                new SqlParameter("@期数", periods),
                new SqlParameter("@主题", theme),
                new SqlParameter("@描述", description),
                new SqlParameter("@点击率", 0));
            return sr;
        }

        public static StatusReport GetPictures(string periodId,string sortType)
        {
            StatusReport sr = new StatusReport();
            string sort = "";
            if (sortType == "normal")
            {
                sort = "order by ID";
            }
            else if (sortType == "new")
            {
                sort = "order by ID desc";
            }
            else if (sortType == "best")
            {
                sort = "order by 得票数 Desc";
            }
            else
            {
                sort = "order by 点击率 Desc";
            }
            
            string sqlstring = " select ID,openid,昵称,手机号,上传时间,得票数,期数ID,照片,照片名,描述,点击率 " +
                                " from 基础资料_摄影比赛 " +
                                " where 期数ID = @期数ID ";
            sqlstring += sort;
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                new SqlParameter("@期数ID", periodId));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                sr.parameters = sqlstring;
                return sr;
            }
            List<Picture> picList = new List<Picture>();
            foreach (DataRow dr in dt.Rows)
            {
                Picture pic = new Picture();
                pic.id = DataTypeHelper.GetIntValue(dr["ID"]);
                pic.openid = DataTypeHelper.GetStringValue(dr["openid"]);
                pic.nackName = DataTypeHelper.GetStringValue(dr["昵称"]);
                pic.phone = DataTypeHelper.GetStringValue(dr["手机号"]);
                pic.uploadTime = DataTypeHelper.GetDateStringValue(dr["上传时间"]);
                pic.periodId = DataTypeHelper.GetIntValue(dr["期数ID"]);
                pic.vote = DataTypeHelper.GetIntValue(dr["得票数"]);
                pic.picPath = DataTypeHelper.GetStringValue(dr["照片"]);
                pic.picName = DataTypeHelper.GetStringValue(dr["照片名"]);
                pic.description = DataTypeHelper.GetStringValue(dr["描述"]);
                pic.rate = DataTypeHelper.GetIntValue(dr["点击率"]);
                picList.Add(pic);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = picList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        public static StatusReport SetPictureVote(string id)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "update 基础资料_摄影比赛 set 得票数 = 得票数 + 1 where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlstring, new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport SetUserVote(string id,string openid,string periodId)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = " insert into 基础资料_摄影比赛用户投票情况 (openid,期数ID,作品ID) " +
                                " select @openid,@期数ID,@作品ID ";
            sr = SQLHelper.Insert("wyt", sqlstring,
                new SqlParameter("@作品ID", id),
                new SqlParameter("@openid", openid),
                new SqlParameter("@期数ID", periodId));
            return sr;
        }

        public static int GetUserVote(string openId,string periodId)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "select count(ID) as 投票次数 from 基础资料_摄影比赛用户投票情况 where openid = @openid and 期数ID = @期数ID";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                new SqlParameter("@openId", openId),
                new SqlParameter("@期数ID", periodId));
            DataRow dr = dt.Rows[0];
            int vote = Convert.ToInt32(dr["投票次数"]);
            return vote;
        }

        public static StatusReport SetPictureTaped (string picId)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "update 基础资料_摄影比赛 set 点击率 = 点击率 + 1 where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlstring, new SqlParameter("@ID", picId));
            return sr;
        }
    }
}