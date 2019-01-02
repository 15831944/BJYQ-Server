using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexiWebService.Models
{
    class Occupier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string IdNumber { get; set; }
        public string Address { get; set; }
        public string account { get; set; }//账套代码
        public string roomNumber { get; set; }//单元编号
    }
}
