using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CommentOutputModel
    {
        public int ID { get; set; }
        public int? PostID { get; set; }
        public int? CommentParentID { get; set; }
        public int CountLike { get; set; }
        public int CountComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Date
        {
            get
            {
                return CreatedDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public int Role { get; set; }
        public int IsIncognito { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int checklike { get; set; }
        public string Content { get; set; }
        public List<CommentInputModel> ListReplyComment { get; set; }
    }
}
