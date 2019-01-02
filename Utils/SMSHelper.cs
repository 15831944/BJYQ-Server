using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexiUtils
{
    public class SMSHelper
    {
        public static StatusReport SendMessage(string phoneNumber, string code)
        {
            string urlString = "https://dx.ipyy.net/smsJson.aspx";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("action", "send");
            nvc.Add("account", "cs115");
            nvc.Add("password", "cs115456");
            nvc.Add("mobile", phoneNumber);
            nvc.Add("content", "您的验证码是：" + code + ",请不要将验证码透漏给他人【亿方物业】");
            StatusReport sr = null;
            sr = RequestHelper.PostRequestTest(urlString, nvc);
            return sr;
        }

        private static string getCode()
        {
            return "123456";
        }
    }
}



/*
 * https://dx.ipyy.net/smsJson.aspx
 * 
 */
