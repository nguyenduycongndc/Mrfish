using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class WardModel
    {
        public int WardID { get; set; }
        public string WardName { get; set; }
        public string Type { get; set; }
        public int DistrictID { get; set; }
    }
}
