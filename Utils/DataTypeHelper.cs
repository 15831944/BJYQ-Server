using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUtils
{
    public class DataTypeHelper
    {
        /// <summary>
        /// 将数据库中取出的数据转换为string类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string GetStringValue(object row)
        {
            if (row == DBNull.Value)
            {
                return "";
            }
            else
            {
                return Convert.ToString(row);
            }
        }

        /// <summary>
        /// 将数据库中取出的数据转换为DateTime类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static DateTime? GetDateValue(object row)
        {
            if (row == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDateTime(row);
            }
        }

        /// <summary>
        /// 将数据库中取出的DateTime类型数据转换为string类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string GetDateStringValue(object row)
        {
            if (row == DBNull.Value)
            {
                return "";
            }
            else
            {
                DateTime dt = Convert.ToDateTime(row);
                string dstring = dt.ToString("yyyy-MM-dd HH:mm:ss");
                //string dstring = dt.ToString("yyyy-MM-dd");
                return dstring;
            }
        }

        /// <summary>
        /// 将数据库中取出的数据转换为double?类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static double? GetDoubleValue(object row)
        {
            if (row == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDouble(row);
            }
        }


        /// <summary>
        /// 将数据库中取出的数据转换为double?类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static decimal? GetDecimalValue(object row)
        {
            if (row == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(row);
            }
        }

        /// <summary>
        /// 将数据库中取出的数据转换为int?类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static int? GetIntValue(object row)
        {
            if (row == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(row);
            }
        }

        /// <summary>
        /// 将数据库中取出的数据转换为int?类型
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static bool? GetBooleanValue(object row)
        {
            if (row == DBNull.Value)
            {
                return null;
            }
            else
            {
                return Convert.ToBoolean(row);
            }
        }

        /// <summary>
        /// 将数据转换为可以存储到数据库中的类型。如果字符串为空，则返回DBNull.Value；如果不为空，则原样返回。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Object GetDBValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
    }
}