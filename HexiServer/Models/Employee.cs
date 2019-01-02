using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Employee
    {
        public int? Id { get; set; }//id
        public string UserCode { get; set; }//UserCode:员工编号
        public string UserName { get; set; }//UserName:员工姓名
        public int? EmpId { get; set; }//员工表Id
        public string Password { get; set; }//Password：密码
        public string[] ZTCodes { get; set; }//ZTCodes：帐套代码（有多个）
        public ZT[] ZTInfo { get; set; }//帐套信息
        public string[] Jurisdiction { get; set; } //权限信息
        public string[] Level { get; set; }//等级信息
    }
}