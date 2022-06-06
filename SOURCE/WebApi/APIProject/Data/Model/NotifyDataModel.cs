using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class NotifyDataModel
    {
        public int id { get; set; }
        public int type { get; set; }
        public string content { get; set; }
    }
    // thêm 3/1/2021
    public class NotiDataPushNotiModel
    {
        public string title { get; set; }
        public string description { get; set; }
        public int type { get; set; }
        public int userSend { get; set; }
    }
}
