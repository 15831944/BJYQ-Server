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
    public class HouseDal
    {
        public static StatusReport GetHouseDetail(string roomNumber)
        {
            string sqlString = "select 房号,层数,建筑面积,所属楼宇,性质,房产类型,户型,部门,所属单元 " +
                " from 资源资料_房产单元 " +
                " where 编号 = @编号 ";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@编号", roomNumber));
            if(dt.Rows.Count == 0)
            {
                return new StatusReport().SetFail();
            }
            DataRow dr = dt.Rows[0];
            House house = new House();
            house.roomNum = DataTypeHelper.GetStringValue(dr["房号"]);
            house.floor = DataTypeHelper.GetStringValue(dr["层数"]);
            house.area = DataTypeHelper.GetDecimalValue(dr["建筑面积"]);
            house.building = DataTypeHelper.GetStringValue(dr["所属楼宇"]);
            house.property = DataTypeHelper.GetStringValue(dr["性质"]);
            house.type = DataTypeHelper.GetStringValue(dr["房产类型"]);
            house.houseType = DataTypeHelper.GetStringValue(dr["户型"]);
            house.department = DataTypeHelper.GetStringValue(dr["部门"]);
            house.unit = DataTypeHelper.GetStringValue(dr["所属单元"]);
            return new StatusReport(house);
        }

    }
}