//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class News
    {
        public int ID { get; set; }
        public int CreateUserID { get; set; }
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string UrlImage { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int TypeSend { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int IsActive { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public int CountLike { get; set; }
        public int CountComment { get; set; }
        public Nullable<System.DateTime> ModifinedDate { get; set; }
        public Nullable<System.DateTime> TimePush { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> CountView { get; set; }
        public Nullable<int> Icheck { get; set; }
    
        public virtual CategoryNew CategoryNew { get; set; }
        public virtual Item Item { get; set; }
        public virtual User User { get; set; }
    }
}