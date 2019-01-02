using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class MyServer
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string phoneNumber { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string picture { get; set; }
        /// <summary>
        /// 主管内容
        /// </summary>
        public string responsibility { get; set; }
    }
}