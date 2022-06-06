using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;
namespace Data.Model.APIWeb
{
    public class ListItemOutputModel
    {
        public int ID { set; get; }
        public string Name { set; get; }
        public string Code { set; get; }
        public string CategoryName { set; get; }
        public int CategoryID { get; set; }
        public int IsHot { set; get; }
        public int Status { set; get; }
        public long Price { set; get; }
        public long PriceSale { set; get; }
        public int SalePercent { set; get; }
        public string ImageUrl { set; get; }
        public string Description { set; get; }
        public DateTime? CreateDate { set; get; }
    }
}
