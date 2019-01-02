using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using HexiUtils;
using HexiServer.Models;


namespace HexiServer.Business
{
    public class DecorationDal
    {
        public static StatusReport GetDecorationList(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            string done = "";
            if (isDone == "0")
            {
                //done = " where (分类 = @分类) AND (客服专员 = @客服专员) AND (验收时间 is null) ORDER BY ID DESC";
                done = " where (left(分类,2) = @分类) AND (状态 is null or 状态 = '正在装修') ORDER BY ID DESC";
            }
            else 
            {
                //done = " where (分类 = @分类) AND (客服专员 = @客服专员) AND (验收时间 is not null) ORDER BY ID DESC ";
                done = " where (left(分类,2) = @分类) AND (状态 is null or 状态 = '正在装修') ORDER BY ID DESC ";
            }
            string sqlstring = " SELECT top 100 ID, 分类, 房产编号, 业主姓名, 业主电话, 客服专员, 装修负责人, 装修负责人电话, 装修类型, " +
                " 装修公司押金交纳人, 装修押金金额, 财务收款人, 收款日期, 装修施工证编号, 办理时间, 开工时间, 状态, 装修内容, 是否封阳台, " +
                " 第一次验收人, 第一次验收时间, 第一次验收结果, 第一次验收结果说明, 第二次验收人, 第二次验收时间, 第二次验收结果, 第二次验收结果说明, " +
                " 安装单位名称, 执照号码, 负责人姓名, 负责人电话, 押金交纳人, 封阳台押金金额, 收款人, 退款人, 退款日期, 业主确认, 工程指导人员签字, " +
                " 工程指导人员签字日期, 门岗进场控制签字, 门岗进场控制签字日期, 工程确认进场签字, 工程确认进场签字日期, 工程封装巡查签字, 工程封装巡查签字日期, " +
                " 封装完毕验收签字, 封装完毕验收签字日期, 工程主管审核, 工程主管审核日期 " +
                " FROM dbo.基础资料_装修管理 ";
            sqlstring += done;

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                new SqlParameter("@分类", classify));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                sr.parameters = sqlstring;
                return sr;
            }
            List<Decoration> decorationList = new List<Decoration>();
            foreach (DataRow dr in dt.Rows)
            {
                Decoration decoration = new Decoration();
                decoration.id = DataTypeHelper.GetIntValue(dr["ID"]);
                decoration.classify = DataTypeHelper.GetStringValue(dr["分类"]);
                decoration.roomNumber = DataTypeHelper.GetStringValue(dr["房产编号"]);
                decoration.name = DataTypeHelper.GetStringValue(dr["业主姓名"]);
                decoration.phone = DataTypeHelper.GetStringValue(dr["业主电话"]);
                decoration.attache = DataTypeHelper.GetStringValue(dr["客服专员"]);
                decoration.decorationMan = DataTypeHelper.GetStringValue(dr["装修负责人"]);
                decoration.decorationPhone = DataTypeHelper.GetStringValue(dr["装修负责人电话"]);
                decoration.type = DataTypeHelper.GetStringValue(dr["装修类型"]);
                decoration.decorationCompanyChargeMan = DataTypeHelper.GetStringValue(dr["装修公司押金交纳人"]);
                decoration.decorationCharge = DataTypeHelper.GetDecimalValue(dr["装修押金金额"]);
                decoration.decorationChargeReceiver = DataTypeHelper.GetStringValue(dr["财务收款人"]);
                decoration.decorationChargeReceiveDate = DataTypeHelper.GetStringValue(dr["收款日期"]);
                //decoration.teamMembers = GetTeamMember(decoration.id);
                decoration.certificateNumber = DataTypeHelper.GetStringValue(dr["装修施工证编号"]);
                decoration.transactTime = DataTypeHelper.GetDateStringValue(dr["办理时间"]);
                decoration.startTime = DataTypeHelper.GetDateStringValue(dr["开工时间"]);
                decoration.status = DataTypeHelper.GetStringValue(dr["状态"]);
                decoration.contents = GetDecorationContent(decoration.id);
                decoration.needSealingBalcony = DataTypeHelper.GetStringValue(dr["是否封阳台"]);
                decoration.checkMan1 = DataTypeHelper.GetStringValue(dr["第一次验收人"]);
                decoration.checkTime1 = DataTypeHelper.GetDateStringValue(dr["第一次验收时间"]);
                decoration.checkResult1 = DataTypeHelper.GetStringValue(dr["第一次验收结果"]);
                decoration.checkResultExplain1 = DataTypeHelper.GetStringValue(dr["第一次验收结果说明"]);
                decoration.checkMan2 = DataTypeHelper.GetStringValue(dr["第二次验收人"]);
                decoration.checkTime2 = DataTypeHelper.GetDateStringValue(dr["第二次验收时间"]);
                decoration.checkResult2 = DataTypeHelper.GetStringValue(dr["第二次验收结果"]);
                decoration.checkResultExplain2 = DataTypeHelper.GetStringValue(dr["第二次验收结果说明"]);
                decoration.installDepartment = DataTypeHelper.GetStringValue(dr["安装单位名称"]);
                decoration.licenseNumber = DataTypeHelper.GetStringValue(dr["执照号码"]);
                decoration.leaderName = DataTypeHelper.GetStringValue(dr["负责人姓名"]);
                decoration.leaderPhone = DataTypeHelper.GetStringValue(dr["负责人电话"]);
                decoration.chargeMan = DataTypeHelper.GetStringValue(dr["押金交纳人"]);
                decoration.charge = DataTypeHelper.GetDecimalValue(dr["封阳台押金金额"]);
                decoration.receiver = DataTypeHelper.GetStringValue(dr["收款人"]);
                decoration.refundMan = DataTypeHelper.GetStringValue(dr["退款人"]);
                decoration.refundDate = DataTypeHelper.GetDateStringValue(dr["退款日期"]);
                decoration.proprietorCheck = DataTypeHelper.GetStringValue(dr["业主确认"]);
                decoration.checkEngineer = DataTypeHelper.GetStringValue(dr["工程指导人员签字"]);
                decoration.checkEngineerSignDate = DataTypeHelper.GetDateStringValue(dr["工程指导人员签字日期"]);
                decoration.accessController = DataTypeHelper.GetStringValue(dr["门岗进场控制签字"]);
                decoration.accessControllerSignDate = DataTypeHelper.GetDateStringValue(dr["门岗进场控制签字日期"]);
                decoration.engineeringCheckAccessMan = DataTypeHelper.GetStringValue(dr["工程确认进场签字"]);
                decoration.engineeringCheckAccessManSignDate = DataTypeHelper.GetDateStringValue(dr["工程确认进场签字日期"]);
                decoration.engineeringPatrolMan = DataTypeHelper.GetStringValue(dr["工程封装巡查签字"]);
                decoration.engineeringPatrolManSignDate = DataTypeHelper.GetDateStringValue(dr["工程封装巡查签字日期"]);
                decoration.engineeringCheckMan = DataTypeHelper.GetStringValue(dr["封装完毕验收签字"]);
                decoration.engineeringCheckManSignDate = DataTypeHelper.GetDateStringValue(dr["封装完毕验收签字日期"]);
                decoration.engineeringManager = DataTypeHelper.GetStringValue(dr["工程主管审核"]);
                decoration.engineeringManagerSignDate = DataTypeHelper.GetDateStringValue(dr["工程主管审核日期"]);
                decorationList.Add(decoration);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = decorationList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        public static StatusReport SetDecorationPatrolResult(string id, string classify, string checkMan, string checkDate,
            string dealWay, string havePeople, string isNormal, string otherUnnormalItemExplain, string schedule,
            string unnormalItemNumber, string roomNumber)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " insert into 基础资料_装修巡检 " +
                " (分类,装修表ID,房产编号,巡检日期,巡检人,是否正常,不合格项编号,其他不合格内容说明,现场是否有人,处理方式,装修进度) " +
                " select " +
                " @分类,@装修表ID,@房产编号,@巡检日期,@巡检人,@是否正常,@不合格项编号,@其他不合格内容说明,@现场是否有人,@处理方式,@装修进度  " +
                " select @@identity ";
            sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@分类", DataTypeHelper.GetDBValue(classify)),
                new SqlParameter("@装修表ID", DataTypeHelper.GetDBValue(id)),
                new SqlParameter("@房产编号", DataTypeHelper.GetDBValue(roomNumber)),
                new SqlParameter("@巡检日期", DataTypeHelper.GetDBValue(checkDate)),
                new SqlParameter("@巡检人", DataTypeHelper.GetDBValue(checkMan)),
                new SqlParameter("@是否正常", DataTypeHelper.GetDBValue(isNormal)),
                new SqlParameter("@不合格项编号", DataTypeHelper.GetDBValue(unnormalItemNumber)),
                new SqlParameter("@其他不合格内容说明", DataTypeHelper.GetDBValue(otherUnnormalItemExplain)),
                new SqlParameter("@现场是否有人", DataTypeHelper.GetDBValue(havePeople)),
                new SqlParameter("@处理方式", DataTypeHelper.GetDBValue(dealWay)),
                new SqlParameter("@装修进度", DataTypeHelper.GetDBValue(schedule)));

