using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CustomerModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public string Token { get; set; }
        public System.DateTime DOB { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public int Sex { get; set; }
        public string AvatarUrl { get; set; }
        public Nullable<int> ProvinceCode { get; set; }
        public Nullable<int> DistrictCode { get; set; }
        public Nullable<int> WardID { get; set; }
        public string Address { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public int Status { get; set; }
        public System.DateTime ExpireTocken { get; set; }
        public int Type { get; set; }
        public System.DateTime CraeteDate { get; set; }
        public int IsActive { get; set; }
        public string DeviceID { get; set; }
        public string AgentCode { get; set; }
        public string Password { get; set; }
        public string PasswordReset { get; set; }
        public string Note { get; set; }
        //public int Province { get; set; }
        public int Point { get; set; }
        public int PointRanking { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> ExpPasswordReset { get; set; }
        public Nullable<int> OTP { get; set; }
    }
}
