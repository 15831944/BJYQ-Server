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
    public class ProprietorDal
    {

        public static Proprietor[] GetProprietorList(string ztCode, string homeNumber, string name, string licensePlateNumber)
        {
            string sqlString =
                " SELECT ID, 房产单元编号, 占用者名称, 占用者身份, 联系电话, 联系地址, 紧急联系人, 紧急联系人电话," +
                " 紧急联系人地址, 建筑面积, 当前欠款, 房号, 层数, 所属楼宇, 部门,车牌号码,车辆颜色,车辆品牌,车辆型号 " +
                " FROM dbo.小程序_现场查询 " +
                " WHERE(占用情况 = '正在占用') " +
                " and (帐套代码 = @帐套代码) " +
                (string.IsNullOrEmpty(homeNumber) ? "" : "and (房产单元编号 like '%" + homeNumber + "%') ") +
                (string.IsNullOrEmpty(name) ? "" : "and (占用者名称 like '%" + name + "%') ") +
                (string.IsNullOrEmpty(licensePlateNumber) ? "" : "and (车牌号码 like '%" + licensePlateNumber + "%') ") +
                "ORDER BY ID ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                new SqlParameter("@帐套代码", ztCode),
                new SqlParameter("@资源表编号", homeNumber),
                new SqlParameter("@占用者名称", name));

            List<Proprietor> proprietorList = new List<Proprietor>();
            foreach (DataRow row in dt.Rows)
            {
                Proprietor p = new Proprietor()
                {
                    Id = DataTypeHelper.GetIntValue(row["ID"]),
                    RoomNumber = DataTypeHelper.GetStringValue(row["房产单元编号"]),
                    RoomAddress = SetRoomAddress(DataTypeHelper.GetStringValue(row["部门"]), DataTypeHelper.GetStringValue(row["所属楼宇"]), DataTypeHelper.GetIntValue(row["层数"]), DataTypeHelper.GetStringValue(row["房号"])),
                    Name = DataTypeHelper.GetStringValue(row["占用者名称"]),
                    Identity = DataTypeHelper.GetStringValue(row["占用者身份"]),
                    Phone = DataTypeHelper.GetStringValue(row["联系电话"]),
                    Address = DataTypeHelper.GetStringValue(row["联系地址"]),
                    EmergencyContact = DataTypeHelper.GetStringValue(row["紧急联系人"]),
                    EmergencyPhone = DataTypeHelper.GetStringValue(row["紧急联系人电话"]),
                    EmergencyAddress = DataTypeHelper.GetStringValue(row["紧急联系人地址"]),
                    Area = DataTypeHelper.GetDoubleValue(row["建筑面积"]),
                    TotalArrearage = DataTypeHelper.GetDoubleValue(row["当前欠款"]),
                    LicensePlateNumber = DataTypeHelper.GetStringValue(row["车牌号码"]),
                    CarBrand = DataTypeHelper.GetStringValue(row["车辆品牌"]),
                    CarColor = DataTypeHelper.GetStringValue(row["车辆颜色"]),
                    CarType = DataTypeHelper.GetStringValue(row["车辆型号"])
                };

                proprietorList.Add(p);
            }
            return proprietorList.ToArray();
        }

        private static string SetRoomAddress(string department,string building, int? floor, string roomNumber)
        {
            building = string.IsNullOrEmpty(building) ? "" : building;
            string floorStr = floor == null ? "" : "" + floor + "层";
            roomNumber = string.IsNullOrEmpty(roomNumber) ? "" : roomNumber + "房";
            return department + building + floorStr + roomNumber;
        }
    }
}



