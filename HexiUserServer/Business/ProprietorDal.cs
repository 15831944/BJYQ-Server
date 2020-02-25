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
            Proprietor proprietor = new Proprietor();
            DataTable dt = null;
            //List<object>  
            //TODO:目前无法解决openid绑定后，系统中又添加了同名占用者或同一占用者又占用了其他房产的问题
            string sqlString = "select top 1 姓名,联系人手机 from dbo.小程序_业主信息 where openid = @openid";
            DataTable dtProprietor = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@openid", openId));
            if (dtProprietor.Rows.Count > 0)
            {
                DataRow drProprietor = dtProprietor.Rows[0];
                sqlString = " SELECT ID, 房产单元ID, 房产单元编号, 姓名, 联系人手机, 帐套名称, 帐套代码, 是否占用者本人 " +
                            " FROM dbo.小程序_业主信息 " +
                            " where 姓名 = @name and 联系人手机 = @phone ";
                dt = SQLHelper.ExecuteQuery("wyt", sqlString, 
                                new SqlParameter("@name", DataTypeHelper.GetStringValue(drProprietor["姓名"])),
                                new SqlParameter("@phone", DataTypeHelper.GetStringValue(drProprietor["联系人手机"])));
                foreach(DataRow dr in dt.Rows)
                {

                }
            }
            else
            {
                sqlString = "select top 1 姓名,联系人手机 from dbo.小程序_业主亲属信息 where openid = @openid";
                DataTable dtFamily = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@openid", openId));
                if (dtFamily.Rows.Count > 0)
                {
                    DataRow drFamily = dtFamily.Rows[0];
                    sqlString = " SELECT ID, 房产单元ID, 房产单元编号, 姓名, 联系人手机, 帐套名称, 帐套代码, 是否占用者本人 " +
                                " FROM dbo.小程序_业主亲属信息 " +
                                 " where 姓名 = @name and 联系人手机 = @phone ";
                    dt = SQLHelper.ExecuteQuery("wyt", sqlString,
                               new SqlParameter("@name", DataTypeHelper.GetStringValue(drFamily["姓名"])),
                               new SqlParameter("@phone", DataTypeHelper.GetStringValue(drFamily["联系人手机"])));
                    foreach (DataRow dr in dt.Rows)
                    {

                    }
                }
                else
                {

                }
            }



            //string sqlStr =
            //    " if exists (select ID from dbo.小程序_业主信息 where openid = @openid) " +
            //    " SELECT ID, 房产单元ID, 房产单元编号, 姓名, 联系人手机, 帐套名称, 帐套代码, 是否占用者本人 " +
            //    " FROM dbo.小程序_业主信息 " +
            //    " where openid = @openid " +
            //    " else " +
            //    " SELECT ID, 房产单元ID, 房产单元编号, 姓名, 联系人手机, 帐套名称, 帐套代码, 是否占用者本人 " +
            //    " FROM dbo.小程序_业主亲属信息 " +
            //    " where openid = @openid ";
            //DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlStr, new SqlParameter("@openid", openId));
            //if (dt.Rows.Count == 0)
            //{
            //    sr.status = "Fail";
            //    sr.result = "无此用户";
            //    return sr;
            //}
            //var dataTable = from dtt in dt select dtt;

            //Proprietor proprietor = new Proprietor();
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(DataTypeHelper.GetStringValue(dr["姓名"])))
                {
                    continue;
                }
                else
                {
                    proprietor.Id = DataTypeHelper.GetIntValue(dr["ID"]);
                    proprietor.Name = DataTypeHelper.GetStringValue(dr["姓名"]);
                    proprietor.Phone = DataTypeHelper.GetStringValue(dr["联系人手机"]);
                    proprietor.IsProprietor = DataTypeHelper.GetStringValue(dr["是否占用者本人"]);
                    break;
                }
            }
           
            List<RoomInfo> pList = new List<RoomInfo>();
            foreach (DataRow datarow in dt.Rows)
            {
                RoomInfo roomInfo = new RoomInfo();
                roomInfo.RoomNumber = DataTypeHelper.GetStringValue(datarow["房产单元编号"]);
                roomInfo.RoomId = DataTypeHelper.GetIntValue(datarow["房产单元ID"]);
                roomInfo.ZTName = DataTypeHelper.GetStringValue(datarow["帐套名称"]);
                roomInfo.ZTCode = DataTypeHelper.GetStringValue(datarow["帐套代码"]);
                pList.Add(roomInfo);
            }
            proprietor.Room = pList.ToArray();
            return sr.SetSuccess(proprietor);
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static StatusReport getCode(string userName, string phoneNumber)
        {
            StatusReport sr = new StatusReport();
            if (CheckProprietorExist(userName, phoneNumber))
            {
                Random random = new Random();
                int randomNumber = random.Next(100000, 999999);
                return sr.SetSuccess(randomNumber.ToString());
            }
            else
            {
                return sr.SetFail("业主不存在，请核对姓名和手机号后重试，如有疑问，敬请联系物业公司");
            }
        }

        /// <summary>
        /// 将微信OpenId和系统中的占用者ID绑定，保存到微信占用者绑定表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static StatusReport BindProprietor(string name, string phoneNumber, string openId)
        {
            string sqlString =
                " if exists (select ID from 资源占用者 where 占用者名称 = @姓名 and 联系人手机 = @联系人手机) " +
                " update 资源占用者 set openid = @openid where 占用者名称 = @姓名 and 联系人手机 = @联系人手机 " +
                " else " +
                " update 占用资料_占用人员详情 set openid = @openid where 姓名 = @姓名 and 联系电话 = @联系人手机 ";

            StatusReport sr = SQLHelper.Update("wyt", sqlString,
                new SqlParameter("@OpenID", openId),
                new SqlParameter("@姓名", name),
                new SqlParameter("@联系人手机", phoneNumber));

            return sr;
        }

        public static StatusReport GetFamilyMembers(string name, string phone)
        {
            StatusReport sr = new StatusReport();
            string sqlString = " select ID,PID,姓名,与登记占用者关系,联系电话 " +
                " from 占用资料_占用人员详情 " +
                " where PID in (select ID from 占用资料 where 占用者ID in (select ID from 小程序_业主信息 where 姓名 = @姓名 and 联系人手机 = @联系人手机) ) "; 
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@姓名", name), new SqlParameter("@联系人手机", phone));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "获取亲属信息失败或还未添加亲属信息";
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
                    //gender = DataTypeHelper.GetStringValue(dr["性别"]),
                    //birth = DataTypeHelper.GetDateStringValue(dr["出生日期"]),
                    //idType = DataTypeHelper.GetStringValue(dr["身份证件名称"]),
                    //idNumber = DataTypeHelper.GetStringValue(dr["身份证件号码"]),
                    //nation = DataTypeHelper.GetStringValue(dr["国籍或地区"]),
                    relation = DataTypeHelper.GetStringValue(dr["与登记占用者关系"]),
                    //company = DataTypeHelper.GetStringValue(dr["工作单位"]),
                    phone = DataTypeHelper.GetStringValue(dr["联系电话"])
                };
                fmList.Add(fm);
            }
            sr.status = "Success";
            sr.result = "成功";
            sr.data = fmList.ToArray();
            return sr;
        }

        public static StatusReport AddFamily(int id, string phoneNumber, string relation, string userName, string roomId)
        {
            StatusReport sr = new StatusReport();
            string sqlStr = "if not exists (select ID from 占用资料 where 占用者ID=@占用者ID and 资源表ID = @资源表ID) " +
                " insert into 占用资料 (资源表名称,资源表ID,占用者ID,占用性质) " +
                " select @资源表名称, @资源表ID ,@占用者ID, @占用性质 " +
                " select @@IDENTITY ";
            sr = SQLHelper.Insert("wyt", sqlStr,
                new SqlParameter("@资源表名称", "资源资料_房产单元"),
                new SqlParameter("@资源表ID", roomId),
                new SqlParameter("@占用者ID", id),
                new SqlParameter("@占用性质", "正常"));
            if (sr.result == "数据已存在" || sr.status == "Success")
            {
                string sqlString = "insert into 占用资料_占用人员详情 (PID,姓名,与登记占用者关系,联系电话) " +
            " select ID, @姓名,@与登记占用者关系,@联系电话 from 占用资料 where 占用者ID = @占用者ID " +
            " select @@IDENTITY ";
                sr = SQLHelper.Insert("wyt", sqlString,
                    new SqlParameter("@姓名", DataTypeHelper.GetDBValue(userName)),
                    new SqlParameter("@与登记占用者关系", DataTypeHelper.GetDBValue(relation)),
                    new SqlParameter("@联系电话", DataTypeHelper.GetDBValue(phoneNumber)),
                    new SqlParameter("@占用者ID", id));
                
            }
            return sr;
            //string[] roomIds = roomId.Split(',');
            //for (int i = 0; i < roomIds.Length; i++)
            //{
            //    string sqlStr = "if not exists (select ID from 占用资料 where 占用者ID=@占用者ID and 资源表ID = @资源表ID) " +
            //    " insert into 占用资料 (资源表名称,资源表ID,占用者ID,占用性质) " +
            //    " select @资源表名称, @资源表ID ,@占用者ID, @占用性质 " +
            //    " select @@IDENTITY ";
            //    sr = SQLHelper.Insert("wyt", sqlStr,
            //        new SqlParameter("@资源表名称", "资源资料_房产单元"),
            //        new SqlParameter("@资源表ID", roomIds[i]),
            //        new SqlParameter("@占用者ID", id),
            //        new SqlParameter("@占用性质", "正常"));
            //    if (sr.result == "数据已存在" || sr.status == "Success")
            //    {
            //        string sqlString = "insert into 占用资料_占用人员详情 (PID,姓名,与登记占用者关系,联系电话) " +
            //    " select ID, @姓名,@与登记占用者关系,@联系电话 from 占用资料 where 占用者ID = @占用者ID " +
            //    " select @@IDENTITY ";
            //        sr = SQLHelper.Insert("wyt", sqlString,
            //            new SqlParameter("@姓名", DataTypeHelper.GetDBValue(userName)),
            //            new SqlParameter("@与登记占用者关系", DataTypeHelper.GetDBValue(relation)),
            //            new SqlParameter("@联系电话", DataTypeHelper.GetDBValue(phoneNumber)),
            //            new SqlParameter("@占用者ID", id));
            //        if (sr.status == "Success")
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
            //return sr;

        }

        //public static StatusReport AddFamily(int id, string gender, string address, string birth, string company, string idNumber, string idType, string job, string nation, string nationality, string phoneNumber, string relation, string userName, string[] roomId)
        //{
        //    StatusReport sr = new StatusReport();

        //    for(int i = 0; i < roomId.Length; i++)
        //    {
        //        string sqlStr = "if not exists (select ID from 占用资料 where 占用者ID=@占用者ID and 资源表ID = @资源表ID) " +
        //        " insert into 占用资料 (资源表名称,资源表ID,占用者ID,占用性质) " +
        //        " select @资源表名称, @资源表ID ,@占用者ID, @占用性质 " +
        //        " select @@IDENTITY ";
        //        sr = SQLHelper.Insert("wyt", sqlStr,
        //            new SqlParameter("@资源表名称", "资源资料_房产单元"),
        //            new SqlParameter("@资源表ID", roomId[i]),
        //            new SqlParameter("@占用者ID", id),
        //            new SqlParameter("@占用性质", "正常"));
        //        if (sr.result == "数据已存在" || sr.status == "Success")
        //        {
        //            string sqlString = "insert into 占用资料_占用人员详情 (PID,姓名,性别,出生日期,身份证件名称,身份证件号码,国籍或地区,与登记占用者关系,工作单位,联系电话,民族,职务,住址) " +
        //        " select ID, @姓名,@性别,@出生日期,@身份证件名称,@身份证件号码,@国籍或地区,@与登记占用者关系,@工作单位,@联系电话,@民族,@职务,@住址 from 占用资料 where 占用者ID = @占用者ID " +
        //        " select @@IDENTITY ";
        //            sr = SQLHelper.Insert("wyt", sqlString,
        //                new SqlParameter("@姓名", DataTypeHelper.GetDBValue(userName)),
        //                new SqlParameter("@性别", DataTypeHelper.GetDBValue(gender)),
        //                new SqlParameter("@出生日期", DataTypeHelper.GetDBValue(birth)),
        //                new SqlParameter("@身份证件名称", DataTypeHelper.GetDBValue(idType)),
        //                new SqlParameter("@身份证件号码", DataTypeHelper.GetDBValue(idNumber)),
        //                new SqlParameter("@国籍或地区", DataTypeHelper.GetDBValue(nationality)),
        //                new SqlParameter("@与登记占用者关系", DataTypeHelper.GetDBValue(relation)),
        //                new SqlParameter("@工作单位", DataTypeHelper.GetDBValue(company)),
        //                new SqlParameter("@联系电话", DataTypeHelper.GetDBValue(phoneNumber)),
        //                new SqlParameter("@民族", DataTypeHelper.GetDBValue(nation)),
        //                new SqlParameter("@职务", DataTypeHelper.GetDBValue(job)),
        //                new SqlParameter("@住址", DataTypeHelper.GetDBValue(address)),
        //                new SqlParameter("@占用者ID", id));
        //            if (sr.status == "Success")
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    return sr;

        //}


        /// <summary>
        /// 判断该占用者是否存在
        /// 在资源占用者表中查找，如果不存在，再去占用资料_占用人员详情表中查找，查到ID返回True，否则返回False
        /// </summary>
        /// <param name="name"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        private static bool CheckProprietorExist(string userName, string phoneNumber)
        {
            string sqlString =
               " if exists (select ID from 资源占用者 where 占用者名称 = @占用者名称 and 联系人手机 like '%" + phoneNumber + "%' ) " +
               " begin " +
               " select " +
               " ID " +
               " from 资源占用者 " +
               " where 占用者名称 = @占用者名称 and 联系人手机 like '%" + phoneNumber + "%' " +
               " select @@IDENTITY " +
               " end " +
               " else " +
               " begin " +
               " select " +
               " ID " +
               " from 占用资料_占用人员详情 " +
               " where 姓名 = @占用者名称 and 联系电话 like '%" + phoneNumber + "%' " +
               " select @@IDENTITY " +
               " end ";
            int id = SQLHelper.ExecuteScalar("wyt", sqlString,
                new SqlParameter("@占用者名称", userName));
            return id > 0 ? true : false;
           
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