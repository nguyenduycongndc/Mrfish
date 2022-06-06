using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListOrderWebModel
    {
        public int ID { get; set; }
        public string CusName { get; set; }
        public string CusPhone { get; set; }
        public long TotalPrice { get; set; }
        public long DiscountRank { get; set; }
        public long DiscountCoupon { get; set; }
        public long TotalPayment { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerAddress { get; set; }
        public string Code { get; set; }
        public int QTY { set; get; }
        public int PaymentType { set; get; }
        public int Status { set; get; }
        public int ProvinceID { set; get; }
        public string ProvinceName { set; get; }
        public int DistrictID { set; get; }
        public string DistrictName { set; get; }
        public int WardID { set; get; }
        public string WardName { set; get; }
        public DateTime CreateDate { get; set; }
    }
}
