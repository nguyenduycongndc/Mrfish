using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListReferralCodeNewModel
    {
        public int ID { set; get; }
        public int CouponID { set; get; }
        public int CusRefID { set; get; }
        public string CusRefName { set; get; }
        public string CusRefPhone { set; get; }
        public string CouponCode { set; get; }
        public string CouponName { set; get; }
        public int CouponDiscount { set; get; }
        public int CouponTypeDiscount { set; get; }
        public int CouponQuantity { set; get; }
        public int CouponRemain { set; get; }
        public string CouponNote { set; get; }
        public int CouponStatus { set; get; }
        public string CouponImageUrl { set; get; } 
        public int CouponType { set; get; } 
        public DateTime? CouponFromDate { set; get; } 
        public DateTime? CouponToDate { set; get; } 
        public int CouponIsActive { set; get; } 
        public DateTime CouponCreateDate { set; get; } 
        public int CustomerID { set; get; }
        public string CustomerName { set; get; }
        public string CustomerPhone { set; get; }
        public int CustomerIsActive { set; get; }
       
        public DateTime CreateDate { set; get; } 
       
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate != null ? CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
       

    }
}
