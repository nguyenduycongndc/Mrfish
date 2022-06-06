using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class OrderDetailOutputModel : OrderOutputModel
    {
        public long BasePrice { get { return listOrderItem.Select(u => u.SumPrice).Sum(); } set { } }
        public List<OrderDetailModel> listOrderItem { get; set; }
    }
    public class OrderDetailModel
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string ImageUrl { get; set; }
        public long SumPrice { get; set; }
        public int Quantity { get; set; }
    }
    public class CancleOrderInputModel
    {
        public int ID { get; set; }
    }
    public class OrderModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
        public int CustomerID { get; set; }
        public string Province { get; set; }
        public string District{ get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public int PaymentType { get; set; }
        public int IsPayment { get; set; }
        public string Note { get; set; } 
        public long SumPrice { get; set; }
        public long DiscountCoupon { get; set; }
        public long DiscountRank{ get; set; }
        public long TotalPrice{ get; set; } 
        public long TotalQuantity{ get; set; } 
        public DateTime CreateDate { get; set; }
        public string UrlVnpay { get; set; }
        public List<OrderDetailModel> ListOrderItem { get; set; }
    }

    public class ListCartOutputModel
    {
        public long TotalPrice { get; set; }
        public List<OrderDetailModel> listCart { get; set; }
        public int SumItem { get;set; }
    }


    public class OrderOutputModel
    {
        public int OrderID { get; set; }
        public string Code { get; set; }
        public long TotalPrice { get; set; }
        public int Status { get; set; }
        public int Qty { get; set; }
        public int UserID { get; set; }
        public string BuyerName { get; set; }
        public string BuyerPhone { get; set; }
        public long Discount { get; set; }
        public int Type { get; set; }
        public int? ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int? WardID { get; set; }
        public string WardName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
        public string Hour
        {
            get
            {
                return CreateDate.ToString("HH:mm");
            }
            set { }
        }
        public string Date
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public string CreateDateStr
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public DateTime? CancelDate { get; set; }
        public string CancelDateStr
        {
            get
            {
                return CancelDate.HasValue ? CancelDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : null;
            }
            set { }
        }
        public DateTime? ConfirmDate { get; set; }
        public string ConfirmDateStr
        {
            get
            {
                return ConfirmDate.HasValue ? ConfirmDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : null;
            }
            set { }
        }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateStr
        {
            get
            {
                return PaymentDate.HasValue ? PaymentDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : null;
            }
            set { }
        }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateStr
        {
            get
            {
                return PaymentDate.HasValue ? PaymentDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : null;
            }
            set { }
        }

    }
    public class CalendarImageOutPut
    {
        public int ID { get; set; }
        public string Image { get; set; }
    }



}
