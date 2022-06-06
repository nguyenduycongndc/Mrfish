using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Data.Model.APIWeb
{
    public class ListNewsWebOutputModel
    {
        public int ID { get; set; }
        public string NewsName { get; set; }
        public int CreateUserID { get; set; }
        public int Display { get; set; }
        public int UserId { set; get; }
        public int CountLike { set; get; }
        public int CountComment { set; get; }
        public string CreateUserName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Depcription { get; set; }
        public string Content { get; set; }
        public string UrlImage { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int IsActive { set; get; }
        public int TypeSend { get; set; }
        public DateTime? timePush { get; set; }
        public int? itemID { get; set; }
        public int SumView { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }

    }
}
