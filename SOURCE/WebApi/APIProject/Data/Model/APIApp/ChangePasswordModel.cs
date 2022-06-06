using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ChangePasswordForgotModel
    {
        public int CustomerId { get; set; }
        public string newPass { get; set; }
    }
    public class ChangePasswordModel
    {
        public string password { get; set; }
        public string newPassword { get; set; }
    }
}
