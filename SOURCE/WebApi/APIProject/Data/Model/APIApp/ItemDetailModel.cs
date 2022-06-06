using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ItemDetailModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public long Price { get; set; }
        public long PriceSale { get; set; }
        public int IsHot { get; set; }
        public int SalePercent { get; set; }
        public int QuantitySale { get; set; }
        public List<string> ImageUrl { get; set; }
        // mô tả sản phẩm
        public string Description { get; set; }

    }
}
