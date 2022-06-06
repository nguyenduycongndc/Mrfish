using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class DataPageListOutputModel
    {
        public int page { get; set; }
        public int limit { get; set; }
        public int totalCountItem { get; set; }
        public object data { get; set; }
    }
}
