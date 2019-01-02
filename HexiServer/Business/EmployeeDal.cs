using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using HexiServer.Common;
using HexiServer.Models;
using System.Security.Cryptography;
using System.Text;
using HexiUtils;

namespace HexiServer.Business
{
    public class EmployeeDal
    {
        /// <summary>
        /// 判断该OpenId是否对应系统中的某员工
        /// 如果对应，则返回该员工的信息
        /// 如果不对应，则返回错误信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static StatusReport CheckOpenIdExist(string openId)
        {
            StatusReport sr = new StatusReport();

            string sqlString =
                "select " +
                "用户ID " +
                "from 基础资料_微信员工绑定表 " +
                "where openid = @OpenId";

            int empId = SQLHelper.ExecuteScalar("wyt", sqlString,
                new SqlParameter("@OpenId", openId));
            
            if (empId > 0)
            {
                string sqlStr =
                    "select " +
                    "ID,UserCode,UserName,员工ID,ZTCodes " +
                    "from 用户 " +
                    "where ID = @ID";

                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlStr, new SqlParameter("@ID", empId));
                DataRow dr = dt.Rows[0];

                Employee emp = new Employee()
                {
                    Id = Convert.ToInt32(dr["ID"]),
                    UserCode = (string)dr["UserCode"],
                    UserName = (string)dr["UserName"],
                    //ZTCodes = ((string)dr["ZTCodes"]).Split(',')
                };
                string zts = DataTypeHelper.GetStringValue(dr["ZTCodes"]);
                if (!string.IsNullOrEmpty(zts))
                {
                    string[] ztcodes = zts.Split(',');
                    emp = GetZTInfo(emp, ztcodes);
                }
                emp = GetJurisdictionInfo(emp, emp.UserName);
                sr.status = "Success";
                sr.result = "成功";
                sr.data = emp;
                return sr;
            }
            else
            {
                sr.status = "Fail";
                sr.result = "无此用户";
                return sr;
            }
        }



        /// <summary>
        /// 判断该员工是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static int CheckEmployeeExist(string userName,string password)
        {

            SHA1 sha1 = SHA1.Create();
            byte[] pw = sha1.ComputeHash(Encoding.Unicode.GetBytes(password));

            //string sqlString =
            //   "select " +
            //   "ID " +
            //   "from 用户 " +
            //   "where UserCode = @UserCode";

            string sqlString = "select ID, Password from 用户 where ltrim(rtrim(UserCode)) = @UserCode";

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@UserCode", userName));
            if (dt.Rows.Count == 0)
            {
                return 0;
            }
            DataRow dr = dt.Rows[0];
            int id = Convert.ToInt32(dr["ID"]);
            byte[] storedPassword = (byte[])dr["Password"];

            Boolean isEqual = ComparePasswords(storedPassword, pw);

            return isEqual ? id : 0;

            //int id = SQLHelper.ExecuteScalar(sqlString,
            //    new SqlParameter("@UserCode", userName));

            //if (id > 0)
            //{
            //    return id;
            //}
            //else
            //{
            //    return 0;
            //}
        }

        /// <summary>
        /// 将微信OpenId和系统中的用户Id绑定，保存到微信员工绑定表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static StatusReport BindEmployee(int id,string openId)
        {
            string sqlString =
                "if not exists (select 用户ID from 基础资料_微信员工绑定表 where 用户ID = @用户表Id) " +
                "insert into 基础资料_微信员工绑定表 " +
                "(用户ID , openid) " +
                "select " +
                "@用户表Id, @OpenId " +
                "select @@identity";

            StatusReport sr = SQLHelper.Insert("wyt", sqlString,
                new SqlParameter("@用户表Id", id),
                new SqlParameter("@OpenId", openId));

            return sr;
        }


        public static StatusReport CheckPassword(string userId, string password)
        {
            StatusReport sr = new StatusReport();
            SHA1 sha1 = SHA1.Create();
            byte[] pw = sha1.ComputeHash(Encoding.Unicode.GetBytes(password));
            string sqlString = "select Password from 用户 where ID = @userId";

            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@userId", userId));
            if (dt.Rows.Count == 0)
            {
                sr.status = "Fail";
                sr.result = "未找到此用户";
                return sr;
            }
            DataRow dr = dt.Rows[0];
            //int id = Convert.ToInt32(dr["ID"]);
            byte[] storedPassword = (byte[])dr["Password"];

            Boolean isEqual = ComparePasswords(storedPassword, pw);
            if (isEqual)
            {
                sr.status = "Success";
                sr.result = "匹配";
                return sr;
            }
            else
            {
                sr.status = "Fail";
                sr.result = "不匹配";
                return sr;
            }

            //return isEqual ? id : 0;
        }

