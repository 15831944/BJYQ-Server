using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{
    public class House
    {
        public string department { get; set; }//部门
        public string roomNum { get; set; }//房号
        public string floor { get; set; }//层数
        public string building { get; set; }//所属楼宇
        public string unit { get; set; }//所属单元 
        public string property { get; set; }//性质
        public string type { get; set; }//房产类型
        public string houseType { get; set; }//户型
        public decimal? area { get; set; }//建筑面积
    }
} 