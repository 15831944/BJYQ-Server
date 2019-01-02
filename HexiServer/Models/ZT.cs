using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class ZT
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ZT()
        {

        }

        /// <summary>
        /// 有两个参数的构造函数
        /// </summary>
        /// <param name="ztCode"></param>
        /// <param name="ztName"></param>
        public ZT(string ztCode,string ztName)
        {
            ZTCode = ztCode;
            ZTName = ztName;
        }


        public string ZTCode { get; set; }
        public string ZTName { get; set; }
    }
}