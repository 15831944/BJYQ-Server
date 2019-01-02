using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiUserServer.Models
{

    public class RoomInfo
    {
        public int? RoomId { get; set; }
        public string RoomNumber { get; set; }
    }
    public class Proprietor
    {
        public int? Id { get; set; }//ID
        public RoomInfo[] Room { get; set; }
        //public int? RoomId { get; set; }//房产单元ID
        //public string[] RoomNumber { get; set; }//资源表编号
        //public string RoomAddress { get; set; }//房产地址
        public string Name { get; set; }//占用者名称
        public string Phone { get; set; }//联系电话
        public string Address { get; set; }//联系地址
        //public string IDNumber { get; set; }//证件号码
        public string ZTCode { get; set; }//帐套代码
        public string ZTName { get; set; }//帐套名称 
        public string Server { get; set; }//客服专员

        public string IsProprietor { get; set; }//是否占用者本人





        //public string EmergencyContact { get; set; }//紧急联系人
        //public string EmergencyPhone { get; set; }//紧急联系人电话
        //public string EmergencyAddress { get; set; }//紧急联系人地址
        //public double? Area { get; set; }//面积
        //public string Identity { get; set; }//占用者性质
        //public double? TotalArrearage { get; set; }//欠费总计
        //public string LicensePlateNumber { get; set; }//车牌号
    }

    public class FamilyMember
    {
        public int? id { get; set; }
        public int? pid { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string birth { get; set; }
        public string idType { get; set; }
        public string idNumber { get; set; }
        public string nation { get; set; }
        public string relation { get; set; }
        public string company { get; set; }
        public string phone { get; set; }
    }
}