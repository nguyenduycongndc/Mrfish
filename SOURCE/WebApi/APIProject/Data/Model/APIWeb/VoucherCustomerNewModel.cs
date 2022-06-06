using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class VoucherCustomerNewModel
    {
        public int ID { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public long Discount { set; get; }
        public int TypeDiscount { set; get; }
        public int Quantity { get; set; }
        public int Remain { get; set; }
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

        public int RankID { get; set; }
        public int CustomerID { get; set; }
        public string RankName { get; set; }
        public string[] ListRankID { get; set; }
        public string ListRank { get; set; }
        public int RankTV { get; set; }
        public int RankB { get; set; }
        public int RankV { get; set; }
        public int RankKC { get; set; }
        public int AmountUsed { get; set; }
        public double DiscountPercent { get; set; }
    }
}