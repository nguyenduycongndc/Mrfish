using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class RankNewModel
    {
        public int ID { set; get; }
        public string RankName { set; get; }
        public double DiscountPercent { set; get; }
        public string Descriptions { set; get; }
        public DateTime CreateDate { set; get; }
        public int IsActive { set; get; }
        public double MinPoint { set; get; }
        public double MaxPoint { set; get; }
        public int Level { set; get; }
    }
}
