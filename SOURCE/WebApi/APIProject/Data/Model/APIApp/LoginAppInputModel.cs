using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class LoginAppInputModel
    {
        public string Value { get; set; }
        public string Password { get; set; }
        //public int Type { get; set; }
        public string DeviceID { get; set; }
    }
}