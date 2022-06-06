using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CreateOrderModel
    {
        public int ReceiveAddressID { get; set; }
        public int PaymentType { get; set; }
        public string Note { get; set; }
        public List<int> CartID { get; set; }
        public int? CouponID { get; set; }
    }
}
