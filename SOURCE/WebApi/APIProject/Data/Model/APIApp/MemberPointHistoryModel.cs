using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class MemberPointHistoryModel
    {
        public int ID { get; set; }
        public int? CustomerID { get; set; }
        public int? OrderID { get;set; }
        public string OrderCode { get; set; }
        public double Point { get; set; }
        public double Balance { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
