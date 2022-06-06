using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class UserInforOutputModel : CustomerDetailOutputModel
    {
        public int UserID { get; set; }
        public string Token { get; set; }
        public double Point { get; set; }
        public int CouponCount { get; set; }
        public string RankName { get; set; }
        public double DiscountRank { get; set; }
        public int RankLevel { get; set; }
        public int CheckLoginPass { get; set; }
        public string DeviceID { get; set; }
        public string Code { get; set; }
        public int OTP { get; set; }
    }
}
