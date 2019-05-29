using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HexiUtils;
using HexiServer.Models;

namespace HexiServer.Business
{
    public class LookOverDal
    {
        public static StatusReport GetLookOverRouteInfo(string ztCode, string func, string name)
        {
            string sqlString = "select distinct 巡检路线 from dbo.基础资料_巡检记录 where 分类 = @分类 and 巡检业务 = @巡检业务 and 巡检人 = @巡检人 and 工作日期 = @工作日期 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, 
                new SqlParameter("@分类", ztCode), 
                new SqlParameter("@巡检业务", func),
                new SqlParameter("@工作日期", DateTime.Now.Date),
                new SqlParameter("@巡检人", name));
            if (dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail("尚未设置巡检路线或当日没有巡检工作");
            }
            List<string> routeList = new List<string>();
            foreach(DataRow dr in dt.Rows)
            {
                routeList.Add(DataTypeHelper.GetStringValue(dr["巡检路线"]));
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "Success";
            sr.data = routeList.ToArray();
            return sr;
        }

        public static StatusReport GetLookOverInfo(string ztCode, string func, string route, string name)
        {
            
            string sqlString =
                " SELECT ID, 巡检业务, 巡检点, 巡检项目, 巡检时间 " +
                " FROM dbo.基础资料_巡检记录 where 分类 = @分类 and 巡检业务 = @巡检业务 and 巡检路线 = @巡检路线 and 巡检人 = @巡检人 and 工作日期 = @工作日期 ";
                  //condition +
                //" order by 巡检对象, 巡检项目, 巡检周期 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, 
                new SqlParameter("@分类", ztCode), 
                new SqlParameter("@巡检业务", func), 
                new SqlParameter("@巡检路线", route),
                new SqlParameter("@工作日期", DateTime.Now.Date),
                new SqlParameter("@巡检人", name));
            if (dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail();
            }
            List<LookOver> loList = new List<LookOver>();
            foreach (DataRow dr in dt.Rows)
            {
                LookOver lo = new LookOver();
                lo.id = DataTypeHelper.GetStringValue(dr["ID"]);
                lo.business = DataTypeHelper.GetStringValue(dr["巡检业务"]);
                lo.objectName = DataTypeHelper.GetStringValue(dr["巡检点"]);
                lo.item = DataTypeHelper.GetStringValue(dr["巡检项目"]);
                //lo.period = DataTypeHelper.GetStringValue(dr["巡检周期"]);
                string time = DataTypeHelper.GetDateStringValue(dr["巡检时间"]);
                lo.isLook = string.IsNullOrEmpty(time) ?  false : true ;
                loList.Add(lo);
            }
            return new StatusReport(loList.ToArray());
        }

