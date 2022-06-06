using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListReceiveAddressModel
    {
        public int ID { set; get; }
        public int CustomerID { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int WardID { get; set; }
        public string WardName { get; set; }
        public string Address { set; get; }
        public DateTime? CreateDate { set; get; }
       
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
        public int IsDefault { set; get; }
        public int IsActive { get; set; }

    }
}
