using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HexiServer.Models
{

    public class LookOver
    {
        public string id { get; set; }
        public string business { get; set; }
        public string item { get; set; }
        public string objectName { get; set; }
        public string period { get; set; }
        public bool? isLook { get; set; }
    }



    //public class LookOver
    //{
    //    public string business { get; set; }
    //    public LookOverItem[] items { get; set; }
    //    public LookOverObject[] objects { get; set; }

    //}

    //public class LookOverItem
    //{
    //    public string number { get; set; }
    //    public string content { get; set; }
    //    public string period { get; set; }
    //}

    //public class LookOverObject
    //{
    //    public string number { get; set; }
    //    public string name { get; set; }
    //    public bool isLook { get; set; }
    //}


    //public class UnnormalItem
    //{
    //    public string num { get; set; }
    //    public string explain { get; set; }
    //    public string[] images { get; set; }
    //}

}