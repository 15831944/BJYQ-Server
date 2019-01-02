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
    public class EquipmentDal
    {
        public static StatusReport GetEquipment(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            string done = "";
            if (isDone == "0")//待处理
            {
                done = " where (分类 = @分类) AND 是否完成 is null " +
                    " and (CONVERT(nvarchar, 工作日期, 111) = CONVERT(nvarchar, GETDATE(), 111) " +
                    " or (datediff(day,工作日期,GETDATE())<= 宽限上延天数) and datediff(day,工作日期,GETDATE())>= 0 " +
                    " or (datediff(day,GETDATE(),工作日期)<=宽限下延天数) and datediff(day,GETDATE(),工作日期)>=0) " +
                    " ORDER BY ID DESC";
            }
            else if (isDone == "1")//已处理
            {
                done = " where  (分类 = @分类)  AND  (是否完成 = 1) ORDER BY ID DESC ";
            }
            else//过期
            {
                done = " where (分类 = @分类) AND 是否完成 is null " +
                    " and (datediff(day,工作日期,GETDATE())> 宽限上延天数) and datediff(day,工作日期,GETDATE())>= 0 " +
                    " ORDER BY ID DESC";
            }
            string sqlstring = " SELECT ID, 分类, 设备运行编号, 设备编号, 设备型号, 设备名称, 系统名称, 出厂日期, " +
                               " 使用日期, 设备价格, 出厂序号, 设计寿命, 卡号, 安装地点, 产地, 设备保养管理代号, 设备保养管理内容, " +
                               " 设备保养管理日期, 工作名称, 工作日期, 是否完成, 录入日期, 录入人, 完成说明, 序次, 保养前照片, 保养中照片, 保养后照片, " +
                               " 宽限上延天数,宽限下延天数 " +
                               " FROM dbo.小程序_设备管理 ";
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
            List<Equipment> equipmentList = new List<Equipment>();
            foreach (DataRow dr in dt.Rows)
            {
                Equipment equipment = new Equipment();
                equipment.ID = DataTypeHelper.GetIntValue(dr["ID"]);
                equipment.Classify = DataTypeHelper.GetStringValue(dr["分类"]);
                equipment.OperationNumber = DataTypeHelper.GetStringValue(dr["设备运行编号"]);
                equipment.Number = DataTypeHelper.GetStringValue(dr["设备编号"]);
                equipment.Name = DataTypeHelper.GetStringValue(dr["设备名称"]);
                equipment.SystemName = DataTypeHelper.GetStringValue(dr["系统名称"]);
                equipment.ProductionDate = DataTypeHelper.GetStringValue(dr["出厂日期"]);
                equipment.UseDate = DataTypeHelper.GetStringValue(dr["使用日期"]);
                equipment.price = DataTypeHelper.GetDoubleValue(dr["设备价格"]);
                equipment.ProductionNumber = DataTypeHelper.GetStringValue(dr["出厂序号"]);
                equipment.DesignedLife = DataTypeHelper.GetStringValue(dr["设计寿命"]);
                equipment.CardNumber = DataTypeHelper.GetStringValue(dr["卡号"]);
                equipment.UseAddress = DataTypeHelper.GetStringValue(dr["安装地点"]);
                equipment.ProductionAddress = DataTypeHelper.GetStringValue(dr["产地"]);
                equipment.MaintainNumber = DataTypeHelper.GetStringValue(dr["设备保养管理代号"]);
                equipment.MaintainContent = DataTypeHelper.GetStringValue(dr["设备保养管理内容"]);
                equipment.MaintainDate = DataTypeHelper.GetDateStringValue(dr["设备保养管理日期"]);
                equipment.WorkDate = DataTypeHelper.GetDateStringValue(dr["工作日期"]);
                equipment.IsDone = DataTypeHelper.GetBooleanValue(dr["是否完成"]) == true ? 1 : 0;
                equipment.InputDate = DataTypeHelper.GetDateStringValue(dr["录入日期"]);
                equipment.InputMan = DataTypeHelper.GetStringValue(dr["录入人"]);
                equipment.DoneInfo = DataTypeHelper.GetStringValue(dr["完成说明"]);
                equipment.BeforeImage = DataTypeHelper.GetStringValue(dr["保养前照片"]);
                equipment.MiddleImage = DataTypeHelper.GetStringValue(dr["保养中照片"]);
                equipment.AfterImage = DataTypeHelper.GetStringValue(dr["保养后照片"]);
                equipment.Order = DataTypeHelper.GetBooleanValue(dr["序次"]) == true ? "1" : "0";
                equipment.BeforeDays = DataTypeHelper.GetIntValue(dr["宽限上延天数"]);
                equipment.AfterDays = DataTypeHelper.GetIntValue(dr["宽限下延天数"]);
                equipmentList.Add(equipment);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipmentList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        public static StatusReport SetEquipment(string id, string isDone, string inputMan, string doneInfo, string inputDate)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "update dbo.小程序_设备管理 set " +
                                "是否完成 = @是否完成, " +
                                "完成说明 = @完成说明, " +
                                "录入人 = @录入人, " +
                                "录入日期 = @录入日期 " +
                                "where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlstring,
                new SqlParameter("@是否完成", isDone == "0" ? false : true),
                new SqlParameter("@完成说明", doneInfo),
                new SqlParameter("@录入人", inputMan),
                new SqlParameter("@录入日期", inputDate),
                new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport SetEquipmentImage(string id, string func, string imagePath)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "";
            if (func == "before")
            {
                sqlstring = "update dbo.小程序_设备管理 set 保养前照片 = @保养前照片 where ID = @ID";
            }
            else if (func == "after")
            {
                sqlstring = "update dbo.小程序_设备管理 set 保养后照片 = @保养后照片 where ID = @ID";
            }
            else
            {
                sqlstring = "update dbo.小程序_设备管理 set 保养中照片 = @保养中照片 where ID = @ID";
            }

            sr = SQLHelper.Update("wyt", sqlstring,
                new SqlParameter("@保养前照片", imagePath),
                new SqlParameter("@保养后照片", imagePath),
                new SqlParameter("@保养中照片", imagePath),
                new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport SearchEquipment(string operationNumber)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "SELECT ID, 系统名称, 设备运行编号, 卡号, 设备名称, 设备型号, 设备编号, 安装地点," +
                                " 产地, 出厂日期, 使用日期, 设备价格, 出厂序号, 设计寿命, 预计残值, 使用年限 " +
                                 "FROM 小程序_设备管理 WHERE 设备编号 = @设备编号";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring, new SqlParameter("@设备编号", operationNumber));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }
            DataRow dr = dt.Rows[0];
            Equipment equipment = new Equipment();
            equipment.OperationNumber = DataTypeHelper.GetStringValue(dr["设备运行编号"]);
            equipment.Number = DataTypeHelper.GetStringValue(dr["设备编号"]);
            equipment.Name = DataTypeHelper.GetStringValue(dr["设备名称"]);
            equipment.SystemName = DataTypeHelper.GetStringValue(dr["系统名称"]);
            equipment.ProductionDate = DataTypeHelper.GetDateStringValue(dr["出厂日期"]);
            equipment.UseDate = DataTypeHelper.GetDateStringValue(dr["使用日期"]);
            equipment.price = DataTypeHelper.GetDoubleValue(dr["设备价格"]);
            equipment.ProductionNumber = DataTypeHelper.GetStringValue(dr["出厂序号"]);
            equipment.DesignedLife = DataTypeHelper.GetStringValue(dr["设计寿命"]);
            equipment.CardNumber = DataTypeHelper.GetStringValue(dr["卡号"]);
            equipment.UseAddress = DataTypeHelper.GetStringValue(dr["安装地点"]);
            equipment.ProductionAddress = DataTypeHelper.GetStringValue(dr["产地"]);

            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipment;
            return sr;
        }

        public static StatusReport SearchEquipmentMaintain(string operationNumber)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = " SELECT ID, 分类,设备编号, 设备型号, 设备名称, 系统名称, 出厂日期, " +
                               " 使用日期, 设备价格, 出厂序号, 设计寿命, 卡号, 安装地点, 产地, 设备保养管理代号, 设备保养管理内容, " +
                               " 设备保养管理日期, 工作名称, 工作日期, 是否完成, 录入日期, 录入人, 完成说明, 序次, 保养前照片, 保养后照片, " +
                               " 宽限上延天数,宽限下延天数 " +
                               " FROM dbo.小程序_设备管理 " +
                               " WHERE 设备编号 = @设备编号 " +
                               " AND (是否完成 is null " +
                               " and (CONVERT(nvarchar, 工作日期, 111) = CONVERT(nvarchar, GETDATE(), 111) " +
                               " or (datediff(day,工作日期,GETDATE())<= 宽限上延天数) and datediff(day,工作日期,GETDATE())>= 0 " +
                               " or (datediff(day,GETDATE(),工作日期)<=宽限下延天数) and datediff(day,GETDATE(),工作日期)>=0))" +
                               " or (设备编号 = @设备编号 and 是否完成 = 1) " +
                               " ORDER BY ID DESC";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring, new SqlParameter("@设备编号", operationNumber));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何记录";
                return sr;
            }
            List<Equipment> equipmentList = new List<Equipment>();
            foreach (DataRow dr in dt.Rows)
            {
                Equipment equipment = new Equipment();
                equipment.ID = DataTypeHelper.GetIntValue(dr["ID"]);
                equipment.Classify = DataTypeHelper.GetStringValue(dr["分类"]);
                //equipment.OperationNumber = DataTypeHelper.GetStringValue(dr["设备运行编号"]);
                equipment.Number = DataTypeHelper.GetStringValue(dr["设备编号"]);
                equipment.Name = DataTypeHelper.GetStringValue(dr["设备名称"]);
                equipment.SystemName = DataTypeHelper.GetStringValue(dr["系统名称"]);
                equipment.ProductionDate = DataTypeHelper.GetDateStringValue(dr["出厂日期"]);
                equipment.UseDate = DataTypeHelper.GetDateStringValue(dr["使用日期"]);
                equipment.price = DataTypeHelper.GetDoubleValue(dr["设备价格"]);
                equipment.ProductionNumber = DataTypeHelper.GetStringValue(dr["出厂序号"]);
                equipment.DesignedLife = DataTypeHelper.GetStringValue(dr["设计寿命"]);
                equipment.CardNumber = DataTypeHelper.GetStringValue(dr["卡号"]);
                equipment.UseAddress = DataTypeHelper.GetStringValue(dr["安装地点"]);
                equipment.ProductionAddress = DataTypeHelper.GetStringValue(dr["产地"]);
                equipment.MaintainNumber = DataTypeHelper.GetStringValue(dr["设备保养管理代号"]);
                equipment.MaintainContent = DataTypeHelper.GetStringValue(dr["设备保养管理内容"]);
                equipment.MaintainDate = DataTypeHelper.GetDateStringValue(dr["设备保养管理日期"]);
                equipment.WorkDate = DataTypeHelper.GetDateStringValue(dr["工作日期"]);
                equipment.IsDone = DataTypeHelper.GetBooleanValue(dr["是否完成"]) == true ? 1 : 0;
                equipment.InputDate = DataTypeHelper.GetDateStringValue(dr["录入日期"]);
                equipment.InputMan = DataTypeHelper.GetStringValue(dr["录入人"]);
                equipment.DoneInfo = DataTypeHelper.GetStringValue(dr["完成说明"]);
                equipment.BeforeImage = DataTypeHelper.GetStringValue(dr["保养前照片"]);
                equipment.AfterImage = DataTypeHelper.GetStringValue(dr["保养后照片"]);
                equipment.Order = DataTypeHelper.GetBooleanValue(dr["序次"]) == true ? "1" : "0";
                equipment.BeforeDays = DataTypeHelper.GetIntValue(dr["宽限上延天数"]);
                equipment.AfterDays = DataTypeHelper.GetIntValue(dr["宽限下延天数"]);
                equipmentList.Add(equipment);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipmentList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        public static StatusReport GetEquipmentTrouble(string classify, string isDone)
        {
            StatusReport sr = new StatusReport();
            string done = "";
            if (isDone == "0")
            {
                done = " where (left(分类,2) = @分类) AND 完成时间 is null ORDER BY ID DESC ";
            }
            else
            {
                done = " where  (left(分类,2) = @分类)  AND  完成时间 is not null ORDER BY ID DESC ";
            }
            string sqlstring = " SELECT ID, 设备名称, 分类, 设备编号, 发生时间, 故障描述, 状态, 维修人, 维修时限, 接单时间, " +
                " 维修说明, 完成时间, 维修前照片1, 维修前照片2, 维修前照片3, 处理后照片1, 处理后照片2, 处理后照片3 " +
                " FROM dbo.基础资料_设备故障管理 ";
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
            List<EquipmentTrouble> equipmentList = new List<EquipmentTrouble>();
            foreach (DataRow dr in dt.Rows)
            {
                EquipmentTrouble equipment = new EquipmentTrouble();
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                equipment.id = DataTypeHelper.GetIntValue(dr["ID"]);
                equipment.classify = DataTypeHelper.GetStringValue(dr["分类"]);
                equipment.name = DataTypeHelper.GetStringValue(dr["设备名称"]);
                equipment.number = DataTypeHelper.GetStringValue(dr["设备编号"]);
                equipment.brokenTime = DataTypeHelper.GetDateStringValue(dr["发生时间"]);
                equipment.brokenInfo = DataTypeHelper.GetStringValue(dr["故障描述"]);
                equipment.status = DataTypeHelper.GetStringValue(dr["状态"]);
                equipment.repairMan = DataTypeHelper.GetStringValue(dr["维修人"]);
                equipment.repairTimeLimit = DataTypeHelper.GetStringValue(dr["维修时限"]);
                equipment.receiveTime = DataTypeHelper.GetDateStringValue(dr["接单时间"]);
                equipment.repairInfo = DataTypeHelper.GetStringValue(dr["维修说明"]);
                equipment.finishTime = DataTypeHelper.GetDateStringValue(dr["完成时间"]);
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片3"]));
                equipment.beforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片3"]));
                equipment.afterImage = afterList.ToArray();
                equipmentList.Add(equipment);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipmentList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }
        
        public static StatusReport SetEquipmentTrouble(string id, string fee, string doneInfo, string doneTime)
        {
            StatusReport sr = new StatusReport();
            string sqlstring = "update dbo.基础资料_设备故障管理 set " +
                                "状态 = @状态, " +
                                "维修说明 = @维修说明, " +
                                "费用 = @费用, " +
                                "完成时间 = @完成时间 " +
                                "where ID = @ID";
            sr = SQLHelper.Update("wyt", sqlstring,
                new SqlParameter("@状态", "已完成"),
                new SqlParameter("@维修说明", doneInfo),
                new SqlParameter("@费用", fee),
                new SqlParameter("@完成时间", doneTime),
                new SqlParameter("@ID", id));
            return sr;
        }

        public static StatusReport SetEquipmentTroubleImage(string ID, string func, string index, string sqlImagePath)
        {
            StatusReport sr = new StatusReport();
            string itemName = func == "before" ? "维修前照片" + index.ToString() : "处理后照片" + index.ToString();
            string sqlString = " update 基础资料_设备故障记录 set " + itemName + " = @路径 " +
                               " where ID = @ID ";
            sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@路径", sqlImagePath),
                new SqlParameter("@ID", ID));
            sr.parameters = index;
            return sr;
        }

        public static StatusReport GetEquipmentStatistics (string ztcode, string level)
        {
            StatusReport sr = new StatusReport();
            string condition = "";
            string order = "";
            string group = "";
            string sqlString = "SELECT " +
                " 帐套名称, " +
                " COUNT(CASE WHEN LEFT(CONVERT(varchar(10), 工作日期, 112), 6) = LEFT(CONVERT(varchar(10), getdate(), 112), 6) THEN ID ELSE NULL END) AS 本月应保养数, " +
                " COUNT(CASE WHEN LEFT(CONVERT(varchar(10), 工作日期, 112), 6)= LEFT(CONVERT(varchar(10), getdate(), 112), 6) AND isnull(是否完成, 0) = 1 THEN ID ELSE NULL END) AS 本月已保养数, " +
                " COUNT(CASE WHEN LEFT(CONVERT(varchar(10), 工作日期, 112), 6) = LEFT(CONVERT(varchar(10),getdate(), 112), 6) AND isnull(是否完成, 0) = 0 THEN ID ELSE NULL END) AS 本月未保养数, " +
                //" 是否完成 is null  and (datediff(day,工作日期,GETDATE())> 宽限上延天数) and datediff(day,工作日期,GETDATE())>= 0 " +
                " FROM dbo.小程序_设备管理";

            if (level == "助理" || level == "项目经理")
            {
                condition = " where 帐套代码 = @帐套代码 ";
                order = " order by 帐套名称 ";
                group = " group by 帐套名称 ";
                sqlString = sqlString + condition + group + order;
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到符合条件的记录";
                    return sr;
                }
                DataRow dr = dt.Rows[0];
                EquipmentStatistics es = new EquipmentStatistics();
                es.ztName = Convert.ToString(dr["帐套名称"]);
                es.countFinished = Convert.ToString(dr["本月已保养数"]);
                es.countUnfinished = Convert.ToString(dr["本月未保养数"]);
                es.countShouldFinished = Convert.ToString(dr["本月应保养数"]);
                es.rateFinished = GetPercent(es.countFinished, es.countShouldFinished);
                es.rateUnfinished = GetPercent(es.countUnfinished, es.countShouldFinished);
                sr.status = "Success";
                sr.result = "成功";
                sr.data = es;
                return sr;
            }
            else
            {
                condition = "";
                order = " order by 帐套名称 ";
                group = " group by 帐套名称 ";
                sqlString = sqlString + condition + group + order;
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到符合条件的记录";
                    return sr;
                }
                EquipmentStatisticsCompany esc = new EquipmentStatisticsCompany();
                List<EquipmentStatistics> esList = new List<EquipmentStatistics>();
                foreach(DataRow dr in dt.Rows)
                {
                    EquipmentStatistics es = new EquipmentStatistics();
                    es.ztName = Convert.ToString(dr["帐套名称"]);
                    es.countFinished = Convert.ToString(dr["本月已保养数"]);
                    es.countUnfinished = Convert.ToString(dr["本月未保养数"]);
                    es.countShouldFinished = Convert.ToString(dr["本月应保养数"]);
                    es.rateFinished = GetPercent(es.countFinished, es.countShouldFinished);
                    es.rateUnfinished = GetPercent(es.countUnfinished, es.countShouldFinished);
                    esc.countFinished = Convert.ToString(Convert.ToDecimal(esc.countFinished) + Convert.ToDecimal(es.countFinished));
                    esc.countUnfinished = Convert.ToString(Convert.ToDecimal(esc.countUnfinished) + Convert.ToDecimal(es.countUnfinished));
                    esc.countShouldFinished = Convert.ToString(Convert.ToDecimal(esc.countShouldFinished) + Convert.ToDecimal(es.countShouldFinished));
                    esList.Add(es);
                }
                esc.rateFinished = GetPercent(esc.countFinished, esc.countShouldFinished);
                esc.rateUnfinished = GetPercent(esc.countUnfinished, esc.countShouldFinished);
                esc.equipmentStatisticsProjects = esList.ToArray();
                
                sr.status = "Success";
                sr.result = "成功";
                sr.data = esc;
                return sr;
            }

        }

        public static StatusReport GetEquipmentReportAbstractList()
        {
            StatusReport sr = new StatusReport();
            string sqlstring =
            " SELECT " +
            " COUNT(CASE WHEN 是否完成 IS NULL AND (datediff(day, 工作日期, GETDATE()) > 宽限上延天数) AND datediff(day, 工作日期, GETDATE()) >= 0 THEN ID ELSE NULL END) AS 过期未完成数, " +
            " 帐套名称, " +
            " 帐套代码 "+
            " FROM dbo.小程序_设备管理 " +
            " GROUP BY 帐套名称,帐套代码 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }
            List<EquipmentReportCompany> ercList = new List<EquipmentReportCompany>();
            foreach(DataRow dr in dt.Rows)
            {
                EquipmentReportCompany erc = new EquipmentReportCompany();
                erc.ztCode = DataTypeHelper.GetStringValue(dr["帐套代码"]);
                erc.ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]);
                erc.countTimeout = DataTypeHelper.GetStringValue(dr["过期未完成数"]);
                ercList.Add(erc);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = ercList.ToArray();
            return sr;
        }

        public static StatusReport GetEquipmentReport(string ztcode)
        {
            StatusReport sr = new StatusReport();
            string done  = " where (帐套代码 = @帐套代码) AND 是否完成 is null " +
                    " and (datediff(day,工作日期,GETDATE())> 宽限上延天数) and datediff(day,工作日期,GETDATE())>= 0 " +
                    " ORDER BY ID DESC";
            string sqlstring = " SELECT ID, 分类, 设备运行编号, 设备编号, 设备型号, 设备名称, 系统名称, 出厂日期, " +
                               " 使用日期, 设备价格, 出厂序号, 设计寿命, 卡号, 安装地点, 产地, 设备保养管理代号, 设备保养管理内容, " +
                               " 设备保养管理日期, 工作名称, 工作日期, 是否完成, 录入日期, 录入人, 完成说明, 序次, 保养前照片, 保养中照片, 保养后照片, " +
                               " 宽限上延天数,宽限下延天数 " +
                               " FROM dbo.小程序_设备管理 ";
            sqlstring += done;

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                new SqlParameter("@帐套代码", ztcode));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                sr.parameters = sqlstring;
                return sr;
            }
            List<Equipment> equipmentList = new List<Equipment>();
            foreach (DataRow dr in dt.Rows)
            {
                Equipment equipment = new Equipment();
                equipment.ID = DataTypeHelper.GetIntValue(dr["ID"]);
                equipment.Classify = DataTypeHelper.GetStringValue(dr["分类"]);
                equipment.OperationNumber = DataTypeHelper.GetStringValue(dr["设备运行编号"]);
                equipment.Number = DataTypeHelper.GetStringValue(dr["设备编号"]);
                equipment.Name = DataTypeHelper.GetStringValue(dr["设备名称"]);
                equipment.SystemName = DataTypeHelper.GetStringValue(dr["系统名称"]);
                equipment.ProductionDate = DataTypeHelper.GetStringValue(dr["出厂日期"]);
                equipment.UseDate = DataTypeHelper.GetStringValue(dr["使用日期"]);
                equipment.price = DataTypeHelper.GetDoubleValue(dr["设备价格"]);
                equipment.ProductionNumber = DataTypeHelper.GetStringValue(dr["出厂序号"]);
                equipment.DesignedLife = DataTypeHelper.GetStringValue(dr["设计寿命"]);
                equipment.CardNumber = DataTypeHelper.GetStringValue(dr["卡号"]);
                equipment.UseAddress = DataTypeHelper.GetStringValue(dr["安装地点"]);
                equipment.ProductionAddress = DataTypeHelper.GetStringValue(dr["产地"]);
                equipment.MaintainNumber = DataTypeHelper.GetStringValue(dr["设备保养管理代号"]);
                equipment.MaintainContent = DataTypeHelper.GetStringValue(dr["设备保养管理内容"]);
                equipment.MaintainDate = DataTypeHelper.GetDateStringValue(dr["设备保养管理日期"]);
                equipment.WorkDate = DataTypeHelper.GetDateStringValue(dr["工作日期"]);
                equipment.IsDone = DataTypeHelper.GetBooleanValue(dr["是否完成"]) == true ? 1 : 0;
                equipment.InputDate = DataTypeHelper.GetDateStringValue(dr["录入日期"]);
                equipment.InputMan = DataTypeHelper.GetStringValue(dr["录入人"]);
                equipment.DoneInfo = DataTypeHelper.GetStringValue(dr["完成说明"]);
                equipment.BeforeImage = DataTypeHelper.GetStringValue(dr["保养前照片"]);
                equipment.MiddleImage = DataTypeHelper.GetStringValue(dr["保养中照片"]);
                equipment.AfterImage = DataTypeHelper.GetStringValue(dr["保养后照片"]);
                equipment.Order = DataTypeHelper.GetBooleanValue(dr["序次"]) == true ? "1" : "0";
                equipment.BeforeDays = DataTypeHelper.GetIntValue(dr["宽限上延天数"]);
                equipment.AfterDays = DataTypeHelper.GetIntValue(dr["宽限下延天数"]);
                equipmentList.Add(equipment);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipmentList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        public static StatusReport GetEquipmentTroubleStatistics(string ztcode, string level)
        {
            StatusReport sr = new StatusReport();
            string condition = "";
            string order = "";
            string group = "";
            string sqlString = " SELECT " +
                " dbo.资源帐套表.帐套代码, " +
                " dbo.资源帐套表.帐套名称, " +
                " COUNT(CASE WHEN 接单时间 IS NULL THEN NULL ELSE 1 END) AS 接单数, " +
                " COUNT(CASE WHEN DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) > 0 AND DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) <= 8 THEN 1 ELSE NULL END) AS [8小时内完成数], " +
                " COUNT(CASE WHEN DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) > 8 AND DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) <= 24 THEN 1 ELSE NULL END)  AS [8到24小时内完成数], " +
                " COUNT(CASE WHEN DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) >= 24 THEN 1 ELSE NULL END) AS [24小时以上完成数] " +
                " FROM dbo.基础资料_设备故障管理 LEFT OUTER JOIN dbo.资源帐套表 ON LEFT(dbo.基础资料_设备故障管理.分类, 2) = dbo.资源帐套表.帐套代码 ";

            if (level == "助理" || level == "项目经理")
            {
                condition = " WHERE (dbo.基础资料_设备故障管理.接单时间 IS NOT NULL) AND 帐套代码 = @帐套代码 ";
                order = " order by 帐套代码 ";
                group = " GROUP BY dbo.资源帐套表.帐套代码, dbo.资源帐套表.帐套名称, dbo.基础资料_设备故障管理.分类";
                sqlString = sqlString + condition + group + order;
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到符合条件的记录";
                    return sr;
                }
                DataRow dr = dt.Rows[0];
                EquipmentTroubleStatistics ets = new EquipmentTroubleStatistics();
                ets.ztName = Convert.ToString(dr["帐套名称"]);
                ets.countReceive = Convert.ToString(dr["接单数"]);
                ets.count8 = Convert.ToString(dr["8小时内完成数"]);
                ets.count8to24 = Convert.ToString(dr["8到24小时内完成数"]);
                ets.count24 = Convert.ToString(dr["24小时以上完成数"]);
                ets.rate8 = GetPercent(ets.count8, ets.countReceive);
                ets.rate8to24 = GetPercent(ets.count8to24, ets.countReceive);
                ets.rate24 = GetPercent(ets.count24, ets.countReceive);
                sr.status = "Success";
                sr.result = "成功";
                sr.data = ets;
                return sr;
            }
            else
            {
                condition = " WHERE (dbo.基础资料_设备故障管理.接单时间 IS NOT NULL) ";
                order = " order by 帐套代码 ";
                group = " GROUP BY dbo.资源帐套表.帐套代码, dbo.资源帐套表.帐套名称, dbo.基础资料_设备故障管理.分类";
                sqlString = sqlString + condition + group + order;
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztcode));
                if (dt.Rows.Count == 0)
                {
                    sr.status = "Fail";
                    sr.result = "未查询到符合条件的记录";
                    return sr;
                }
                EquipmentTroubleStatisticsCompany esc = new EquipmentTroubleStatisticsCompany();
                List<EquipmentTroubleStatistics> esList = new List<EquipmentTroubleStatistics>();
                foreach (DataRow dr in dt.Rows)
                {
                    EquipmentTroubleStatistics es = new EquipmentTroubleStatistics();
                    es.ztName = Convert.ToString(dr["帐套名称"]);
                    es.countReceive = Convert.ToString(dr["接单数"]);
                    es.count8 = Convert.ToString(dr["8小时内完成数"]);
                    es.count8to24 = Convert.ToString(dr["8到24小时内完成数"]);
                    es.count24 = Convert.ToString(dr["24小时以上完成数"]);
                    es.rate8 = GetPercent(es.count8, es.countReceive);
                    es.rate8to24 = GetPercent(es.count8to24, es.countReceive);
                    es.rate24 = GetPercent(es.count24, es.countReceive);
                    esc.countReceive = Convert.ToString(Convert.ToDecimal(esc.countReceive) + Convert.ToDecimal(es.countReceive));
                    esc.count8 = Convert.ToString(Convert.ToDecimal(esc.count8) + Convert.ToDecimal(es.count8));
                    esc.count8to24 = Convert.ToString(Convert.ToDecimal(esc.count8to24) + Convert.ToDecimal(es.count8to24));
                    esc.count24 = Convert.ToString(Convert.ToDecimal(esc.count24) + Convert.ToDecimal(es.count24));
                    esList.Add(es);
                }
                esc.rate8 = GetPercent(esc.count8, esc.countReceive);
                esc.rate8to24 = GetPercent(esc.count8to24, esc.countReceive);
                esc.rate24 = GetPercent(esc.count24, esc.countReceive);
                esc.equipmentTroubleStatisticsProjects = esList.ToArray();

                sr.status = "Success";
                sr.result = "成功";
                sr.data = esc;
                return sr;
            }

        }

        public static StatusReport GetEquipmentTroubleReportAbstractList()
        {
            StatusReport sr = new StatusReport();
            string sqlstring =
            " SELECT " +
            " dbo.资源帐套表.帐套代码, " +
            " dbo.资源帐套表.帐套名称, " +
            " COUNT(CASE WHEN DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) > CONVERT(int, LEFT(维修时限, LEN(维修时限) - 2)) THEN 1 ELSE NULL END) AS 未按时完成数 " +
            " FROM dbo.基础资料_设备故障管理 " +
            " LEFT OUTER JOIN dbo.资源帐套表 ON LEFT(dbo.基础资料_设备故障管理.分类, 2) = dbo.资源帐套表.帐套代码 " +
            " GROUP BY dbo.资源帐套表.帐套代码, dbo.资源帐套表.帐套名称 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring);
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                return sr;
            }
            List<EquipmentTroubleReportCompany> ercList = new List<EquipmentTroubleReportCompany>();
            foreach (DataRow dr in dt.Rows)
            {
                EquipmentTroubleReportCompany erc = new EquipmentTroubleReportCompany();
                erc.ztCode = DataTypeHelper.GetStringValue(dr["帐套代码"]);
                erc.ztName = DataTypeHelper.GetStringValue(dr["帐套名称"]);
                erc.countTimeout = DataTypeHelper.GetStringValue(dr["未按时完成数"]);
                ercList.Add(erc);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = ercList.ToArray();
            return sr;
        }

        public static StatusReport GetEquipmentTroubleReport(string ztcode)
        {
            StatusReport sr = new StatusReport();
            string done = " where (left(分类,2) = @分类) " +
                " AND (完成时间 is not null) " +
                " AND ( DATEDIFF(hh, 接单时间, ISNULL(完成时间, '1990/1/1')) > CONVERT(int, LEFT(维修时限, LEN(维修时限) - 2))) " +
                " ORDER BY ID DESC ";
            string sqlstring = " SELECT ID, 设备名称, 分类, 设备编号, 发生时间, 故障描述, 状态, 维修人, 维修时限, 接单时间, " +
                " 维修说明, 完成时间, 维修前照片1, 维修前照片2, 维修前照片3, 处理后照片1, 处理后照片2, 处理后照片3 " +
                " FROM dbo.基础资料_设备故障管理 ";
            sqlstring += done;

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlstring,
                new SqlParameter("@分类", ztcode));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到任何数据";
                sr.parameters = sqlstring;
                return sr;
            }
            List<EquipmentTrouble> equipmentList = new List<EquipmentTrouble>();
            foreach (DataRow dr in dt.Rows)
            {
                EquipmentTrouble equipment = new EquipmentTrouble();
                List<string> beforeList = new List<string>();
                List<string> afterList = new List<string>();
                equipment.id = DataTypeHelper.GetIntValue(dr["ID"]);
                equipment.classify = DataTypeHelper.GetStringValue(dr["分类"]);
                equipment.name = DataTypeHelper.GetStringValue(dr["设备名称"]);
                equipment.number = DataTypeHelper.GetStringValue(dr["设备编号"]);
                equipment.brokenTime = DataTypeHelper.GetDateStringValue(dr["发生时间"]);
                equipment.brokenInfo = DataTypeHelper.GetStringValue(dr["故障描述"]);
                equipment.status = DataTypeHelper.GetStringValue(dr["状态"]);
                equipment.repairMan = DataTypeHelper.GetStringValue(dr["维修人"]);
                equipment.repairTimeLimit = DataTypeHelper.GetStringValue(dr["维修时限"]);
                equipment.receiveTime = DataTypeHelper.GetDateStringValue(dr["接单时间"]);
                equipment.repairInfo = DataTypeHelper.GetStringValue(dr["维修说明"]);
                equipment.finishTime = DataTypeHelper.GetDateStringValue(dr["完成时间"]);
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片1"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片2"]));
                beforeList.Add(DataTypeHelper.GetStringValue(dr["维修前照片3"]));
                equipment.beforeImage = beforeList.ToArray();
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片1"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片2"]));
                afterList.Add(DataTypeHelper.GetStringValue(dr["处理后照片3"]));
                equipment.afterImage = afterList.ToArray();
                equipmentList.Add(equipment);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = equipmentList.ToArray();
            sr.parameters = sqlstring;
            return sr;
        }

        //TODO: 待完善
        private static string GetPercent(string value1, string value2)
        {
            decimal number1 = Convert.ToDecimal(value1);
            decimal number2 = Convert.ToDecimal(value2);
            decimal result = 0;
            if (number2 == 0)
            {
                return "0%";
            }
            else
            {
                result = number1 / number2;
                return result.ToString("p2");
            }
        }





       
    }
}