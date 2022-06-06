using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ReceiveAddressModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int ProvinceID { get; set; }
        public int DistrictID { get; set; }
        public int WardID { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public int IsDefault { get; set; }
    }
    public class AddReceiveAddressModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int ProvinceID { get; set; }
        public int DistrictID { get; set; }
        public int WardID { get; set; }
        public string Address { get; set; }
        public int IsDefault { get; set; }
    }
    public class UpdateReceiveAddressModel : AddReceiveAddressModel
    {
        public int ID { get; set; }
    }
    public class DeleteReceiveAddressModel 
    {
        public int ID { get; set; }
    }

}
