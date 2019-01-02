using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using HexiUtils;
using HexiUserServer.Models;

namespace HexiUserServer.Business
{
    public class ProprietorDal
    {
        /// <summary>
        /// 判断该OpenId是否对应系统中的某占用者
        /// 如果对应，则返回该占用者的信息
        /// 如果不对应，则返回错误信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static StatusReport CheckOpenIdExist(string openId)
        {
            StatusReport sr = new StatusReport();

            //string sqlString =
            //    "select " +
            //    "占用者ID " +
            //    "from 基础资料_微信占用者绑定表 " +
            //    "where OpenID = @OpenId";

            //int proprietorId = SQLHelper.ExecuteScalar("wyt", sqlString,
            //    new SqlParameter("@OpenId", openId));

            //if (proprietorId > 0)
            //{
            string sqlStr =
                " SELECT ID, 房产单元ID, 房产单元编号, 姓名, 联系电话, 帐套名称, 帐套代码, 客服专员 ,是否占用者本人 " +
                " FROM dbo.小程序_业主信息 " +
                " where OpenID = @OpenID";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlStr, new SqlParameter("@OpenID", openId));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "无此用户";
                return sr;
            }
            //var dataTable = from dtt in dt select dtt;
            DataRow dr = dt.Rows[0];
            Proprietor proprietor = new Proprietor();
            proprietor.Id = DataTypeHelper.GetIntValue(dr["ID"]);
            //proprietor.RoomId = DataTypeHelper.GetIntValue(dr["房产单元ID"]);
            //proprietor.RoomNumber = DataTypeHelper.GetStringValue(dr["房产单元编号"]);
            proprietor.Name = DataTypeHelper.GetStringValue(dr["姓名"]);
            proprietor.Phone = DataTypeHelper.GetStringValue(dr["联系电话"]);
            proprietor.ZTName = DataTypeHelper.GetStringValue(dr["帐套名称"]);
            proprietor.ZTCode = DataTypeHelper.GetStringValue(dr["帐套代码"]);
            proprietor.Server = DataTypeHelper.GetStringValue(dr["客服专员"]);
            proprietor.IsProprietor = DataTypeHelper.GetStringValue(dr["是否占用者本人"]);
            List<RoomInfo> pList = new List<RoomInfo>();
            foreach (DataRow datarow in dt.Rows)
            {
                RoomInfo roomInfo = new RoomInfo();
                roomInfo.RoomNumber = DataTypeHelper.GetStringValue(datarow["房产单元编号"]);
                roomInfo.RoomId = DataTypeHelper.GetIntValue(datarow["房产单元ID"]);
                pList.Add(roomInfo);
            }
            proprietor.Room = pList.ToArray();
            sr.status = "Success";
            sr.result = "成功";
            sr.data = JsonConvert.SerializeObject(proprietor);
            return sr;
            //}
            //else
            //{
            //    sr.status = "Fail";
            //    sr.result = "无此用户";
            //    return sr;
            //}
        }



        /// <summary>
        /// 判断该占用者是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static int CheckProprietorExist(string userName, string phoneNumber)
        {
            //string sqlString =
            //    "select " +
            //    "ID " +
            //    "from 用户 " +
            //    "where UserCode = @UserCode and Password = @Password";

            //int id = SQLHelper.ExecuteScalar(sqlString,
            //    new SqlParameter("@UserCode",name),
            //    new SqlParameter("@Password",password));
            string sqlString =
               "select " +
               "ID " +
               "from 资源占用者 " +
               "where 占用者名称 = @占用者名称 and 联系电话 like '%" + phoneNumber + "%' " +
               "select @@IDENTITY";

            int id = SQLHelper.ExecuteScalar("wyt", sqlString,
                new SqlParameter("@占用者名称", userName));

            if (id > 0)
            {
                return id;
            }
            else
            {
                sqlString = "select ID from 资源占用者 where ID = " +
                    " (select 占用者ID from 占用资料 where ID = " +
                    " (select PID from 占用资料_占用人员详情 where 姓名 = @姓名 and 联系电话 like '%" + phoneNumber + "%')) " +
                    " select @@IDENTITY ";
                id = SQLHelper.ExecuteScalar("wyt", sqlString, new SqlParameter("@姓名", userName));
                if (id > 0)
                {
                    return -(id);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 将微信OpenId和系统中的占用者ID绑定，保存到微信占用者绑定表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static StatusReport BindProprietor(int id, string name, string phoneNumber, string openId, bool isProprietor)
        {
            string sqlString =
                "if not exists (select ID from 基础资料_微信占用者绑定表 where 占用者ID=@占用者ID and openid = @OpenID) "+
                "insert into 基础资料_微信占用者绑定表 " +
                "(占用者ID , openid, 姓名, 联系电话, 是否占用者本人) " +
                "select @占用者ID, @OpenID, @姓名, @联系电话, @是否占用者本人 " +
                "select @@IDENTITY ";

            StatusReport sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@占用者ID", id),
                new SqlParameter("@OpenID", openId),
                new SqlParameter("@姓名", name),
                new SqlParameter("@联系电话", phoneNumber),
                new SqlParameter("@是否占用者本人", isProprietor ? "是" : "否"));

            return sr;
        }

        public static StatusReport GetFamilyMembers(int id)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " select ID,PID,姓名,性别,出生日期,身份证件名称,身份证件号码, " +
                " 国籍或地区,与登记占用者关系,工作单位,联系电话 " +
                " from 占用资料_占用人员详情 " +
                " where PID = (select ID from 占用资料 where 占用者ID = @占用者ID) "; 
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@占用者ID", id));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未查询到符合条件的记录";
                return sr;
            }
            List<FamilyMember> fmList = new List<FamilyMember>();
            foreach (DataRow dr in dt.Rows)
            {
                FamilyMember fm = new FamilyMember()
                {
                    id = DataTypeHelper.GetIntValue(dr["ID"]),
                    pid = DataTypeHelper.GetIntValue(dr["PID"]),
                    name = DataTypeHelper.GetStringValue(dr["姓名"]),
                    gender = DataTypeHelper.GetStringValue(dr["性别"]),
                    birth = DataTypeHelper.GetDateStringValue(dr["出生日期"]),
                    idType = DataTypeHelper.GetStringValue(dr["身份证件名称"]),
                    idNumber = DataTypeHelper.GetStringValue(dr["身份证件号码"]),
                    nation = DataTypeHelper.GetStringValue(dr["国籍或地区"]),
                    relation = DataTypeHelper.GetStringValue(dr["与登记占用者关系"]),
                    company = DataTypeHelper.GetStringValue(dr["工作单位"]),
                    phone = DataTypeHelper.GetStringValue(dr["联系电话"])
                };
                fmList.Add(fm);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = fmList.ToArray();
            return sr;
        }

        public static StatusReport AddFamily(int id, string gender, string address, string birth, string company, string idNumber, string idType, string job, string nation, string nationality, string phoneNumber, string relation, string userName, string[] roomId)
        {
            StatusReport sr = new StatusReport();

            for(int i = 0; i < roomId.Length; i++)
            {
                string sqlStr = "if not exists (select ID from 占用资料 where 占用者ID=@占用者ID and 资源表ID = @资源表ID) " +
                " insert into 占用资料 (资源表名称,资源表ID,占用者ID,占用性质) " +
                " select @资源表名称, @资源表ID ,@占用者ID, @占用性质 " +
                " select @@IDENTITY ";
                sr = SQLHelper.Insert("wyt", sqlStr,
                    new SqlParameter("@资源表名称", "资源资料_房产单元"),
                    new SqlParameter("@资源表ID", roomId[i]),
                    new SqlParameter("@占用者ID", id),
                    new SqlParameter("@占用性质", "正常"));
                if (sr.result == "数据已存在" || sr.status == "Success")
                {
                    string sqlString = "insert into 占用资料_占用人员详情 (PID,姓名,性别,出生日期,身份证件名称,身份证件号码,国籍或地区,与登记占用者关系,工作单位,联系电话,民族,职务,住址) " +
                " select ID, @姓名,@性别,@出生日期,@身份证件名称,@身份证件号码,@国籍或地区,@与登记占用者关系,@工作单位,@联系电话,@民族,@职务,@住址 from 占用资料 where 占用者ID = @占用者ID " +
                " select @@IDENTITY ";
                    sr = SQLHelper.Insert("wyt", sqlString,
                        new SqlParameter("@姓名", userName),
                        new SqlParameter("@性别", gender),
                        new SqlParameter("@出生日期", birth),
                        new SqlParameter("@身份证件名称", idType),
                        new SqlParameter("@身份证件号码", idNumber),
                        new SqlParameter("@国籍或地区", nationality),
                        new SqlParameter("@与登记占用者关系", relation),
                        new SqlParameter("@工作单位", company),
                        new SqlParameter("@联系电话", phoneNumber),
                        new SqlParameter("@民族", nation),
                        new SqlParameter("@职务", job),
                        new SqlParameter("@住址", address),
                        new SqlParameter("@占用者ID", id));
                    if (sr.status == "Success")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return sr;

        }


        //private static Employee GetZTInfo(Employee employee, string[] ztcodes)
        //{
        //    List<ZT> zts = new List<ZT>();

        //    for (int i = 0; i < ztcodes.Length; i++)
        //    {
        //        string ztcode = ztcodes[i];
        //        string sqlString = "select 帐套代码,帐套名称 from 资源帐套表 where 帐套代码 = @帐套代码";
        //        DataTable dt = SQLHelper.ExecuteQuery(sqlString, new SqlParameter("@帐套代码", ztcode));
        //        DataRow dr = dt.Rows[0];
        //        ZT zt = new ZT((string)dr["帐套代码"], (string)dr["帐套名称"]);
        //        zts.Add(zt);
        //    }
        //    employee.ZTInfo = zts.ToArray();
        //    return employee;
        //}

        //private static Employee GetJurisdictionInfo(Employee employee)
        //{

        //    return null;
        //}
    }
}