/*
 * view: 小程序_现场查询
 SELECT   dbo.通用_占用者一览表.占用者ID AS ID, dbo.通用_占用者一览表.占用情况, dbo.通用_占用者一览表.房产单元编号, 
                dbo.通用_占用者一览表.占用者名称, dbo.通用_占用者一览表.占用者身份, dbo.通用_占用者一览表.联系电话, 
                dbo.通用_占用者一览表.联系地址, dbo.通用_占用者一览表.紧急联系人, dbo.通用_占用者一览表.紧急联系人电话, 
                dbo.通用_占用者一览表.紧急联系人地址, dbo.通用_占用者一览表.建筑面积, SUM(dbo.通用_未交费明细.未收合计) 
                AS 当前欠款, dbo.资源资料_房产单元.房号, dbo.资源资料_房产单元.层数, dbo.资源资料_房产单元.所属楼宇, 
                dbo.资源资料_房产单元.部门, dbo.通用_占用者一览表.帐套代码, dbo.通用_占用者一览表.帐套名称
FROM      dbo.通用_占用者一览表 LEFT OUTER JOIN
                dbo.资源资料_房产单元 ON dbo.通用_占用者一览表.房产单元ID = dbo.资源资料_房产单元.ID LEFT OUTER JOIN
                dbo.通用_未交费明细 ON dbo.通用_占用者一览表.占用者ID = dbo.通用_未交费明细.占用者ID AND 
                dbo.通用_占用者一览表.房产单元ID = dbo.通用_未交费明细.房产单元ID AND 
                dbo.通用_占用者一览表.房产单元编号 = dbo.通用_未交费明细.房产单元编号
GROUP BY dbo.通用_占用者一览表.房产单元编号, dbo.通用_占用者一览表.占用者名称, dbo.通用_占用者一览表.占用者身份, 
                dbo.通用_占用者一览表.联系电话, dbo.通用_占用者一览表.联系地址, dbo.通用_占用者一览表.紧急联系人, 
                dbo.通用_占用者一览表.紧急联系人电话, dbo.通用_占用者一览表.紧急联系人地址, dbo.通用_占用者一览表.建筑面积, 
                dbo.通用_占用者一览表.占用者ID, dbo.资源资料_房产单元.房号, dbo.资源资料_房产单元.层数, 
                dbo.资源资料_房产单元.所属楼宇, dbo.资源资料_房产单元.部门, dbo.通用_占用者一览表.占用情况, 
                dbo.通用_占用者一览表.帐套代码, dbo.通用_占用者一览表.帐套名称 
 */

/*
 * view: 通用_占用者一览表
 SELECT   CASE WHEN isnull(占用终止, 0) = 1 THEN '占用终止' ELSE '正在占用' END AS 占用情况, 
            CASE WHEN 资源占用表.资源表名称 = '资源资料_房产单元' THEN 编号 ELSE 房产单元编号 END AS 房产单元编号, 
            dbo.通用_资源明细_房产单元.房产单元名称, dbo.资源占用者.占用者名称, dbo.资源占用者身份.占用者身份, 
            dbo.资源占用表.资源表编号, STUFF(dbo.资源占用表.资源表名称, 1, 5, '') AS 资源类型, dbo.资源占用表.资源表ID, 
            CASE WHEN 资源占用表.资源表名称 = '资源资料_房产单元' THEN 资源资料_房产单元.ID ELSE 资源依附表.房产单元ID END
             AS 房产单元ID, dbo.资源占用表.占用者ID, dbo.资源占用表.费用项目ID, dbo.资源占用表.ID AS 占用表ID, 
            dbo.资源占用表.资源表名称, dbo.资源占用表.帐套代码, dbo.资源占用表.费用计算公式ID, 
            dbo.资源资料_房产单元.建筑面积
FROM      dbo.资源占用表 LEFT OUTER JOIN
            dbo.资源资料_房产单元 ON dbo.资源占用表.资源表ID = dbo.资源资料_房产单元.ID AND 
            dbo.资源占用表.资源表编号 = dbo.资源资料_房产单元.编号 LEFT OUTER JOIN
            dbo.资源依附表 ON dbo.资源占用表.资源表ID = dbo.资源依附表.资源表ID AND 
            dbo.资源占用表.资源表名称 = dbo.资源依附表.资源表名称 LEFT OUTER JOIN
            dbo.通用_资源明细_房产单元 ON dbo.资源依附表.房产单元ID = dbo.通用_资源明细_房产单元.房产单元ID INNER JOIN
            dbo.资源占用者身份 ON dbo.资源占用表.占用者身份代码 = dbo.资源占用者身份.代码 INNER JOIN
            dbo.资源占用者 ON dbo.资源占用表.占用者ID = dbo.资源占用者.ID
GROUP BY CASE WHEN isnull(占用终止, 0) = 1 THEN '占用终止' ELSE '正在占用' END, 
            CASE WHEN 资源占用表.资源表名称 = '资源资料_房产单元' THEN 编号 ELSE 房产单元编号 END, 
            dbo.通用_资源明细_房产单元.房产单元名称, dbo.资源占用者.占用者名称, dbo.资源占用者身份.占用者身份, 
            dbo.资源占用表.资源表编号, STUFF(dbo.资源占用表.资源表名称, 1, 5, ''), dbo.资源占用表.资源表ID, 
            CASE WHEN 资源占用表.资源表名称 = '资源资料_房产单元' THEN 资源资料_房产单元.ID ELSE 资源依附表.房产单元ID END,
             dbo.资源占用表.占用者ID, dbo.资源占用表.费用项目ID, dbo.资源占用表.ID, dbo.资源占用表.资源表名称, 
            dbo.资源占用表.帐套代码, dbo.资源占用表.费用计算公式ID, dbo.资源资料_房产单元.建筑面积
 */