            return sr;
        }


        public static StatusReport SetDecorationResult(string id, string accessController, string accessControllerSignDate,
            string checkEngineer, string checkEngineerSignDate, string checkMan1, string checkResult1, string checkResultExplain1,
            string checkTime1, string checkMan2, string checkResult2, string checkResultExplain2,
            string checkTime2, string engineeringCheckAccessMan, string engineeringCheckAccessManSignDate,
            string engineeringCheckMan, string engineeringCheckManSignDate, string engineeringManager,
            string engineeringManagerSignDate, string engineeringPatrolMan, string engineeringPatrolManSignDate)
        {
            StatusReport sr = new StatusReport();
            string sqlString = 
                " update dbo.基础资料_装修管理 set " +
                " 第一次验收时间 = @第一次验收时间, 第一次验收人 = @第一次验收人, 第一次验收结果 = @第一次验收结果, 第一次验收结果说明 = @第一次验收结果说明, " +
                " 第二次验收时间 = @第二次验收时间, 第二次验收人 = @第二次验收人, 第二次验收结果 = @第二次验收结果, 第二次验收结果说明 = @第二次验收结果说明, " +
                " 工程指导人员签字 = @工程指导人员签字, 工程指导人员签字日期 = @工程指导人员签字日期, 门岗进场控制签字 = @门岗进场控制签字, 门岗进场控制签字日期 = @门岗进场控制签字日期, " +
                " 工程确认进场签字 = @工程确认进场签字, 工程确认进场签字日期 = @工程确认进场签字日期, 工程封装巡查签字 = @工程封装巡查签字, 工程封装巡查签字日期 = @工程封装巡查签字日期, " +
                " 封装完毕验收签字 = @封装完毕验收签字, 封装完毕验收签字日期 = @封装完毕验收签字日期, 工程主管审核 = @工程主管审核, 工程主管审核日期 = @工程主管审核日期 " +
                " where ID = @id " +
                " select @@identity ";
            sr = SQLHelper.Update("wyt", sqlString,
                 new SqlParameter("@第一次验收时间", DataTypeHelper.GetDBValue(checkTime1)),
                 new SqlParameter("@第一次验收人", DataTypeHelper.GetDBValue(checkMan1)),
                 new SqlParameter("@第一次验收结果", DataTypeHelper.GetDBValue(checkResult1)),
                 new SqlParameter("@第一次验收结果说明", DataTypeHelper.GetDBValue(checkResultExplain1)),
                 new SqlParameter("@第二次验收时间", DataTypeHelper.GetDBValue(checkTime2)),
                 new SqlParameter("@第二次验收人", DataTypeHelper.GetDBValue(checkMan2)),
                 new SqlParameter("@第二次验收结果", DataTypeHelper.GetDBValue(checkResult2)),
                 new SqlParameter("@第二次验收结果说明", DataTypeHelper.GetDBValue(checkResultExplain2)),
                 new SqlParameter("@工程指导人员签字", DataTypeHelper.GetDBValue(checkEngineer)),
                 new SqlParameter("@工程指导人员签字日期", DataTypeHelper.GetDBValue(checkEngineerSignDate)),
                 new SqlParameter("@门岗进场控制签字", DataTypeHelper.GetDBValue(accessController)),
                 new SqlParameter("@门岗进场控制签字日期", DataTypeHelper.GetDBValue(accessControllerSignDate)),
                 new SqlParameter("@工程确认进场签字", DataTypeHelper.GetDBValue(engineeringCheckAccessMan)),
                 new SqlParameter("@工程确认进场签字日期", DataTypeHelper.GetDBValue(engineeringCheckAccessManSignDate)),
                 new SqlParameter("@工程封装巡查签字", DataTypeHelper.GetDBValue(engineeringPatrolMan)),
                 new SqlParameter("@工程封装巡查签字日期", DataTypeHelper.GetDBValue(engineeringPatrolManSignDate)),
                 new SqlParameter("@封装完毕验收签字", DataTypeHelper.GetDBValue(engineeringCheckMan)),
                 new SqlParameter("@封装完毕验收签字日期", DataTypeHelper.GetDBValue(engineeringCheckManSignDate)),
                 new SqlParameter("@工程主管审核", DataTypeHelper.GetDBValue(engineeringManager)),
                 new SqlParameter("@工程主管审核日期", DataTypeHelper.GetDBValue(engineeringManagerSignDate)),
                 new SqlParameter("@id", DataTypeHelper.GetDBValue(id)));
            return sr;
        }

        public static StatusReport GetDecorationPatrolList(string classify)
        {
            StatusReport sr = new StatusReport();
            DecorationPatrol dp = new DecorationPatrol();
            string sqlstringNpd = " SELECT ID, 分类, 房产编号, 业主姓名, 业主电话, 客服专员, 装修负责人, 装修负责人电话, 装修类型" +
                " FROM dbo.基础资料_装修管理 " +
                " where (left(分类,2) = @分类) AND (状态 = '正在装修') ORDER BY ID DESC ";
            //NeedPatrolDecoration npd = new NeedPatrolDecoration();
            DataTable dtNpd = SQLHelper.ExecuteQuery("wyt", sqlstringNpd, new SqlParameter("@分类", classify));
            if (dtNpd.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }
            List<NeedPatrolDecoration> npdList = new List<NeedPatrolDecoration>();
            foreach(DataRow dr in dtNpd.Rows)
            {
                NeedPatrolDecoration npd = new NeedPatrolDecoration();
                npd.id = DataTypeHelper.GetIntValue(dr["ID"]);
                npd.classify = DataTypeHelper.GetStringValue(dr["分类"]);
                npd.roomNumber = DataTypeHelper.GetStringValue(dr["房产编号"]);
                npd.name = DataTypeHelper.GetStringValue(dr["业主姓名"]);
                npd.phone = DataTypeHelper.GetStringValue(dr["业主电话"]);
                npd.attache = DataTypeHelper.GetStringValue(dr["客服专员"]);
                npd.decorationMan = DataTypeHelper.GetStringValue(dr["装修负责人"]);
                npd.decorationPhone = DataTypeHelper.GetStringValue(dr["装修负责人电话"]);
                npd.type = DataTypeHelper.GetStringValue(dr["装修类型"]);
                npdList.Add(npd);
            }
            dp.needPatrolDecorations = npdList.ToArray();

            string sqlstringPi = " select 序号, 巡检项目, 是否上报 from 基础资料_装修巡检设置_检查项目 order by 序号 ";
            DataTable dtPi = SQLHelper.ExecuteQuery("wyt", sqlstringPi);
            if (dtPi.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何巡检项目";
                return sr;
            }
            List<PatrolItem> piList = new List<PatrolItem>();
            foreach(DataRow dr in dtPi.Rows)
            {
                PatrolItem pi = new PatrolItem();
                pi.number = DataTypeHelper.GetStringValue(dr["序号"]);
                pi.item = DataTypeHelper.GetStringValue(dr["巡检项目"]);
                pi.needReport = DataTypeHelper.GetStringValue(dr["是否上报"]);
                piList.Add(pi);
            }
            dp.patrolItems = piList.ToArray();

            string sqlstringDw = " select 处理方式 from 基础资料_装修巡检设置_处理方式 ";
            DataTable dtDw = SQLHelper.ExecuteQuery("wyt", sqlstringDw);
            if (dtDw.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何处理方式";
                return sr;
            }       
            List<string> dwList = new List<string>();
            foreach(DataRow dr in dtDw.Rows)
            {
                dwList.Add(DataTypeHelper.GetStringValue(dr["处理方式"]));
            }
            dp.disposeWay = dwList.ToArray();

            sr.status = "Success";
            sr.result = "成功";
            sr.data = dp;
            return sr;

        }

        public static StatusReport SetDecorationPatrolImage(string ID, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = "不正常情况照片" + index;
            string sqlString = " update 基础资料_装修巡检 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }

        //private static TeamMember[] GetTeamMember(int? pid)
        //{
        //    if (pid.HasValue)
        //    {
        //        string sqlstring = " SELECT 姓名, 联系电话, 身份证号码 FROM dbo.基础资料_装修管理_施工队成员 " +
        //            " where PID = @PID ";

        //        DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
        //            new SqlParameter("@PID", pid));
        //        if (dt.Rows.Count == 0)
        //        {
        //            return null;
        //        }
        //        List<TeamMember> tmList = new List<TeamMember>();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            TeamMember tm = new TeamMember();
        //            tm.name = DataTypeHelper.GetStringValue(dr["姓名"]);
        //            tm.phone = DataTypeHelper.GetStringValue(dr["联系电话"]);
        //            tm.idCard = DataTypeHelper.GetStringValue(dr["身份证号码"]);
        //            tmList.Add(tm);
        //        }
        //        return tmList.ToArray();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        private static DecorationContent[] GetDecorationContent(int? pid)
        {
            if (pid.HasValue)
            {
                string sqlstring = " SELECT 序号, 装修内容 FROM dbo.基础资料_装修管理_装修内容 " +
                    " where PID = @PID ";

                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                    new SqlParameter("@PID", pid));
                if (dt.Rows.Count == 0)
                {
                    return null;
                }
                List<DecorationContent> dcList = new List<DecorationContent>();
                foreach (DataRow dr in dt.Rows)
                {
                    DecorationContent dc = new DecorationContent();
                    dc.number = DataTypeHelper.GetStringValue(dr["序号"]);
                    dc.content = DataTypeHelper.GetStringValue(dr["装修内容"]);
                    dcList.Add(dc);
                }
                return dcList.ToArray();
            }
            else
            {
                return null;
            }
        }




    }
}