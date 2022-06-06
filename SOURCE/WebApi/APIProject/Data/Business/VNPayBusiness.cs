using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Data.Business.VNPAY_CS_ASPX;

namespace Data.Business
{
    public class VNPayBusiness : GenericBusiness
    {
        public VNPayBusiness(MrFishEntities context = null) : base()
        {

        }
        VnPayLibrary vnpay = new VnPayLibrary();
        PushNotifyBusiness oneSignalBus = new PushNotifyBusiness();
        OrderBusiness orderBus = new OrderBusiness();
        RequestAPIBusiness requestAPI = new RequestAPIBusiness();
        //TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        public string GetUrl(int orderID)
        {
            //Get Config Info
            //Get payment input
            //Build URL for VNPAY
            Order order = cnn.Orders.Find(orderID);
            var Content = "Thanh toan don hang " + order.Code + " .So tien " + Util.ConvertCurrency(order.TotalPrice) + " VND";
            vnpay.AddRequestData("vnp_Version", "2.0.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", SystemParam.vnp_TmnCode);
            string locale = "vn";//"en"
            if (!string.IsNullOrEmpty(locale))
            {
                vnpay.AddRequestData("vnp_Locale", locale);
            }
            else
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_TxnRef", order.ID.ToString());
            vnpay.AddRequestData("vnp_OrderInfo", Content);
            vnpay.AddRequestData("vnp_OrderType", "insurance");
            vnpay.AddRequestData("vnp_Amount", (order.TotalPrice * 100).ToString());
            vnpay.AddRequestData("vnp_ReturnUrl", SystemParam.vnp_Return_url);
            vnpay.AddRequestData("vnp_IpAddr", GetIpAddress());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string paymentUrl = vnpay.CreateRequestUrl(SystemParam.vnp_Url, SystemParam.vnp_HashSecret);
            oneSignalBus.SaveLog("url", paymentUrl);
            return paymentUrl;
        }

        public static string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown"))
                    ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }

        public VnpViewModel GetVnpReturn(VnpOutputModel vnp)
        {
            string json = JsonConvert.SerializeObject(vnp);
            oneSignalBus.SaveLog("url_return", json);
            VnpViewModel vnpOutput = new VnpViewModel();
            string lang = "vi";
            string Transaction_Succes = SystemParam.TRANSACTION_SUCCESS;
            string Transaction_False = SystemParam.TRANSACTION_FAIL;
            try
            {
                Order order = cnn.Orders.Find(int.Parse(vnp.vnp_TxnRef));
                long money;
                try
                {
                    money = Int32.Parse(vnp.vnp_Amount) / 100;
                    if (money != order.TotalPrice)
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", money), order.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                        return vnpOutput;
                    }
                }
                catch (Exception ex)
                {
                    string jsonEx = JsonConvert.SerializeObject(ex);
                    oneSignalBus.SaveLog("Exepcion", jsonEx);
                    vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", order.TotalPrice), order.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                    return vnpOutput;
                }

                if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSucces)
                {
                    if (order != null)
                    {
                        if (order.Status == SystemParam.STATUS_ORDER_CANCEL)
                        {
                            vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", order.TotalPrice), order.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed + order.ID);
                        }
                        else
                        {
                            vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", order.TotalPrice), order.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_Succes, SystemParam.customer_success + order.ID);
                        }
                    }
                    else
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", 0), DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                    }
                }
                else
                {
                    if (order != null)
                    {
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", order.TotalPrice), order.CreateDate.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed + order.ID);

                    }
                    else
                        vnpOutput.getVnpModel(vnp.vnp_TxnRef, string.Format("{0:#,0}", 0), DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
                }
            }
            catch (Exception ex)
            {
                string jsonEx = JsonConvert.SerializeObject(ex);
                oneSignalBus.SaveLog("Exepcion", jsonEx);
                vnpOutput.getVnpModel(vnp.vnp_TxnRef, "", DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"), Transaction_False, SystemParam.customer_failed);
            }
            return vnpOutput;
        }

        public VNPayOutputModel GetVnpIpn(VnpOutputModel vnp)
        {

            VNPayOutputModel output = new VNPayOutputModel();
            try
            {
                string json = JsonConvert.SerializeObject(vnp);
                oneSignalBus.SaveLog("url_ipn", json);
                int appID = 0;
                appID = int.Parse(vnp.vnp_TxnRef);
                Order order = cnn.Orders.Find(int.Parse(vnp.vnp_TxnRef));
                if (order == null)
                {
                    output = output.GetPayOutputModel("Order not found", "01");
                    return output;
                }
                if (order != null)
                {
                    int money = 0;
                    try
                    {
                        money = int.Parse(vnp.vnp_Amount) / 100;
                        if (money != order.TotalPrice)
                        {
                            output = output.GetPayOutputModel("Invalid amount", "04");
                            return output;
                        }

                    }
                    catch
                    {
                        output = output.GetPayOutputModel("Invalid amount", "04");
                        return output;
                    }
                    bool checkSignature = vnpay.ValidateSignature(vnp.vnp_SecureHash, SystemParam.vnp_HashSecret, vnp);
                    if (checkSignature)
                    {
                        try
                        {
                            if (vnp.vnp_ResponseCode == SystemParam.vnp_CodeSucces)
                            {

                                if (order.Status == SystemParam.STATUS_ORDER_PENDING && order.IsPayment.Equals(SystemParam.ORDER_NOT_PAID))
                                {
                                    order.IsPayment = SystemParam.ORDER_PAID;
                                    cnn.SaveChanges();
                                    var content = "Có đơn hàng " + order.Code + " mới cần xác nhận";
                                    var noti = new Notification
                                    {
                                        Content = content,
                                        Title = content,
                                        OrderID = order.ID,
                                        IsAdmin = SystemParam.ROLE_ADMIN,
                                        IsActive = SystemParam.ACTIVE,
                                        CreateDate = DateTime.Now,
                                        Viewed = SystemParam.NOTI_NOT_VIEWED,
                                        Type = SystemParam.NOTIFICATION_TYPE_ORDER,
                                    };
                                    cnn.Notifications.Add(noti);
                                    cnn.SaveChanges();
                                    var data = SystemParam.URL_WEB_SOCKET + "?noti_id=" + noti.ID + "&content=" + content + "&order_id=" + order.ID + "&type=1";
                                    requestAPI.GetJson(data);
                                    output = output.GetPayOutputModel("Confirm Success", vnp.vnp_ResponseCode);
                                    return output;
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }

                            }
                            else
                            {
                                if (order.Status == SystemParam.STATUS_ORDER_PENDING && order.IsPayment.Equals(SystemParam.ORDER_NOT_PAID))
                                {
                                    orderBus.ChangeStatusOrder(order.ID, SystemParam.STATUS_ORDER_CANCEL,null);
                                    output = output.GetPayOutputModel("Confirm Success", "00");
                                    return output;
                                }
                                else
                                {
                                    output = output.GetPayOutputModel("Order already confirmed", "02");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            output = output.GetPayOutputModel("Unknow error", "99");
                            oneSignalBus.SaveLog("Exception", ex.ToString());
                        }
                    }
                    else
                    {
                        output = output.GetPayOutputModel("Invalid signature", "97");
                    }
                }
                else
                    output = output.GetPayOutputModel("Order not found", "01");
            }
            catch (Exception ex)
            {
                output = output.GetPayOutputModel("Unknow error", "99");
                oneSignalBus.SaveLog("Exception", ex.ToString());
            }
            oneSignalBus.SaveLog(output.RspCode, output.Message);
            return output;
        }
    }
}
