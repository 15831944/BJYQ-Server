using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiPicture.Models
{
    public class Picture
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// openid
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string nackName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public string uploadTime { get; set; }
        /// <summary>
        /// 得票数
        /// </summary>
        public int? vote { get; set; }
        /// <summary>
        /// 期数ID
        /// </summary>
        public int? periodId { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string picPath { get; set; }
        /// <summary>
        /// 照片名
        /// </summary>
        public string picName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 点击率
        /// </summary>
        public int? rate { get; set; }
    }
}