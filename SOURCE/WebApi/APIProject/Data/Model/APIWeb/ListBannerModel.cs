using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;
namespace Data.Model.APIWeb
{
    public class ListBannerModel
    {
        public int ID { set; get; }
        public int IsActive { set; get; }
        public string Title { set; get; }
        public int Type { set; get; }
        public int? icheck { set; get; }
        public int TypeSend { set; get; }
        public int Status { set; get; }
        public string Content { get; set; }
        public string AvatarUrl { get; set; }

        public DateTime? CreateDate { set; get; }
        public string GetStringCreateDate
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string GetNameStatus
        {
            get
            {
                return Status.Equals(SystemParam.ACTIVE_FALSE) ? "Ngừng hoạt động" : "Đang hoạt động";
            }
        }

    }
}
