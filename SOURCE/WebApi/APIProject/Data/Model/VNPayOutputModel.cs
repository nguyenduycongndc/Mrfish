using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class VNPayOutputModel
    {
        public string Message { get; set; }
        public string RspCode { get; set; }
        public VNPayOutputModel GetPayOutputModel(string Message, string ResponseCode)
        {
            VNPayOutputModel vnp = new VNPayOutputModel();
            vnp.Message = Message;
            vnp.RspCode = ResponseCode;
            return vnp;
        }

    }
    public class CreateOrderVNPayOutputModel
    {
        public int ID { get; set; }
        public string Url { get; set; }
    }
}
