using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class Delivery
    {
        /// <summary>
        /// 分类
        /// </summary>
        public string Classify { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 物品描述
        /// </summary>
        public string GoodsInfo { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string AddresseePhone { get; set; }
        /// <summary>
        /// 送件人
        /// </summary>
        public string Deliver { get; set; }
        /// <summary>
        /// 送件时间
        /// </summary>
        public string DeliveryTime { get; set; }
        /// <summary>
        /// 领取人
        /// </summary>
        public string Receiver { get; set; }
        /// <summary>
        /// 领取人电话
        /// </summary>
        public string ReceiverPhone { get; set; }
        /// <summary>
        /// 领取时间
        /// </summary>
        public string ReceiveTime { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        public string Registrant { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public string RegisterTime { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

    }
}