/*
 * view: 通用_未交费明细
 SELECT   TOP (100) PERCENT CASE WHEN 房产单元编号 IS NULL 
            THEN 应收款.资源表编号 ELSE 房产单元编号 END AS 房产单元编号, dbo.通用_资源明细_房产单元.房产单元名称, 
            dbo.应收款.资源表编号, STUFF(dbo.应收款.资源表名称, 1, 5, '') AS 资源类型, dbo.资源占用者.占用者名称, 
            dbo.应收款.占用者身份, dbo.费用项目.费用名称, dbo.应收款.费用说明, 
            CASE WHEN 产生来源 = 0 THEN '批次' WHEN 产生来源 = 1 THEN '零星' WHEN 产生来源 = 2 THEN '预交' WHEN 产生来源
             = 3 THEN '预减免' WHEN 产生来源 = 5 THEN '缓交' WHEN 产生来源 = 4 THEN '变更' END AS 产生来源, 
            dbo.应收款.计费年月, dbo.应收款.计费年月开始日期, dbo.应收款.计费年月截至日期, dbo.应收款.概要编号, 
            dbo.应收款.应收日期, ISNULL(dbo.应收款.应收延期, 0) AS 应收延期, dbo.应收款.滞纳金计算日期, 
            ISNULL(dbo.应收款.滞纳金计算延期, 0) AS 滞纳金计算延期, ISNULL(dbo.应收款.应收金额, 0) AS 应收金额, 
            ISNULL(dbo.应收款.优惠金额, 0) AS 优惠金额, ISNULL(dbo.应收款.滞纳金金额, 0) AS 滞纳金金额, 
            ISNULL(dbo.应收款.应收金额, 0) - ISNULL(dbo.应收款.优惠金额, 0) + ISNULL(dbo.应收款.滞纳金金额, 0) AS 未收合计, 
            ISNULL(dbo.资源占用表.占用终止, 0) AS 占用终止, ISNULL(dbo.资源占用变更表.变更状态, 0) AS 变更状态, 
            ISNULL(dbo.应收款.收费状态, '未交') AS 收费状态, dbo.应收款.房产单元ID, dbo.应收款.占用者ID, dbo.应收款.资源表ID, 
            dbo.应收款.ID AS 应收款ID, dbo.应收款.占用表ID, dbo.应收款.帐套代码, dbo.费用项目.费用种类, 
            dbo.通用_资源明细_房产单元.性质, CASE WHEN isnull(通用_资源明细_房产单元.性质, '') = '住宅' OR
            isnull(通用_资源明细_房产单元.性质, '') = '公寓' OR
            isnull(通用_资源明细_房产单元.性质, '') = '商住' THEN '住宅' WHEN isnull(通用_资源明细_房产单元.性质, '') 
            = '商铺' THEN '商业' ELSE '其它' END AS 房产性质, dbo.费用项目.属性, 
            CASE WHEN 占用者身份 = '开发商' THEN '开发商' ELSE '小业主' END AS 占用者身份类别, 
            dbo.资源帐套表.帐套名称 + N'-' + ISNULL(dbo.通用_资源明细_房产单元.所属组团, N'未知') AS 管理处分区, 
            dbo.资源帐套表.帐套名称, dbo.通用_资源明细_房产单元.客服专员, dbo.通用_资源明细_房产单元.所属组团, 
            dbo.通用_资源明细_房产单元.建筑面积
FROM      dbo.应收款 LEFT OUTER JOIN
            dbo.资源帐套表 ON dbo.应收款.帐套代码 = dbo.资源帐套表.帐套代码 LEFT OUTER JOIN
            dbo.资源占用者 ON dbo.应收款.占用者ID = dbo.资源占用者.ID LEFT OUTER JOIN
            dbo.费用项目 ON dbo.应收款.费用项目ID = dbo.费用项目.ID LEFT OUTER JOIN
            dbo.通用_资源明细_房产单元 ON dbo.应收款.房产单元ID = dbo.通用_资源明细_房产单元.房产单元ID LEFT OUTER JOIN
            dbo.资源占用表 ON dbo.应收款.占用表ID = dbo.资源占用表.ID LEFT OUTER JOIN
            dbo.资源占用变更表 ON dbo.应收款.变更表ID = dbo.资源占用变更表.ID
WHERE   (ISNULL(dbo.应收款.收费状态, N'未交') = '未交') AND (ISNULL(dbo.应收款.应收金额, 0) - ISNULL(dbo.应收款.优惠金额, 
            0) + ISNULL(dbo.应收款.滞纳金金额, 0) <> 0) AND (dbo.应收款.帐套代码 <> '16')
 */
