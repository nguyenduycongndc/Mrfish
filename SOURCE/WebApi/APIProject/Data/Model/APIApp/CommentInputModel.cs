using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CommentInputModel
    {
        public int ID { get; set; }
        public int? PostID { get; set; }
        public int? CommentParentID { get; set; }
        public int CountLike { get; set; }
        public int CountComment { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Role { get; set; }
        public int IsIncognito { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int checklike { get; set; }
        public string Content { get; set; }
    }
}
