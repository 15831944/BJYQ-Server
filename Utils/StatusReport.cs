﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexiUtils
{
    public class StatusReport
    {
        public string status { get; set; }//状态
        public string result { get; set; }//详情
        public object data { get; set; }//返回数据
        public object parameters { get; set; }//参数信息

        public StatusReport()
        {

        }

        public StatusReport(object data)
        {
            this.status = "Success";
            this.result = "成功";
            this.data = data;
        }



        public StatusReport(string status, string result, object data)
        {
            this.status = status;
            this.result = result;
            this.data = data;
        }

        public StatusReport SetSuccess(object data)
        {
            this.status = "Success";
            this.result = "成功";
            this.data = data;
            return this;
        }

        public StatusReport SetFail()
        {
            this.status = "Fail";
            this.result = "未查询到任何数据";
            return this;
        }

        public StatusReport SetFail(string result)
        {
            this.status = "Fail";
            this.result = result;
            return this;
        }
    }
}
