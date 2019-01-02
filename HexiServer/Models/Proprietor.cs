using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Proprietor
    {
        public int? Id { get; set; }//ID
        public string RoomNumber { get; set; }//资源表编号
        public string RoomAddress { get; set; }//房产地址
        public string Name { get; set; }//占用者名称
        public string Phone { get; set; }//联系电话
        public string Address { get; set; }//联系地址
        public string IDNumber { get; set; }//证件号码
        public string ZTCode { get; set; }//帐套代码
        public string EmergencyContact { get; set; }//紧急联系人
        public string EmergencyPhone { get; set; }//紧急联系人电话
        public string EmergencyAddress { get; set; }//紧急联系人地址
        public double? Area { get; set; }//面积
        public string Identity { get; set; }//占用者性质
        public double? TotalArrearage { get; set; }//欠费总计
        public string LicensePlateNumber { get; set; }//车牌号码
        public string CarColor { get; set; }//车辆颜色
        public string CarBrand { get; set; }//车辆品牌
        public string CarType { get; set; }//车辆型号
    }
}