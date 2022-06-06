using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class HomeScreenOutPutModel
    {
        public List<BannerModel> ListBanner { get; set; }
        public double? Point { get; set; }
        public int? CouponCount { get; set; }
        public List<ItemHomeModel> ListFlashSale { get; set; }
        public List<CategoryHomeModel> ListCategory { get; set; }
        public List<ItemHomeModel> ListHotItems { get; set; }
    }
    public class BannerModel
    {
        public int ID { get; set; }
        public string ImageUrl { get; set; }
    }
    public class ItemHomeModel
    {
        public int ID { get; set; }
        public string ImageUrl { get; set; }
        public long Price { get; set; }
        public long? PriceSale { get; set; }
        public long? SalePercent { get; set; }
        public string Name { get; set; }
        public int IsHot { get; set; }
    }
    public class CategoryHomeModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }   
    }
    public class CustomerLogin
    {
        public string CustomerName { get; set; }
        public int Point { get; set; }
    }
    
}
