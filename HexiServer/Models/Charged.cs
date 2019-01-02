using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{
    public class Charged
    {
        public string RoomNumber { get; set; }
        public string Name { get; set; }
        public double? Total { get; set; }
        public string ZTCode { get; set; }
    }


    public class ChargedDetail
    {
        public double? AmountReceivable { get; set; }
        public string AmountMonth { get; set; }
        public string ChargeWay { get; set; }
        public string ChargeName { get; set; }
        public string ChargeDate { get; set; }
        public string StartMonth { get; set; }
        public string EndMonth { get; set; }
        public string Cashier { get; set; }
    }

    public class ChargedResult
    {
        public string AmountMonth { get; set; }
        public ChargedDetail[] Detail { get; set; }
    }
}