using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace HexiUtils
{
    public class SQLHelper
    {

        //private static string wytConnectionString = "Data Source=192.168.13.1;Initial Catalog=f2wyt;Integrated Security=false;User ID=sa;Password=!1asdfgh";//松园服务器连接字符串
        //private static string wytConnectionString = "Data Source=192.168.0.102;Initial Catalog=wyt;Integrated Security=false;User ID=sa;Password=101128";//松园本地连接字符串
        //private static string wxConnectionString = "Data Source=192.168.0.102;Initial Catalog=wyt;Integrated Security=false;User ID=sa;Password=101128";//松园本地连接字符串
        private static string wytConnectionString = "Data Source=192.168.1.104;Initial Catalog=wytnetsz;Integrated Security=false;User ID=sa;Password=aBCD1234";//苏州本地连接字符串
        private static string wxConnectionString = "Data Source=192.168.1.104;Initial Catalog=weixin;Integrated Security=false;User ID=sa;Password=aBCD1234";//苏州本地连接字符串
        //private static string wytConnectionString = "Data Source=192.168.1.199;Initial Catalog=wytnet;Integrated Security=false;User ID=sa;Password=Yq123";//苏州服务器连接字符串
        //private static string wxConnectionString = "Data Source=192.168.1.199;Initial Catalog=weixin;Integrated Security=false;User ID=sa;Password=Yq123";//苏州服务器连接字符串
        /**
         * 该静态方法，用于根据传入的sql语句和相关参数，在数据库中查询
         * 数据，并以表的数据表（DataTable）的形式返回查询到的数据。
         * */
        public static DataTable ExecuteQuery(string database, string sqlString, params SqlParameter[] parameters)
        {
            string connectionString = "";
            if (database == "wyt")
            {
                connectionString = wytConnectionString;
            }
            else
            {
                connectionString = wxConnectionString;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    cmd.Parameters.AddRange(parameters);
                    DataTable dt = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        /**
         * 该静态方法，用于根据传入的sql语句和相关参数，在数据库中查询
         * 数据，并以表的数据表（DataTable）的形式返回查询到的数据。
         * */
        public static int ExecuteScalar(string database, string sqlString, params SqlParameter[] parameters)
        {
            string connectionString = "";
            if (database == "wyt")
            {
                connectionString = wytConnectionString;
            }
            else
            {
                connectionString = wxConnectionString;
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlString;
                    cmd.Parameters.AddRange(parameters);
                    int id = Convert.ToInt32(cmd.ExecuteScalar());
                    return id;
                }
            }
        }

        /**
         * 该静态方法，用于根据传入的sql语句和相关参数，在数据库中插入数据，
         * 并将插入的结果返回
         * */
        public static StatusReport Insert(string database, string sqlString, params SqlParameter[] parameters)
        {
            string connectionString = "";
            if (database == "wyt")
            {
                connectionString = wytConnectionString;
            }
            else
            {
                connectionString = wxConnectionString;
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "操作成功";
            sr.parameters = sqlString;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlString;
                        cmd.Parameters.AddRange(parameters);
                        //int? id = Convert.ToInt32(cmd.ExecuteScalar());//执行插入操作，并返回受影响行数
                        object id = null;
                        id = cmd.ExecuteScalar();
                        if (id == DBNull.Value)
                        {
                            sr.status = "Fail";
                            sr.result = "数据已存在";
                            //sr.parameters = sqlString;
                        }
                        else
                        {
                            sr.data = id.ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
                //sr.parameters = sqlString;
                //throw exp;
            }
            finally
            {

            }
            return sr;
        }



        public static StatusReport Update(string database, string sqlString, params SqlParameter[] parameters)
        {
            string connectionString = "";
            if (database == "wyt")
            {
                connectionString = wytConnectionString;
            }
            else
            {
                connectionString = wxConnectionString;
            }
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "操作成功";
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sqlString;
                        //cmd.CommandType
                        cmd.Parameters.AddRange(parameters);
                        int lines = cmd.ExecuteNonQuery();//执行插入操作，并返回受影响行数
                        if (lines < 1)
                        {
                            sr.status = "Fail";
                            sr.result = "数据写入失败";
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
            }
            finally
            {

            }
            return sr;
        }



        private void DataSetTest ()
        {
            DataSet ds = new DataSet();
            ds.DataSetName = "aTable";
            DataTableCollection dtCollection = ds.Tables;
        }
    }
}