        private static Employee GetJurisdictionInfo(Employee employee, string name)
        {
            string jurisdiction = "";
            string level = "";
            string sqlString = "select 权限,等级 from 基础资料_小程序员工权限配置 where 员工 = @员工";
            DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@员工", name));
            if (dt.Rows.Count == 0)
            {
                employee.Jurisdiction = null;
                employee.Level = null;
                return employee;
            }
            DataRow dr = dt.Rows[0];
            jurisdiction = DataTypeHelper.GetStringValue(dr["权限"]);
            level = DataTypeHelper.GetStringValue(dr["等级"]);
            employee.Jurisdiction = jurisdiction.Split(',');
            employee.Level = level.Split(',');
            return employee;
        }

        private static Employee GetZTInfo(Employee employee, string[] ztcodes)
        {
            List<ZT> zts = new List<ZT>();

            for (int i = 0; i < ztcodes.Length; i++)
            {
                string ztcode = ztcodes[i];
                string sqlString = "select 帐套代码,帐套名称 from 资源帐套表 where 帐套代码 = @帐套代码";
                DataTable dt = SQLHelper.ExecuteQuery("wyt", sqlString, new SqlParameter("@帐套代码", ztcode));
                DataRow dr = dt.Rows[0];
                ZT zt = new ZT((string)dr["帐套代码"], (string)dr["帐套名称"]);
                zts.Add(zt);
            }
            employee.ZTInfo = zts.ToArray();
            return employee;
        }

        private static Employee GetJurisdictionInfo(Employee employee)
        {

            return null;
        }

        private static Boolean ComparePasswords(byte[] storedPassword, byte[] hashedPassword)
        {
            int saltLength = 4;
            if (storedPassword.Length == 0 || hashedPassword.Length == 0 || storedPassword.Length - hashedPassword.Length != saltLength)
            {
                return false;
            }

            byte[] saltValue = new byte[saltLength];
            int saltOffset = storedPassword.Length - saltLength;
            for (int i = 0; i < saltLength; i++)
            {
                saltValue[i] = storedPassword[saltOffset + i];
            }
            byte[] saltedPassword = CreateSaltedPassword(saltValue, hashedPassword);
            return CompareByteArray(storedPassword,saltedPassword);
        }

        private static Boolean CompareByteArray(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static byte[] CreateSaltedPassword (byte[] saltValue, byte[] unsaltedPassword)
        {
            byte[] rawSalted = new byte[unsaltedPassword.Length + saltValue.Length];
            unsaltedPassword.CopyTo(rawSalted, 0);
            saltValue.CopyTo(rawSalted, unsaltedPassword.Length);
            SHA1 sha1 = SHA1.Create();
            byte[] saltedPassword = sha1.ComputeHash(rawSalted);
            byte[] dbPassword = new byte[saltedPassword.Length + saltValue.Length];
            saltedPassword.CopyTo(dbPassword, 0);
            saltValue.CopyTo(dbPassword, saltedPassword.Length);
            return dbPassword;
        }
    }
}



/**
 *  
 *  Dim sha1 As sha1 = sha1.Create()
    Dim pw As Byte() = sha1.ComputeHash(Encoding.Unicode.GetBytes(password))
 *  
 *  ' compare the hashed password against the stored password
    Private Function ComparePasswords(ByVal storedPassword As Byte(), ByVal hashedPassword As Byte()) As Boolean
        If ((storedPassword Is Nothing) OrElse (hashedPassword Is Nothing) OrElse (hashedPassword.Length <> storedPassword.Length - saltLength)) Then
            Return False
        End If

        ' get the saved saltValue
        Dim saltValue(saltLength - 1) As Byte
        Dim saltOffset As Integer = storedPassword.Length - saltLength
        Dim i As Integer = 0
        For i = 0 To saltLength - 1
            saltValue(i) = storedPassword(saltOffset + i)
        Next

        Dim saltedPassword As Byte() = CreateSaltedPassword(saltValue, hashedPassword)

        ' compare the values
        Return CompareByteArray(storedPassword, saltedPassword)
    End Function

    ' compare the contents of two byte arrays
    Private Function CompareByteArray(ByVal array1 As Byte(), ByVal array2 As Byte()) As Boolean
        If (array1.Length <> array2.Length) Then
            Return False
        End If

        Dim i As Integer
        For i = 0 To array1.Length - 1
            If (array1(i) <> array2(i)) Then
                Return False
            End If
        Next

        Return True
    End Function

    ' create a salted password given the salt value
    Private Function CreateSaltedPassword(ByVal saltValue As Byte(), ByVal unsaltedPassword As Byte()) As Byte()
        ' add the salt to the hash
        Dim rawSalted(unsaltedPassword.Length + saltValue.Length - 1) As Byte
        unsaltedPassword.CopyTo(rawSalted, 0)
        saltValue.CopyTo(rawSalted, unsaltedPassword.Length)

        'Create the salted hash			
        Dim sha1 As sha1 = sha1.Create()
        Dim saltedPassword As Byte() = sha1.ComputeHash(rawSalted)

        ' add the salt value to the salted hash
        Dim dbPassword(saltedPassword.Length + saltValue.Length - 1) As Byte
        saltedPassword.CopyTo(dbPassword, 0)
        saltValue.CopyTo(dbPassword, saltedPassword.Length)

        Return dbPassword
    End Function
*/
