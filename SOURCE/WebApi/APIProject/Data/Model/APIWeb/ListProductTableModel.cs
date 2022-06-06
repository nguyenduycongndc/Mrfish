using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListProductTableModel
    {
        public int ID { set; get; }
        public string ItemCode { set; get; }
        public string ItemName { set; get; }
        public string GroupItemName { set; get; }
        public int GroupID { set; get; }
        public int OrderItemQTY { set; get; }
        public int ItemID { set; get; }
        public int OrderItemID { set; get; }
        public long SumPrice { set; get; }
        
        public DateTime CreateDate { set; get; }
       
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate!= null ? CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
       

    }
}
