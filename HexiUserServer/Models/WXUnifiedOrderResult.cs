using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class WXUnifiedOrderResult
    {
        /// <summary>
        /// SUCCESS/FAIL 此字段是通信标识，非交易标识，交易是否成功需要查看result_code来判断
        /// </summary>
        public string return_code { get; set; }
        /// <summary>
        /// 返回信息，如非空，为错误原因 签名失败 参数格式校验错误
        /// </summary>
        public string return_msg { get; set; }
        /// <summary>
        /// 调用接口提交的小程序ID
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// 调用接口提交的商户号
        /// </summary>
        public string mch_id { get; set; }
        /// <summary>
        /// 调用接口提交的终端设备号
        /// </summary>
        public string device_info { get; set; }
        /// <summary>
        /// 微信返回的随机字符串
        /// </summary>
        public string nonce_str { get; set; }
        /// <summary>
        /// 微信返回的签名
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 业务结果SUCCESS/FAIL
        /// </summary>
        public string result_code { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }
        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
        /// <summary>
        /// 交易类型 调用接口提交的交易类型，取值如下：JSAPI
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 预支付交易会话标识  微信生成的预支付回话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        public string prepay_id { get; set; }
    }
}



/**
名称 描述 原因 解决方案 
NOAUTH 商户无此接口权限 商户未开通此接口权限 请商户前往申请此接口权限 
NOTENOUGH 余额不足 用户帐号余额不足 用户帐号余额不足，请用户充值或更换支付卡后再支付 
ORDERPAID 商户订单已支付 商户订单已支付，无需重复操作 商户订单已支付，无需更多操作 
ORDERCLOSED 订单已关闭 当前订单已关闭，无法支付 当前订单已关闭，请重新下单 
SYSTEMERROR 系统错误 系统超时 系统异常，请用相同参数重新调用 
APPID_NOT_EXIST APPID不存在 参数中缺少APPID 请检查APPID是否正确 
MCHID_NOT_EXIST MCHID不存在 参数中缺少MCHID 请检查MCHID是否正确 
APPID_MCHID_NOT_MATCH appid和mch_id不匹配 appid和mch_id不匹配 请确认appid和mch_id是否匹配 
LACK_PARAMS 缺少参数 缺少必要的请求参数 请检查参数是否齐全 
OUT_TRADE_NO_USED 商户订单号重复 同一笔交易不能多次提交 请核实商户订单号是否重复提交 
SIGNERROR 签名错误 参数签名结果不正确 请检查签名参数和方法是否都符合签名算法要求 
XML_FORMAT_ERROR XML格式错误 XML格式错误 请检查XML参数格式是否正确 
REQUIRE_POST_METHOD 请使用post方法 未使用post传递参数  请检查请求参数是否通过post方法提交 
POST_DATA_EMPTY post数据为空 post数据不能为空 请检查post数据是否为空 
NOT_UTF8 编码格式错误 未使用指定编码格式 请使用UTF-8编码格式 
 */
