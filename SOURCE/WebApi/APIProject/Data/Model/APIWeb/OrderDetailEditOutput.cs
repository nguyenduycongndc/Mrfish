using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class OrderDetailEditOutput
    {
        public class OrderItemEdit
        {
            public int ItemID { get; set; }
            public string ItemName { get; set; }
            public int ItemQTY { get; set; }
            public long ItemPrice { get; set; }
            public long ItemTotalPrice { get; set; }
            public string ItemCode { get; set; }

        }

        public List<OrderItemEdit> ListItem { get; set; }
        public int OrderID { get; set; }
        public long TotalPrice { get; set; }
        public long Discount { get; set; }
        public string CusName { get; set; }
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }
        public string AgentCode { get; set; }
        public int Status { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public string Code { get; set; }
        public double? addPoint { get; set; }
        public int? PointNow { get; set; }
        public int QTY { set; get; }
        public string Note { set; get; }
    }
}
