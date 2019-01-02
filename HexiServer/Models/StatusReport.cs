using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class StatusReport1
    {
        public string status { get; set; }//状态
        public string result { get; set; }//详情
        public object data { get; set; }//返回数据
        public string parameters { get; set; }//参数信息
    }
}