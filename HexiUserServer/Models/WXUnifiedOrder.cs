using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class WXUnifiedOrder
    {
        /// <summary>
        /// 微信分配的小程序ID 
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 商品简单描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 单品优惠字段
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        /// 随机字符串，不长于32位
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        /// 接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数
        /// </summary>
        public string notify_url { get; set; }
        /// <summary>
        /// 用户在商户appid下的唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 商户系统内部的订单号,32个字符内、可包含字母
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP
        /// </summary>
        public string spbill_create_ip { get; set; }
        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 小程序取值如下：JSAPI
        /// </summary>
        public string trade_type { get; set; }
    }
}