        public static StatusReport SetLookOverResult(string isSpotCheck, string items)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " update 基础资料_巡检记录 set 巡检时间 = @巡检时间, " +
                " 是否合格 = @是否合格, 不合格说明 = @不合格说明, 状态 = @状态 " +
                " where ID = @ID ";
            JArray itemArray = (JArray)JsonConvert.DeserializeObject(items);
            for (int i = 0; i < itemArray.Count; i++)
            {
                JObject item = (JObject)itemArray[i];
                sr = SQLHelper.Update("wyt", sqlString,
                    new SqlParameter("@巡检时间", DateTime.Now),
                    new SqlParameter("@状态", "已巡检"),
                    new SqlParameter("@是否合格", Convert.ToBoolean(item["isNormal"]) ? "是" : "否"),
                    new SqlParameter("@不合格说明", DataTypeHelper.GetDBValue(item["explain"].ToString())),
                    new SqlParameter("@ID", DataTypeHelper.GetDBValue(item["id"].ToString())));
            }
            return sr;
        }


            public static StatusReport SetLookOverImage(string id, string index, string sqlImagePath)
            {
                StatusReport sr = new StatusReport();
                string itemName = "照片" + index.ToString();
                string sqlString = " update 基础资料_巡检记录 set " + itemName + " = @路径 " +
                                   " where ID = @ID ";
                sr = SQLHelper.Update("wyt", sqlString,
                    new SqlParameter("@路径", sqlImagePath),
                    new SqlParameter("@ID", id));
                sr.parameters = index;
                return sr;
            }

            ////string sqlString = " insert into 基础资料_巡检记录 (分类,巡检业务,巡检对象,巡检人,巡检时间,是否合格,是否抽检) " +
            ////    " select @分类,@巡检业务,@巡检对象,@巡检人,@巡检时间,@是否合格,@是否抽检 " +
            ////    " select @@identity ";
            //sr = SQLHelper.Insert("wyt", sqlString,
            //    new SqlParameter("@分类", ztCode),
            //    new SqlParameter("@巡检业务", business),
            //    new SqlParameter("@巡检对象", objectName),
            //    new SqlParameter("@巡检人", userName),
            //    new SqlParameter("@巡检时间", DateTime.Now),
            //    new SqlParameter("@是否合格", isNormal),
            //    new SqlParameter("@是否抽检", isSpotCheck));
            //if (sr.status == "Success")
            //{
            //    int? pid = DataTypeHelper.GetIntValue(sr.data);
            //    if (pid.HasValue)
            //    {
            //        JArray uiArr = (JArray)JsonConvert.DeserializeObject(unnormalItems);
            //        string sqlStr = " insert into 基础资料_巡检记录_不合格项 (DefClass,PID,不合格项序号,说明) " +
            //        " select @DefClass,@PID,@不合格项序号,@说明 " +
            //        " select @@identity ";
            //        for (int i = 0; i < uiArr.Count; i++)
            //        {
            //            JObject ui = (JObject)uiArr[i];
            //            sr = SQLHelper.Insert("wyt", sqlStr,
            //                    new SqlParameter("@DefClass", ztCode),
            //                    new SqlParameter("@PID", pid),
            //                    new SqlParameter("@不合格项序号", DataTypeHelper.GetDBValue(ui["num"].ToString())),
            //                    new SqlParameter("@说明", DataTypeHelper.GetDBValue(ui["explain"].ToString())));
            //        }
            //        if (sr.status == "Success")
            //        {
            //            string queryStr = "select ID from 基础资料_巡检记录_不合格项 where PID = @PID";
            //            DataTable dt = SQLHelper.ExecuteQuery("wyt", queryStr, new SqlParameter("@PID", pid));
            //            if (dt.Rows.Count != 0)
            //            {
            //                List<int?> idList = new List<int?>();
            //                foreach (DataRow dr in dt.Rows)
            //                {
            //                    idList.Add(DataTypeHelper.GetIntValue(dr["ID"]));
            //                }
            //                sr.SetSuccess(idList.ToArray());
            //            }
            //        }

            //    }
            //}
            //return sr;
        //}




        //基础资料_巡检设置
        //public static StatusReport GetHouseLookOverInfo(string ztCode, string func)
        //{
        //    string sqlString = "select 编号 from 小程序_空房 where 帐套代码 = @帐套代码 order by ID ";
        //    DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztCode));
        //    if (dt.Rows.Count == 0)
        //    {
        //        return new StatusReport().SetFail();
        //    }
        //    List<LookOverObject> looList = new List<LookOverObject>();
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        LookOverObject loo = new LookOverObject();
        //        loo.name = DataTypeHelper.GetStringValue(dr["编号"]);
        //        loo.isLook = false;
        //        looList.Add(loo);
        //    }
        //    LookOver lo = new LookOver();
        //    lo.business = func;
        //    lo.objects = looList.ToArray();
        //    lo.items = GetItems(ztCode, func);
        //    return new StatusReport(lo);
        //}

        //public static StatusReport GetNormalLookOverInfo(string ztCode, string func)
        //{
        //    LookOver lo = new LookOver();
        //    lo.business = func;
        //    lo.objects = GetObjects(ztCode, func);
        //    lo.items = GetItems(ztCode, func);
        //    return new StatusReport(lo);
        //}

        //public static StatusReport SetLookOverResult(string ztCode, string userName, string business, string objectName, string isNormal, string isSpotCheck, string unnormalItems)
        //{

        //    using (StreamWriter sw = new StreamWriter("D:\\1_importTemp\\TestFile1.txt"))
        //    {
        //        sw.WriteLine(unnormalItems);
        //        sw.WriteLine(JsonConvert.SerializeObject(unnormalItems));
        //    }
        //    StatusReport sr = new StatusReport();
        //    string sqlString = " insert into 基础资料_巡检记录 (分类,巡检业务,巡检对象,巡检人,巡检时间,是否合格,是否抽检) " +
        //        " select @分类,@巡检业务,@巡检对象,@巡检人,@巡检时间,@是否合格,@是否抽检 " +
        //        " select @@identity ";
        //    sr = SQLHelper.Insert("wyt", sqlString,
        //        new SqlParameter("@分类", ztCode),
        //        new SqlParameter("@巡检业务", business),
        //        new SqlParameter("@巡检对象", objectName),
        //        new SqlParameter("@巡检人", userName),
        //        new SqlParameter("@巡检时间", DateTime.Now),
        //        new SqlParameter("@是否合格", isNormal),
        //        new SqlParameter("@是否抽检", isSpotCheck));
        //    if (sr.status == "Success")
        //    {
        //        int? pid = DataTypeHelper.GetIntValue(sr.data);
        //        if (pid.HasValue)
        //        {
        //            JArray uiArr = (JArray)JsonConvert.DeserializeObject(unnormalItems);
        //            string sqlStr = " insert into 基础资料_巡检记录_不合格项 (DefClass,PID,不合格项序号,说明) " +
        //            " select @DefClass,@PID,@不合格项序号,@说明 " +
        //            " select @@identity ";
        //            for (int i = 0; i < uiArr.Count; i++)
        //            {
        //                JObject ui = (JObject)uiArr[i];
        //                sr = SQLHelper.Insert("wyt", sqlStr,
        //                        new SqlParameter("@DefClass", ztCode),
        //                        new SqlParameter("@PID", pid),
        //                        new SqlParameter("@不合格项序号", DataTypeHelper.GetDBValue(ui["num"].ToString())),
        //                        new SqlParameter("@说明", DataTypeHelper.GetDBValue(ui["explain"].ToString())));
        //            }
        //            if (sr.status == "Success")
        //            {
        //                string queryStr = "select ID from 基础资料_巡检记录_不合格项 where PID = @PID";
        //                DataTable dt = SQLHelper.ExecuteQuery("wyt", queryStr, new SqlParameter("@PID", pid));
        //                if (dt.Rows.Count != 0)
        //                {
        //                    List<int?> idList = new List<int?>();
        //                    foreach(DataRow dr in dt.Rows)
        //                    {
        //                        idList.Add(DataTypeHelper.GetIntValue(dr["ID"]));
        //                    }
        //                    sr.SetSuccess(idList.ToArray());
        //                }
        //            }

        //        }
        //    }
        //    return sr;
        //}

        //public static StatusReport SetLookOverImage(string id, string index, string sqlImagePath)
        //{
        //    StatusReport sr = new StatusReport();
        //    string itemName = "照片" + index.ToString();
        //    string sqlString = " update 基础资料_巡检记录_不合格项 set " + itemName + " = @路径 " +
        //                       " where ID = @ID ";
        //    sr = SQLHelper.Update("wyt", sqlString,
        //        new SqlParameter("@路径", sqlImagePath),
        //        new SqlParameter("@ID", id));
        //    sr.parameters = index;
        //    return sr;
        //}
        //private static LookOverObject[] GetObjects (string ztCode, string func)
        //{
        //    List<LookOverObject> looList = new List<LookOverObject>();
        //    string sqlString = " select 序号, 对象名称 from 基础资料_巡检设置_巡检对象 " +
        //        " where PID in (select ID from 基础资料_巡检设置 where left(分类,2) = @分类 and 巡检业务 = @巡检业务) " +
        //        " order by 序号 ";
        //    DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@分类", ztCode), new SqlParameter("@巡检业务", func));
        //    if (dt.Rows.Count == 0)
        //    {
        //        return null;
        //    }
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        LookOverObject loo = new LookOverObject();
        //        loo.number = DataTypeHelper.GetStringValue(dr["序号"]);
        //        loo.name = DataTypeHelper.GetStringValue(dr["对象名称"]);
        //        loo.isLook = false;
        //        looList.Add(loo);
        //    }

        //    return looList.ToArray();
        //}



        //private static LookOverItem[] GetItems (string ztCode, string func)
        //{
        //    List<LookOverItem> loiList = new List<LookOverItem>();
        //    string sqlString = " select 序号, 内容, 周期 from 基础资料_巡检设置_检查项目 " +
        //        " where PID in (select ID from 基础资料_巡检设置 where left(分类,2) = @分类 and 巡检业务 = @巡检业务) " +
        //        " order by 序号 ";

        //    DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@分类", ztCode), new SqlParameter("@巡检业务", func));
        //    if(dt.Rows.Count == 0)
        //    {
        //        return null;
        //    }
        //    foreach(DataRow dr in dt.Rows)
        //    {
        //        LookOverItem loi = new LookOverItem();
        //        loi.number = DataTypeHelper.GetStringValue(dr["序号"]);
        //        loi.content = DataTypeHelper.GetStringValue(dr["内容"]);
        //        loi.period = DataTypeHelper.GetStringValue(dr["周期"]);
        //        loiList.Add(loi);
        //    }

        //    return loiList.ToArray();
        //}





    }
}