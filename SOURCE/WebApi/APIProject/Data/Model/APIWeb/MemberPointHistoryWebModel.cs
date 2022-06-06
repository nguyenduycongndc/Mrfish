using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class MemberPointHistoryWebModel
    {
        public int ID { set; get; }
        public int? CustomerID { set; get; }
        public double? Point { set; get; }
        public int? OrderID { get; set; }
        public string OrderCode { set; get; }
        public string Comment { set; get; }
        public string Title { set; get; }
        public DateTime? Date { set; get; }

        public string CreateDateStr
        {
            set { }
            get
            {
                return Date.HasValue ? Date.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
        public int IsActive { get; set; }
        public double? Balance { get; set; }

    }
}