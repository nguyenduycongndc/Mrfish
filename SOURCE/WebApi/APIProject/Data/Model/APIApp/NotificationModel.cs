using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class NotificationModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public int IsRead { get; set; }
        public DateTime CreateDate { get; set; }
        public int? OrderID { get; set; }
        public int? NewsID { get; set; }
        public int? CouponID { get; set; }
        public int? CustomerID { get; set; }
    }
}
