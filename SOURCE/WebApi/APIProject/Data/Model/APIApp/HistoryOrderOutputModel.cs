using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class HistoryOrderOutputModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public int OrderID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DateStr {
            get
            {
                return CreatedDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public int IsActive { get; set; }
    }
    public class HistoryOrder
    {
        public int Status { get; set; }
        public int OrderID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int IsActive { get; set; }
    }

    public class HistoryOutput
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int totalPage { get; set; }
        public IPagedList<HistoryOrderOutputModel> listHistoryModel { get; set; }
    }
}
