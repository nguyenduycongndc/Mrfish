using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListOrderHistoryTableModel
    {
        public int ID { set; get; }
        public int OrderID { set; get; }
        public string Title { set; get; }
        public int Status { set; get; }
        public int? CreateBy { set; get; }
        public string CreateByName { set; get; }
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
