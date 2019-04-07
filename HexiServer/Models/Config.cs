using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Config
    {
        /// <summary>
        /// 物业通图片存储主路径
        /// </summary>
        public const string imageMainPath = "D:\\Servers\\bjyqServer\\wgxt\\WYTWS\\Files\\";//物业通图片存储主路径


        /// <summary>
        /// 报修图片
        /// </summary>
        public const string repairImageMainPath = imageMainPath + "jczl_fwrwgl\\";//报修图片
        /// <summary>
        /// 巡更图片
        /// </summary>
        public const string patrolImageMainPath = imageMainPath + "jczl_fwrwgl\\";//巡更图片
    }
}