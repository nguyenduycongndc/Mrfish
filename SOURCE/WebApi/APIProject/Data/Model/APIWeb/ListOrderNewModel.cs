using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListOrderNewModel
    {
        public int ID { set; get; }
        public string Code { set; get; }
        public int CustomerID { set; get; }
        public string CustomerName { set; get; }
        public string CustomerPhone { set; get; }
        public long TotalPrice { set; get; }
        public long DiscountCoupon { set; get; }
        public long DiscountRank { set; get; }
        public long TotalPayment { get; set; }
        public int? CouponID { set; get; }
        public string BuyerName { set; get; }
        public string BuyerPhone { set; get; }
        public string BuyerAddress { set; get; }
        public string Note { set; get; }
        public int ProvinceID { set; get; }
        public string ProvinceName { set; get; }
        public int DistrictID { set; get; }
        public string DistrictName { set; get; }
        public int WardID { set; get; }
        public string WardName { set; get; }
        public int Status { set; get; }
        public int PaymentType { set; get; }
        public int IsPayment { set; get; }
        public int IsActive { set; get; }
       
        public DateTime CreateDate { set; get; }
        public DateTime? ProcessDate { set; get; }
        public DateTime? CompleteDate { set; get; }
        public DateTime? CancelDate { set; get; }
        public DateTime? RefundDate { set; get; }
       
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate!= null ? CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }

        public List<ListProductTableModel> ListItem { get; set; }
    }
}
