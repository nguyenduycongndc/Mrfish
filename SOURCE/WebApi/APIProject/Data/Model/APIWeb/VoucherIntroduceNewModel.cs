using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class VoucherIntroduceNewModel
    {
        public int ID { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public long Discount { set; get; }
        public int TypeDiscount { set; get; }
        public int Quantity { get; set; }
        public int Remain { get; set; }
        public int QuantityReceived { get; set; }
        public string Note { set; get; }
        public int Status { set; get; }
        public string ImageUrl { set; get; }
        public int Type { set; get; }
        public DateTime? FromDate { set; get; }
        public DateTime? ToDate { set; get; }
        public string DetailFromDate { set; get; }
        public string DetailToDate { set; get; }
        public int IsActive { set; get; }
        public DateTime CreateDate { set; get; }
        public int CouponID { set; get; }

        public int CustomerID { get; set; }
        public int CusRefID { get; set; }
        public DateTime CreateDateRefer { get; set; }
      
        public int AmountUsed{ get; set; }
        public int IsStatus{ get; set; }
    }
}