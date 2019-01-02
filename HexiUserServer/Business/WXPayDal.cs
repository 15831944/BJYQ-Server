using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;
using HexiUserServer.Models;
using HexiUtils;

namespace HexiUserServer.Business
{
    public class WXPayDal
    {

        /// <summary>
        /// 统一下单
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="totalCharge"></param>
        /// <returns>StatusReport</returns>
        public static StatusReport UnifiedOrder(string openId,double totalCharge, string dataBag)//统一下单
        {
            StatusReport sr = new StatusReport();
            string outTradeNo = GetOutTradeNumber();
            WXUnifiedOrder order = new WXUnifiedOrder()
            {
                appid = Common.Appid,
                mch_id = Common.Mchid,
                attach = dataBag,
                body = "北京燕侨物业管理有限公司苏州分公司-物业收费",
                detail = "dfsaff",
                nonce_str = GetNonceStr(),
                notify_url = "http://k17154485y.imwork.net/wxuser/Charge/OnTest",
                openid = openId,
                out_trade_no = outTradeNo,
                spbill_create_ip = "115.159.93.120",
                total_fee = Convert.ToInt32(totalCharge * 100).ToString(),  
                trade_type = "JSAPI",
            };
            string xmlParam = GetXmlParam(order);
            //sr.parameters = xmlParam;
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            sr = RequestHelper.PostRequest(url, xmlParam);
            if (sr.status == "Fail")
            {
                return sr;
            }
            string xmlResult = (string)sr.data;
            sr = GetOrderResult(xmlResult);
            sr.parameters = order.out_trade_no;
            return sr;
        }

        private static StatusReport GetOrderInfo(string xmlResult)
        {
            StatusReport sr = new StatusReport();
            //xmlResult = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg><appid><![CDATA[wx3bfed4fd2abe0355]]></appid><mch_id><![CDATA[1492807892]]></mch_id><nonce_str><![CDATA[0mYmAa79azHOQfsT]]></nonce_str><sign><![CDATA[972EBB9552267F6A06426A2667BB7715]]></sign><result_code><![CDATA[SUCCESS]]></result_code><prepay_id>wx20171129080541ec6fa2b7c60713319480</prepay_id><trade_type><![CDATA[JSAPI]]></trade_type></xml>";
            try
            {
                byte[] array = Encoding.ASCII.GetBytes(xmlResult);
                Stream stream = new MemoryStream(array);

                XDocument xDocument = new XDocument();
                xDocument = XDocument.Load(stream);
                XElement root = xDocument.Root;
                XElement prepayid = root.Element("prepay_id");
                string prepayId = prepayid.Value;
                if (!string.IsNullOrEmpty(prepayId))
                {
                    sr.status = "Success";
                    sr.result = "成功获取prepay_id";
                    sr.parameters = xmlResult;
                    sr.data = prepayId;
                }
                else
                {
                    sr.status = "Fail";
                    sr.result = "获取失败";
                }
            }
            catch (Exception exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
            }

            return sr;
            
        }

        private static StatusReport GetOrderResult(string xmlResult)
        {
            StatusReport sr = new StatusReport();
            sr.status = "Success";
            sr.result = "成功";
            WXUnifiedOrderResult orderResult = new WXUnifiedOrderResult();
            //xmlResult = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg><appid><![CDATA[wx3bfed4fd2abe0355]]></appid><mch_id><![CDATA[1492807892]]></mch_id><nonce_str><![CDATA[0mYmAa79azHOQfsT]]></nonce_str><sign><![CDATA[972EBB9552267F6A06426A2667BB7715]]></sign><result_code><![CDATA[SUCCESS]]></result_code><prepay_id>wx20171129080541ec6fa2b7c60713319480</prepay_id><trade_type><![CDATA[JSAPI]]></trade_type></xml>";
            try
            {
                byte[] array = Encoding.ASCII.GetBytes(xmlResult);
                Stream stream = new MemoryStream(array);
                XDocument xDocument = new XDocument();
                xDocument = XDocument.Load(stream);
                XElement root = xDocument.Root;
                orderResult.return_code = root.Element("return_code").Value;
                orderResult.return_msg = root.Element("return_msg").Value;
                if (orderResult.return_code == "SUCCESS")
                {
                    orderResult.appid = root.Element("appid").Value;
                    orderResult.mch_id = root.Element("mch_id").Value;
                    //orderResult.device_info = root.Element("device_info").Value;
                    orderResult.nonce_str = root.Element("nonce_str").Value;
                    orderResult.sign = root.Element("sign").Value;
                    orderResult.result_code = root.Element("result_code").Value;
                    if (orderResult.result_code == "SUCCESS")
                    {
                        orderResult.trade_type = root.Element("trade_type").Value;
                        orderResult.prepay_id = root.Element("prepay_id").Value;
                    }
                    else
                    {
                        orderResult.err_code = root.Element("err_code").Value;
                        orderResult.err_code_des = root.Element("err_code_des").Value;
                        sr.status = orderResult.err_code;
                        sr.result = orderResult.err_code_des;
                    }
                }
                else
                {
                    sr.status = orderResult.return_code;
                    sr.result = orderResult.return_msg;
                }
            }
            catch (Exception exp)
            {
                sr.status = "Fail";
                sr.result = exp.Message;
            }

            sr.data = JsonConvert.SerializeObject(orderResult);
            return sr;

        }


        /// <summary>
        /// 获取统一下单需要的xml参数
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static string GetXmlParam(WXUnifiedOrder order)

        {
            Dictionary<string, string> dic = ToDictionary(order);
            dic = dic.OrderBy(m => m.Key).ToDictionary(m => m.Key, m => m.Value);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<xml>");
            foreach (var item in dic)
            {
                sbXml.Append("<" + item.Key + ">" + item.Value + "</" + item.Key + ">");
                sb.Append(item.Key + "=" + item.Value + "&");
            }
            sb.Append("key=" + Common.Mchkey);
            string str = sb.ToString().Replace(" ", "");
            string sign = EncryptionHelper.MD5Encryption(str).ToUpper();
            sbXml.Append("<sign>" + sign + "</sign></xml>");
            return sbXml.ToString().Replace(" ", "");

        }

        /// <summary>
        /// 根据形成订单的系统时间生成订单号
        /// </summary>
        /// <returns></returns>
        private static string GetOutTradeNumber()
        {
            string dt = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            return dt + randomNumber.ToString();
        }

        /// <summary>
        /// 将对象转换为字典
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ToDictionary(Object o)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            Type t = o.GetType();
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, (string)(mi.Invoke(o, new Object[] { })));
                }
            }
            return map;
        }

        private static string GetNonceStr()
        {
            string nonceStr = Guid.NewGuid().ToString("N");
            nonceStr = nonceStr.Substring(0, 32).ToUpper();
            return nonceStr;
        }
    }
}