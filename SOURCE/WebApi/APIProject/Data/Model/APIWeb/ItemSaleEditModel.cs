using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ItemSaleEditModel
    {
        public int ID { set; get; }
        public int ItemID  { set; get; }
        public int CategoryID { set; get; }

        public string Price { set; get; }
        public int Quantity { set; get; }
        public int QuantitySold { set; get; }
        public int Remain { set; get; }
        public string FromTime { set; get; }
        public string FromDate { set; get; }
        public string ToTime { set; get; }
        public string ToDate { set; get; }
        public int IsActive { set; get; }
        public DateTime CreateDate { set; get; }
        public int GroupItemID { set; get; }
    }
}
