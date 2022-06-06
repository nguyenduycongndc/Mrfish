using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CartModel
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
        public long PriceSale { get; set; }
        public int QuantitySale { get; set; }
        public int Quantity { get; set; }
    }
    public class AddCartModel
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartModel
    {
        public int CartID { get; set; }
        public int Quantity { get;set; }
    }
    public class DeleteCartModel
    {
        public List<int> CartID { get; set; }
    }
}
