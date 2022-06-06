using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListOrderModel
    {
        public int orderID { get; set; }
        public long totalPrice { get; set; }
        public string code { get; set; }
        public string Image { get; set; }
        public string ListImage { get; set; }
        public List<string> ArrImage { get; set; }
        public string name { get; set; }
        public int status { get; set; }
        public int qty { get; set; }
        public int count { get; set; }
        public int CountItem { get; set; }
        public string Date
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
            set { }
        }
        public DateTime? CancelDate { get; set; }
        public string CancelDateStr {
            get
            {
                if(CancelDate != null)
                    return CancelDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                return null;
            }
            set{ }
        }
        public DateTime? ConfirmDate { get; set; }
        public string ConfirmDateStr
        {
            get
            {
                if(ConfirmDate != null)
                    return ConfirmDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                return null;
            }
            set { }
        }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateStr
        {
            get
            {
                if(DeliveryDate != null)
                    return DeliveryDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                return null;
            }
            set { }
        }
        public DateTime? PaymentDate { get; set; }
        public string PaymentDateStr
        {
            get
            {
                if(PaymentDate!= null)
                    return PaymentDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                return null;
            }
            set { }
        }
    }
}
