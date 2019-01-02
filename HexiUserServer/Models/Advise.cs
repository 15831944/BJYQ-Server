using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class Advise
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string classify { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// 表扬建议
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public string SubmitDate { get; set; }
        /// <summary>
        /// 照片1，2，3
        /// </summary>
        public string[] Image { get; set; }
    }
}