using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Data.Model.APIWeb.OrderDetailEditOutput;
using ListOrderModel = Data.Model.APIApp.ListOrderModel;

namespace Data.Business
{
    public class OrderBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        public OrderBusiness(MrFishEntities context = null) : base()
        {

        }

        CartBusiness cartBus = new CartBusiness();
        CouponBusiness couponBus = new CouponBusiness();
        PushNotifyBusiness pushNotiBus = new PushNotifyBusiness();
        RequestAPIBusiness requestAPI = new RequestAPIBusiness();
        // tìm kiếm đơn hàng
        public List<ListOrderWebModel> Search(int? Status, DateTime? startdate, DateTime? endDate, string Phone, int? PaymentType)
        {
            try
            {
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1);
                //endDate = endDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                var query = (from o in cnn.Orders
                             where o.IsActive.Equals(SystemParam.ACTIVE)
                                && (startdate.HasValue ? o.CreateDate >= startdate.Value : true)
                                && (endDate.HasValue ? o.CreateDate <= endDate.Value : true)
                                && (Status.HasValue ? o.Status.Equals(Status.Value) : true)
                                && (PaymentType.HasValue ? o.PaymentType.Equals(PaymentType.Value) : true)
                                && (o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY) ? o.IsPayment.Equals(SystemParam.ORDER_PAID) : true)
                             orderby o.CreateDate descending
                             select new ListOrderWebModel
                             {
                                 ID = o.ID,
                                 CusName = o.Customer.Name,
                                 CusPhone = o.Customer.Phone,
                                 BuyerPhone = o.BuyerPhone,
                                 BuyerName = o.BuyerName,
                                 BuyerAddress = o.BuyerAddress,
                                 Code = o.Code,
                                 TotalPrice = o.TotalPrice,
                                 QTY = o.OrderItems.Sum(x => x.QTY),
                                 CreateDate = o.CreateDate,
                                 Status = o.Status,
                                 PaymentType = o.PaymentType,
                                 DiscountRank = o.DiscountRank,
                                 DiscountCoupon = o.DiscountCoupon,
                                 TotalPayment = (o.TotalPrice - o.DiscountCoupon - o.DiscountRank),
                                 ProvinceID = o.ProvinceID,
                                 ProvinceName = o.Province.Name,
                                 DistrictID = o.DistrictID,
                                 DistrictName = o.District.Name,
                                 WardID = o.WardID,
                                 WardName = o.Ward.Name,
                             }).ToList();
                if (Phone != "" && Phone != null)
                {
                    query = query.Where(x => x.BuyerPhone.Contains(Phone)
                    || Util.Converts(x.BuyerName.ToLower()).Contains(Util.Converts(Phone.ToLower()))
                    || x.Code.ToLower().Contains(Phone.ToLower())
                    || x.CusName.ToLower().Contains(Phone.ToLower())
                    || x.CusPhone.ToLower().Contains(Phone.ToLower())).ToList();
                }
                return query;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListOrderWebModel>();
            }
        }

        // Gen Code của đơn 
        public string GenerateCodeOrder()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 8;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public bool CheckCartChange(int CusID)
        {
            var listCart = cnn.Carts.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            var check = false;
            foreach (var cart in listCart)
            {
                var item = cnn.Items.FirstOrDefault(x => x.ID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE) && x.GroupItem.IsActive.Equals(SystemParam.ACTIVE) && x.GroupItem.Status.Equals(SystemParam.ACTIVE));
                if (item == null)
                {
                    cart.IsActive = SystemParam.ACTIVE_FALSE;
                    check = true;
                    cnn.SaveChanges();
                    continue;
                }
                var priceSale = cnn.ItemsSales.Where(x => x.ItemID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault();
                var quantitySale = cnn.ItemsSales.Where(x => x.ItemID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Remain).FirstOrDefault();
                if (cart.Price != item.Price || cart.PriceSale != priceSale || (cart.QuantitySale != quantitySale && cart.Quantity > quantitySale))
                {
                    check = true;
                }
                cart.Price = item.Price;
                cart.PriceSale = priceSale;
                cart.QuantitySale = quantitySale;
                cnn.SaveChanges();

            }
            return check;
        }
        public int CreateOrder(CreateOrderModel input, int CusID)
        {

            try
            {
                long totalPrice = 0, discountType = 0, discountValue = 0, discountRank = 0;
                long discountCoupon = 0;
                var cus = cnn.Customers.FirstOrDefault(x => x.ID.Equals(CusID));
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {

                    if (CheckCartChange(CusID))
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ERROR_ORDER_CART_UPDATED;
                    }
                    if (input.CouponID.HasValue)
                    {
                        if (couponBus.CheckCoupon(input.CouponID.Value, CusID))
                        {
                            var coupon = cnn.Coupons.FirstOrDefault(x => x.ID.Equals(input.CouponID.Value));
                            coupon.Remain--;
                            if (coupon.Type.Equals(SystemParam.COUPON_TYPE_RANK))
                            {
                                var couponUsed = new CouponCustomer
                                {
                                    CouponID = coupon.ID,
                                    CustomerID = CusID,
                                };
                                cnn.CouponCustomers.Add(couponUsed);
                                cnn.SaveChanges();
                            }
                            else
                            {
                                var couponRefer = cnn.CouponRefers.Where(x => x.CouponID.Equals(coupon.ID) && x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.EndDate > DateTime.Now).OrderBy(x => x.EndDate).FirstOrDefault();
                                couponRefer.IsActive = SystemParam.ACTIVE_FALSE;
                                cnn.SaveChanges();
                            }
                            discountType = coupon.TypeDiscount;
                            discountValue = coupon.Discount;

                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.ERROR_ORDER_COUPON_NOT_VALID;
                        }
                    }

                var listCart = cnn.Carts.Where(x => input.CartID.Contains(x.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.CustomerID.Equals(CusID)).ToList();
                if (listCart.Count() == 0)
                {
                    return SystemParam.ERROR_ORDER_CART_EMPTY;
                }
                var receiveAddress = cnn.ReceiveAddresses.FirstOrDefault(x => x.ID.Equals(input.ReceiveAddressID) && x.IsActive.Equals(SystemParam.ACTIVE));
                if (receiveAddress == null)
                {
                    return SystemParam.ERROR_ORDER_RECEIVE_ADDRESS_NOT_FOUND;
                }
                foreach (var item in listCart)
                {
                    if (item.Quantity > item.QuantitySale)
                    {
                        totalPrice += item.QuantitySale * item.PriceSale + (item.Quantity - item.QuantitySale) * item.Price;
                    }
                    else
                    {
                        totalPrice += item.Quantity * item.PriceSale;
                    }
                }
                if (discountType.Equals(SystemParam.COUPON_DISCOUNT_MONEY))
                {
                    discountCoupon = discountValue;
                }
                else if (discountType.Equals(SystemParam.COUPON_DISCOUNT_PERCENT))
                {
                    discountCoupon = totalPrice * discountValue / 100;
                }

                discountRank = (long)(cus.RankID.HasValue ? ((double)totalPrice * cus.Ranking.DiscountPercent / 100) : 0);

                totalPrice = totalPrice - discountCoupon - discountRank;
                if (totalPrice < 0)
                {
                    totalPrice = 0;
                }
                else if (totalPrice < 10000 && input.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY))
                {
                    return SystemParam.ERROR_VNPAY_MONEY_EXCEED;
                }
                var order = new Order
                {
                    Code = GenerateCodeOrder(),
                    CustomerID = CusID,
                    CouponID = input.CouponID,
                    TotalPrice = totalPrice,
                    DiscountCoupon = discountCoupon,
                    DiscountRank = discountRank,
                    BuyerName = receiveAddress.Name,
                    BuyerPhone = receiveAddress.Phone,
                    BuyerAddress = receiveAddress.Address,
                    Note = input.Note,
                    ProvinceID = receiveAddress.ProvinceID,
                    DistrictID = receiveAddress.DistrictID,
                    WardID = receiveAddress.WardID,
                    Status = SystemParam.STATUS_ORDER_PENDING,
                    PaymentType = input.PaymentType,
                    IsPayment = SystemParam.ORDER_NOT_PAID,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now
                };
                cnn.Orders.Add(order);
                cnn.SaveChanges();
                var orderHistory = new OrderHistory
                {
                    OrderID = order.ID,
                    Status = SystemParam.STATUS_ORDER_PENDING,
                    Title = SystemParam.STATUS_ORDER_PENDING_MSG,
                    CreateDate = DateTime.Now,
                };
                cnn.OrderHistories.Add(orderHistory);
                cnn.SaveChanges();
                foreach (var cart in listCart)
                {
                    var orderItem = new OrderItem
                    {
                        ItemID = cart.ItemID,
                        OrderID = order.ID,
                        QTY = cart.Quantity,
                        SumPrice = cart.Quantity > cart.QuantitySale ? (cart.QuantitySale * cart.PriceSale + (cart.Quantity - cart.QuantitySale) * cart.Price) : (cart.Quantity * cart.PriceSale),
                        Status = SystemParam.ACTIVE,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now
                    };
                    cnn.OrderItems.Add(orderItem);
                    cart.IsActive = SystemParam.ACTIVE_FALSE;
                    cnn.SaveChanges();
                    if (cart.QuantitySale > 0)
                    {
                        var itemSale = cnn.ItemsSales.FirstOrDefault(x => x.ItemID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now);
                        if (itemSale != null)
                        {
                            orderItem.ItemSaleID = itemSale.ID;
                            if (cart.QuantitySale < cart.Quantity)
                            {
                                itemSale.Remain -= cart.QuantitySale;
                                itemSale.QuantitySold += cart.QuantitySale;
                                orderItem.QTYSale = cart.QuantitySale;
                            }
                            else
                            {
                                itemSale.Remain -= cart.Quantity;
                                itemSale.QuantitySold += cart.Quantity;
                                orderItem.QTYSale = cart.Quantity;
                            }
                            cnn.SaveChanges();
                        }
                    }
                }
                if (order.PaymentType != SystemParam.PAYMENT_TYPE_VNPAY)
                {
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
                }
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return order.ID;

                }
            }

            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        public IPagedList<OrderModel> GetListOrder(int page, int limit, int status, int cusID)
        {
            try
            {
                var model = (from o in cnn.Orders
                             where o.IsActive.Equals(SystemParam.ACTIVE)
                             && o.CustomerID.Equals(cusID)
                                 && o.Status == status && (o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY) ? o.IsPayment.Equals(SystemParam.ORDER_PAID) : true)
                             orderby o.ID descending
                             select new OrderModel
                             {
                                 ID = o.ID,
                                 Address = o.BuyerAddress,
                                 BuyerName = o.BuyerName,
                                 BuyerPhone = o.BuyerPhone,
                                 Province = o.Province.Name,
                                 District = o.District.Name,
                                 Ward = o.Ward.Name,
                                 CustomerID = o.CustomerID,
                                 Status = o.Status,
                                 Code = o.Code,
                                 Note = o.Note,
                                 PaymentType = o.PaymentType,
                                 SumPrice = o.TotalPrice + o.DiscountCoupon + o.DiscountRank,
                                 TotalQuantity = o.OrderItems.Sum(x => x.QTY),
                                 DiscountCoupon = o.DiscountCoupon,
                                 DiscountRank = o.DiscountRank,
                                 TotalPrice = o.TotalPrice,
                                 CreateDate = o.CreateDate,
                                 UrlVnpay = o.UrlVnpay,
                                 IsPayment = o.IsPayment
                             }).AsEnumerable().Select(o => new OrderModel
                             {
                                 ID = o.ID,
                                 Address = o.Address,
                                 BuyerName = o.BuyerName,
                                 BuyerPhone = o.BuyerPhone,
                                 Province = o.Province,
                                 District = o.District,
                                 Ward = o.Ward,
                                 CustomerID = o.CustomerID,
                                 Status = o.Status,
                                 Code = o.Code,
                                 Note = o.Note,
                                 PaymentType = o.PaymentType,
                                 SumPrice = o.SumPrice,
                                 DiscountCoupon = o.DiscountCoupon,
                                 DiscountRank = o.DiscountRank,
                                 TotalPrice = o.TotalPrice,
                                 TotalQuantity = o.TotalQuantity,
                                 CreateDate = o.CreateDate,
                                 UrlVnpay = o.UrlVnpay,
                                 IsPayment = o.IsPayment,
                                 ListOrderItem = cnn.OrderItems.Where(x => x.OrderID.Equals(o.ID)).Select(x => new OrderDetailModel
                                 {
                                     ID = x.ID,
                                     ItemID = x.ItemID,
                                     ImageUrl = x.Item.ImageUrl,
                                     ItemName = x.Item.Name,
                                     SumPrice = x.SumPrice,
                                     Quantity = x.QTY
                                 }).AsEnumerable().Select(x => new OrderDetailModel
                                 {
                                     ID = x.ID,
                                     ItemID = x.ItemID,
                                     ItemName = x.ItemName,
                                     SumPrice = x.SumPrice,
                                     Quantity = x.Quantity,
                                     ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault()
                                 }).ToList()
                             }).ToPagedList(page, limit);
                return model;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<OrderModel>().ToPagedList(1, 1);
            }

        }
        public OrderModel GetOrderDetail(int ID)
        {
            try
            {
                var model = (from o in cnn.Orders
                             where o.ID.Equals(ID)
                             select new OrderModel
                             {
                                 ID = o.ID,
                                 Address = o.BuyerAddress,
                                 BuyerName = o.BuyerName,
                                 BuyerPhone = o.BuyerPhone,
                                 Province = o.Province.Name,
                                 District = o.District.Name,
                                 Ward = o.Ward.Name,
                                 CustomerID = o.CustomerID,
                                 Status = o.Status,
                                 Code = o.Code,
                                 Note = o.Note,
                                 PaymentType = o.PaymentType,
                                 SumPrice = o.TotalPrice + o.DiscountCoupon + o.DiscountRank,
                                 TotalQuantity = o.OrderItems.Sum(x => x.QTY),
                                 DiscountCoupon = o.DiscountCoupon,
                                 DiscountRank = o.DiscountRank,
                                 TotalPrice = o.TotalPrice,
                                 CreateDate = o.CreateDate,
                                 UrlVnpay = o.UrlVnpay,
                             }).AsEnumerable().Select(o => new OrderModel
                             {
                                 ID = o.ID,
                                 Address = o.Address,
                                 BuyerName = o.BuyerName,
                                 BuyerPhone = o.BuyerPhone,
                                 Province = o.Province,
                                 District = o.District,
                                 Ward = o.Ward,
                                 CustomerID = o.CustomerID,
                                 Status = o.Status,
                                 Code = o.Code,
                                 Note = o.Note,
                                 PaymentType = o.PaymentType,
                                 SumPrice = o.SumPrice,
                                 DiscountCoupon = o.DiscountCoupon,
                                 DiscountRank = o.DiscountRank,
                                 TotalPrice = o.TotalPrice,
                                 TotalQuantity = o.TotalQuantity,
                                 CreateDate = o.CreateDate,
                                 UrlVnpay = o.UrlVnpay,
                                 ListOrderItem = cnn.OrderItems.Where(x => x.OrderID.Equals(o.ID)).Select(x => new OrderDetailModel
                                 {
                                     ID = x.ID,
                                     ItemID = x.ItemID,
                                     ImageUrl = x.Item.ImageUrl,
                                     ItemName = x.Item.Name,
                                     SumPrice = x.SumPrice,
                                     Quantity = x.QTY
                                 }).AsEnumerable().Select(x => new OrderDetailModel
                                 {
                                     ID = x.ID,
                                     ItemID = x.ItemID,
                                     ItemName = x.ItemName,
                                     SumPrice = x.SumPrice,
                                     Quantity = x.Quantity,
                                     ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault()
                                 }).ToList()
                             }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new OrderModel();
            }

        }
        public void RemoveOrderNotPaidProcedure()
        {
            var time = DateTime.Now.AddMinutes(-SystemParam.TIME_REMOVE_ORDER_NOT_PAID);
            var order = cnn.Orders.Where(x => x.Status.Equals(SystemParam.STATUS_ORDER_PENDING) && x.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY) && x.IsPayment.Equals(SystemParam.ORDER_NOT_PAID)
                                        && x.CreateDate < time);
            foreach (var item in order)
            {
                ChangeStatusOrder(item.ID, SystemParam.CANCEL_STATUS_ORDER, null);
            }
        }
        public int ChangeStatusOrder(int ID, int status, int? userID)
        {
            try
            {
                var order = cnn.Orders.FirstOrDefault(x => x.ID.Equals(ID));
                var customer = cnn.Customers.FirstOrDefault(x => x.ID.Equals(order.CustomerID));
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {

                    if (order == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ERROR_ORDER_NOT_FOUND;
                    }
                    if (status == SystemParam.STATUS_ORDER_PROCESS)
                    {
                        if (order.Status == SystemParam.STATUS_ORDER_PENDING)
                        {
                            order.Status = status;
                            order.ProcessDate = DateTime.Now;
                            cnn.SaveChanges();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.ERROR_ORDER_STATUS_UPDATED;
                        }
                    }
                    else if (status == SystemParam.STATUS_ORDER_DONE)
                    {
                        if (order.Status == SystemParam.STATUS_ORDER_PROCESS)
                        {
                            order.Status = status;
                            order.CompleteDate = DateTime.Now;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.ERROR_ORDER_STATUS_UPDATED;
                        }
                    }
                    else if (status == SystemParam.STATUS_ORDER_CANCEL)
                    {
                        if (order.Status == SystemParam.STATUS_ORDER_PENDING || order.Status == SystemParam.STATUS_ORDER_PROCESS)
                        {
                            order.Status = status;
                            order.CancelDate = DateTime.Now;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.ERROR_ORDER_STATUS_UPDATED;
                        }
                    }
                    else if (status == SystemParam.STATUS_ORDER_REFUND)
                    {
                        if (order.Status == SystemParam.STATUS_ORDER_CANCEL && order.PaymentType != SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                        {
                            order.Status = status;
                            order.RefundDate = DateTime.Now;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.ERROR_ORDER_STATUS_UPDATED;
                        }
                    }
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                }
                if (status == SystemParam.STATUS_ORDER_PROCESS)
                {
                    var orderHistory = new OrderHistory
                    {
                        OrderID = ID,
                        Status = SystemParam.STATUS_ORDER_PROCESS,
                        Title = SystemParam.STATUS_ORDER_PROCESS_MSG,
                        CreateDate = DateTime.Now,
                        CreateBy = userID
                    };
                    cnn.OrderHistories.Add(orderHistory);
                    cnn.SaveChanges();
                    var title = "Đơn hàng " + order.Code + " vừa được xác nhận ";
                    pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_ORDER, customer, order.ID, null, null);
                }
                else if (status == SystemParam.STATUS_ORDER_DONE)
                {
                    var orderHistory = new OrderHistory
                    {
                        OrderID = ID,
                        Status = SystemParam.STATUS_ORDER_DONE,
                        Title = SystemParam.STATUS_ORDER_DONE_MSG,
                        CreateDate = DateTime.Now,
                        CreateBy = userID
                    };
                    cnn.OrderHistories.Add(orderHistory);
                    cnn.SaveChanges();
                    var title = "Đơn hàng " + order.Code + " vừa được hoàn thành ";
                    pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_ORDER, customer, order.ID, null, null);

                    if (customer != null)
                    {
                        var bonusPointPercent = cnn.Configs.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE));
                        var bonusValue = bonusPointPercent != null ? bonusPointPercent.Value : SystemParam.CONFIG_BONUS_POINT_PERCENT_VALUE_DEFAULT;
                        double totalPrice = order.TotalPrice;
                        var value = totalPrice * bonusValue / 100;
                        customer.Point += (int)value;
                        title = "Bạn vừa được cộng " + value + " điểm từ đơn hàng " + order.Code;
                        var memberPointHistory = new MembersPointHistory
                        {
                            CustomerID = customer.ID,
                            AddPointCode = order.Code,
                            Balance = customer.Point,
                            Point = (int)value,
                            CreateDate = DateTime.Now,
                            IsActive = SystemParam.ACTIVE,
                            Title = title,
                            Comment = title,
                            OrderID = order.ID,
                        };

                        cnn.MembersPointHistories.Add(memberPointHistory);
                        cnn.SaveChanges();
                        pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_POINT, customer, null, null, null);
                        if (customer.RankID.HasValue)
                        {
                            var listRank = cnn.Rankings.OrderByDescending(x => x.Level).ToList();
                            foreach (var item in listRank)
                            {
                                if (customer.Ranking.Level < item.Level && customer.Point >= item.MinPoint)
                                {
                                    var content = "Khách hàng " + customer.Name + " đã đủ điểm lên hạng " + item.RankName;
                                    var noti = new Notification
                                    {
                                        Content = content,
                                        Title = content,
                                        CustomerID = customer.ID,
                                        IsAdmin = SystemParam.ROLE_ADMIN,
                                        IsActive = SystemParam.ACTIVE,
                                        CreateDate = DateTime.Now,
                                        Viewed = SystemParam.NOTI_NOT_VIEWED,
                                        Type = SystemParam.NOTIFICATION_TYPE_RANK,
                                    };
                                    cnn.Notifications.Add(noti);
                                    cnn.SaveChanges();
                                    var data = SystemParam.URL_WEB_SOCKET + "?noti_id=" + noti.ID + "&content=" + content + "&customer_id=" + customer.ID + "&type=2";
                                    requestAPI.GetJson(data);
                                    break;
                                }
                            }
                        }

                    }
                    var countOrderDone = cnn.Orders.Where(x => x.CustomerID.Equals(customer.ID) && x.Status.Equals(SystemParam.STATUS_ORDER_DONE)).Count();
                    if (countOrderDone.Equals(1))
                    {
                        var PhoneRef = cnn.Customers.FirstOrDefault(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone.Equals(customer.Code));
                        var coupon = cnn.Coupons.FirstOrDefault(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE) && x.Type.Equals(SystemParam.COUPON_TYPE_REFER));
                        if (coupon != null && PhoneRef != null)
                        {
                            coupon.Quantity++;
                            coupon.Remain++;
                            var couponRefer = new CouponRefer
                            {
                                CouponID = coupon.ID,
                                CustomerID = PhoneRef.ID,
                                CusRefID = customer.ID,
                                CreateDate = DateTime.Now,
                                EndDate = DateTime.Now.AddDays(30),
                                IsActive = SystemParam.ACTIVE
                            };
                            cnn.CouponRefers.Add(couponRefer);
                            cnn.SaveChanges();
                            title = "Bạn vừa nhận được mã giảm giá từ khách hàng " + customer.Name;
                            pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_COUPON, PhoneRef, null, null, coupon.ID);
                        }
                    }
                }
                else if (status == SystemParam.STATUS_ORDER_CANCEL)
                {
                    var orderHistory = new OrderHistory
                    {
                        OrderID = ID,
                        Status = SystemParam.STATUS_ORDER_CANCEL,
                        Title = SystemParam.STATUS_ORDER_CANCEL_MSG,
                        CreateDate = DateTime.Now,
                        CreateBy = userID
                    };
                    cnn.OrderHistories.Add(orderHistory);
                    cnn.SaveChanges();
                    var orderItem = cnn.OrderItems.Where(x => x.OrderID.Equals(order.ID)).ToList();
                    foreach (var item in orderItem)
                    {
                        if (item.ItemSaleID.HasValue)
                        {
                            var itemSale = cnn.ItemsSales.FirstOrDefault(x => x.ID.Equals(item.ItemSaleID.Value));
                            itemSale.Remain += item.QTYSale.GetValueOrDefault();
                            itemSale.QuantitySold -= item.QTYSale.GetValueOrDefault();
                            cnn.SaveChanges();
                        }
                    }
                    if (order.CouponID.HasValue)
                    {
                        var coupon = cnn.Coupons.FirstOrDefault(x => x.ID.Equals(order.CouponID.Value));
                        coupon.Remain++;
                        cnn.SaveChanges();
                        if (coupon.Type.Equals(SystemParam.COUPON_TYPE_RANK))
                        {
                            var couponCustomer = cnn.CouponCustomers.FirstOrDefault(x => x.CouponID.Equals(order.CouponID.Value) && x.CustomerID.Equals(order.CustomerID));

                            if (couponCustomer != null)
                            {
                                cnn.CouponCustomers.Remove(couponCustomer);
                            }
                            cnn.SaveChanges();
                        }
                        else
                        {
                            var couponRefer = cnn.CouponRefers.Where(x => x.CouponID.Equals(coupon.ID) && x.CustomerID.Equals(order.CustomerID) && x.IsActive.Equals(SystemParam.ACTIVE_FALSE)).OrderByDescending(x => x.EndDate).FirstOrDefault();
                            if (couponRefer != null)
                            {
                                couponRefer.IsActive = SystemParam.ACTIVE;
                                cnn.SaveChanges();
                            }
                        }

                    }
                    if (userID.HasValue)
                    {
                        var title = "Đơn hàng " + order.Code + " vừa bị hủy ";
                        pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_ORDER, customer, order.ID, null, null);
                    }
                }
                else if (status == SystemParam.STATUS_ORDER_REFUND)
                {
                    var orderHistory = new OrderHistory
                    {
                        OrderID = ID,
                        Status = SystemParam.STATUS_ORDER_REFUND,
                        Title = SystemParam.STATUS_ORDER_REFUND_MSG,
                        CreateDate = DateTime.Now,
                        CreateBy = userID
                    };
                    cnn.OrderHistories.Add(orderHistory);
                    cnn.SaveChanges();
                    var title = "Bạn vừa được hoàn tiền từ đơn hàng " + order.Code;
                    pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_ORDER, customer, order.ID, null, null);

                }
                return SystemParam.SUCCESS;


            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }



        public List<ListOrderModel> GetListOrderByStatus(int? status, int cusID, string fromdate, string todate)
        {
            try
            {
                List<ListOrderModel> listOutput = new List<ListOrderModel>();
                var query = (from o in cnn.Orders
                             where o.IsActive.Equals(SystemParam.ACTIVE)
                             && o.CustomerID.Equals(cusID)
                             && (status.HasValue ? o.Status == status.Value : true)
                             orderby o.ID descending
                             select new ListOrderModel
                             {
                                 orderID = o.ID,
                                 totalPrice = o.TotalPrice,
                                 status = o.Status,
                                 qty = o.OrderItems.Count(),
                                 count = o.OrderItems.Select(u => u.QTY).ToList().Sum(),
                                 code = o.Code,
                                 ListImage = (from oi in cnn.OrderItems
                                              where oi.OrderID == o.ID
                                              select new
                                              {
                                                  image = oi.Item.ImageUrl
                                              }
                                          ).FirstOrDefault().image,
                                 name = (from oi in cnn.OrderItems
                                         where oi.OrderID == o.ID
                                         select new
                                         {
                                             name = oi.Item.Name
                                         }
                                          ).FirstOrDefault().name,
                                 CreateDate = o.CreateDate,
                                 CancelDate = o.CancelDate.Value,
                             }).ToList();
                if (query != null)
                {
                    var data = query.Select(d => new ListOrderModel
                    {
                        orderID = d.orderID,
                        totalPrice = d.totalPrice,
                        status = d.status,
                        qty = d.qty,
                        count = d.count,
                        code = d.code,
                        Image = d.ListImage.Split(',').ToList()[0],
                        ListImage = d.ListImage,
                        ArrImage = d.ListImage.Split(',').ToList(),
                        name = d.name,
                        CreateDate = d.CreateDate,
                        Date = d.CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR),
                        CancelDate = d.CancelDate,
                        ConfirmDate = d.ConfirmDate,
                        DeliveryDate = d.DeliveryDate,
                        PaymentDate = d.PaymentDate,
                        CountItem = d.count,
                    }).ToList();
                    listOutput = data.ToList();
                    return listOutput;
                }
                else
                    return new List<ListOrderModel>();
            }
            catch (Exception e)
            {
                e.ToString();
                return new List<ListOrderModel>();
            }
        }

        public ListOrderNewModel OrderDetailNew(int ID)
        {
            try
            {
                var query = cnn.Orders.Find(ID);
                var dataCus = cnn.Customers.Find(query.CustomerID);
                var dataProvince = cnn.Provinces.Find(query.ProvinceID);
                var dataDistrict = cnn.Districts.Find(query.DistrictID);
                var dataWard = cnn.Wards.Find(query.WardID);

                ListOrderNewModel detailOr = new ListOrderNewModel();
                detailOr.ID = ID;
                detailOr.Code = query.Code;
                detailOr.CustomerID = query.CustomerID;
                detailOr.CustomerName = dataCus.Name;
                detailOr.CustomerPhone = dataCus.Phone;
                detailOr.TotalPrice = query.TotalPrice;
                detailOr.DiscountCoupon = query.DiscountCoupon;
                detailOr.DiscountRank = query.DiscountRank;
                detailOr.TotalPayment = (query.TotalPrice + query.DiscountCoupon + query.DiscountRank);
                detailOr.CouponID = query.CouponID;
                detailOr.BuyerName = query.BuyerName;
                detailOr.BuyerPhone = query.BuyerPhone;
                detailOr.BuyerAddress = query.BuyerAddress;
                detailOr.Note = query.Note;
                detailOr.ProvinceID = query.ProvinceID;
                detailOr.ProvinceName = dataProvince.Name;
                detailOr.DistrictID = query.DistrictID;
                detailOr.DistrictName = dataDistrict.Name;
                detailOr.WardID = query.WardID;
                detailOr.WardName = dataWard.Name;
                detailOr.Status = query.Status;
                detailOr.PaymentType = query.PaymentType;
                detailOr.IsPayment = query.IsPayment;
                detailOr.IsActive = query.IsActive;
                detailOr.CreateDate = query.CreateDate;
                detailOr.ProcessDate = query.ProcessDate;
                detailOr.CompleteDate = query.CompleteDate;
                detailOr.CancelDate = query.CancelDate;
                detailOr.RefundDate = query.RefundDate;
                detailOr.ListItem = (from od in cnn.Orders
                                     join odi in cnn.OrderItems on od.ID equals odi.OrderID
                                     join its in cnn.ItemsSales on odi.ItemSaleID equals its.ID into itsi
                                     join it in cnn.Items on odi.ItemID equals it.ID
                                     join gr in cnn.GroupItems on it.GroupItemID equals gr.ID
                                     where od.IsActive.Equals(SystemParam.ACTIVE) && od.ID == ID
                                     select new ListProductTableModel
                                     {
                                         ID = od.ID,
                                         ItemCode = it.Code,
                                         ItemName = it.Name,
                                         GroupID = gr.ID,
                                         GroupItemName = gr.Name,
                                         OrderItemQTY = odi.QTY,
                                         ItemID = it.ID,
                                         OrderItemID = odi.ID,
                                         SumPrice = (long)(itsi.FirstOrDefault() == null ? odi.SumPrice : (itsi.FirstOrDefault().Price * odi.QTYSale)),
                                     }).ToList();
                return detailOr;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListOrderNewModel();
            }
        }



        public OrderDetailEditOutput ItemEdit(int ID)
        {
            try
            {
                var query = cnn.Orders.Find(ID);
                string codeOrder = query.Code;
                OrderDetailEditOutput edit = new OrderDetailEditOutput();
                edit.OrderID = query.ID;
                edit.Code = query.Code;
                edit.CusName = query.Customer.Name;
                edit.Phone = query.Customer.Phone;
                edit.CreateDate = query.CreateDate;
                edit.Status = query.Status;
                edit.BuyerName = query.BuyerName;
                edit.BuyerPhone = query.BuyerPhone;
                edit.BuyerAddress = query.BuyerAddress;
                //edit.ProvinceName = query.Province.Name;
                //edit.DistrictName = query.District.Name;
                //edit.WardName=query.Ward.Name;
                edit.Note = query.Note;


                var queryFindPoint = cnn.MembersPointHistories.Where(x => x.AddPointCode.Equals(codeOrder));

                if (queryFindPoint != null && queryFindPoint.Count() > 0)
                {
                    edit.addPoint = queryFindPoint.ToList().LastOrDefault().Point;
                }


                edit.ListItem = (from oi in cnn.OrderItems
                                 where oi.IsActive.Equals(SystemParam.ACTIVE) && oi.OrderID.Equals(ID)
                                 select new OrderItemEdit
                                 {
                                     ItemID = oi.ItemID,
                                     ItemName = oi.Item.Name,
                                     ItemCode = oi.Item.Code,
                                     ItemQTY = oi.QTY,
                                     ItemPrice = cnn.Items.Where(u => u.ID.Equals(oi.ItemID)).FirstOrDefault().Price,
                                     ItemTotalPrice = oi.SumPrice,
                                 }).ToList();
                return edit;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new OrderDetailEditOutput();
            }
        }
        public int SaveEdit(int ID, int Status, int? AddPoint, string BuyerName, string BuyerPhone, string BuyerAddress, long TotalPrice, int Discount, string Note = "")
        {
            try
            {
                PackageBusiness packageBusiness = new PackageBusiness();
                NotifyBusiness notifyBusiness = new NotifyBusiness();
                MembersPointHistory pointHistory = new MembersPointHistory();
                var itemEdit = cnn.Orders.Find(ID);
                var customer = cnn.Customers.Find(itemEdit.CustomerID);
                var PointNow = cnn.Customers.Find(itemEdit.CustomerID).Point;
                var statusCurrent = itemEdit.Status;
                if (AddPoint == null)
                {
                    AddPoint = 0;
                }

                if (Status == SystemParam.STATUS_ORDER_CANCEL)
                {
                    itemEdit.CancelDate = DateTime.Now;
                }

                itemEdit.TotalPrice = TotalPrice;
                itemEdit.BuyerName = BuyerName;
                itemEdit.BuyerPhone = BuyerPhone;
                itemEdit.BuyerAddress = BuyerAddress;
                itemEdit.Status = Status;
                itemEdit.Note = Note;
                if (itemEdit.Status == 0 || itemEdit.Status == 1 || itemEdit.Status == 3)
                {
                    customer.Point = 0 + PointNow;
                }
                else
                {
                    customer.Point = AddPoint.Value + PointNow;

                }
                cnn.SaveChanges();
                string titleNotify = "";
                string contentNoti = "";
                int typeNotify = 0;

                if (statusCurrent != Status && Status != SystemParam.STATUS_ORDER_PENDING)
                {
                    switch (Status)
                    {
                        case SystemParam.STATUS_ORDER_CANCEL:
                            titleNotify = "đã bị hủy";
                            typeNotify = SystemParam.NOTIFY_TYPE_ORDER_CANCEL;
                            break;
                        case SystemParam.STATUS_ORDER_DONE:
                            titleNotify = "đã hoàn thành";
                            typeNotify = SystemParam.NOTIFY_TYPE_ORDER_PAID;
                            break;
                        default:
                            break;
                    }
                    if (Status == SystemParam.STATUS_ORDER_DONE)
                    {
                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify;
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.NOTIFY_NAVIGATE_ORDER, contentNoti, contentNoti, ID);
                    }
                    else if (Status == SystemParam.STATUS_ORDER_CANCEL)
                    {
                        contentNoti = "Đơn hàng " + itemEdit.Code + " của bạn " + titleNotify;
                        notifyBusiness.CreateNoti(itemEdit.CustomerID, SystemParam.NOTIFY_NAVIGATE_ORDER, contentNoti, contentNoti, ID);
                    }
                    if (customer.DeviceID != null && customer.DeviceID.Length > 0)
                    {
                        NotifyDataModel notifyData = new NotifyDataModel();
                        notifyData.id = ID;
                        notifyData.type = SystemParam.NOTIFY_NAVIGATE_ORDER;
                        List<string> listDevice = new List<string>();
                        listDevice.Add(customer.DeviceID);
                    }
                }


                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int DeleteOrder(int ID)
        {
            try
            {
                var ItemDel = cnn.Orders.Find(ID);
                ItemDel.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public string countOrder()
        {
            int isDone = cnn.Orders.Count(x => x.Status == SystemParam.STATUS_ORDER_DONE && x.IsActive == SystemParam.ACTIVE);
            int orderCount = cnn.Orders.Where(x => x.IsActive == SystemParam.ACTIVE && (x.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY) ? x.IsPayment.Equals(SystemParam.ORDER_PAID) : true)).Count();
            return "" + isDone + " / " + orderCount;
        }

        public int countNewOrder()
        {
            int newOrder = cnn.Orders.Count(x => x.Status == SystemParam.STATUS_ORDER_PENDING && x.IsActive == SystemParam.ACTIVE && (x.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VNPAY) ? x.IsPayment.Equals(SystemParam.ORDER_PAID) : true));
            return newOrder;
        }
        //public string sumSale()
        //{
        //    var query = (from o in cnn.Orders
        //                 where o.IsActive.Equals(SystemParam.ACTIVE)
        //                 && o.Status.Equals(SystemParam.COMPLETED_STATUS_ORDER)
        //                 orderby o.CreateDate descending
        //                 select new ListOrderWebModel
        //                 {
        //                     TotalPrice = o.TotalPrice,
        //                 }).ToList();
        //    var x = query;
        //    return null;
        //    //    int isDone = query.Sum();
        //    //    int orderCount = cnn.Orders.Where(x => x.IsActive == SystemParam.ACTIVE).Count();
        //    //    return "" + isDone + " / " + orderCount;
        //}
        public async Task<long> sumSale()
        {
            try
            {
                //var today = DateTime.Today;
                //var first = new DateTime(today.Year, 1, 1);
                //var last = first.AddYears(1);

                var sale = cnn.Orders.Where(x => x.Status == SystemParam.COMPLETED_STATUS_ORDER && x.IsActive.Equals(SystemParam.ACTIVE)).Sum(x => (int?)x.TotalPrice) ?? 0;
                //var sale = cnn.Orders.Where(x => x.Status != SystemParam.COMPLETED_STATUS_ORDER && x.Status != SystemParam.PROCESSING_STATUS_ORDER).Sum(x => (int?)x.TotalPrice) ?? 0;

                //var sale2 = cnn.MembersTransactionHistories.Where(x => x.TransactionType.Equals(Constant.TYPE_TRANSACTION_RECHARGE)
                //&& x.Status.Equals(Constant.STATUS_TRANSACTION_SUCCESS) && x.CreateDate >= first && x.CreateDate <= last).Sum(x => x.Point) ?? 0;
                //return sale + sale2;
                return sale;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


        //xuất Excel One_Order
        public ExcelPackage ExportOneOrder(int ID)
        {
            try
            {
                ListOrderNewModel Detail = OrderDetailNew(ID);
                string path = HttpContext.Current.Server.MapPath(@"/Template/Donhang_Print_MRFISH.xlsx");
                FileInfo file = new FileInfo(path);
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                sheet.Cells[1, 4].Value = Detail.Code;
                sheet.Cells[2, 4].Value = Detail.CreateDate;
                sheet.Cells[5, 2].Value = Detail.BuyerName;
                sheet.Cells[6, 2].Value = Detail.BuyerPhone;
                sheet.Cells[7, 2].Value = Detail.BuyerAddress.Trim() + ", " + Detail.WardName + ", " + Detail.DistrictName + ", " + Detail.ProvinceName;
                sheet.Cells[8, 2].Value = Detail.Note;
                int row = 10;
                int stt = 1;
                foreach (var item in Detail.ListItem)
                {
                    sheet.Cells[row, 1].Value = stt.ToString();
                    sheet.Cells[row, 2].Value = item.ItemName;
                    sheet.Cells[row, 3].Value = item.OrderItemQTY.ToString();
                    sheet.Cells[row, 4].Value = @String.Format("{0:n0}", item.SumPrice);
                    row++;
                    stt++;
                }
                sheet.Cells[row + 1, 3].Value = "Tổng tiền hàng:";
                sheet.Cells[row + 1, 4].Value = @String.Format("{0:n0}", Detail.TotalPayment);
                sheet.Cells[row + 2, 3].Value = "Tổng tiền giảm(Hạng KH + Voucher):";
                sheet.Cells[row + 2, 4].Value = @String.Format("{0:n0}", Detail.DiscountCoupon + Detail.DiscountRank);
                sheet.Cells[row + 3, 3].Value = "Tổng tiền phải thanh toán:";
                sheet.Cells[row + 3, 4].Value = @String.Format("{0:n0}", Detail.TotalPrice);

                return pack;
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }
        public double Revenue()
        {
            return cnn.Orders.Where(x => x.Status == SystemParam.STATUS_ORDER_DONE && x.IsActive == SystemParam.ACTIVE).Sum(x => x.TotalPrice);
        }

        public ExcelPackage ExportExcelOrder(int? Status, DateTime? FromDate, DateTime? ToDate, string Phone, int? PaymentType)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Order.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 2;
                int stt = 1;

                var list = Search(Status, FromDate, ToDate, Phone, PaymentType);

                foreach (var item in list)
                {
                    sheet.Cells[row, 1].Value = stt;
                    sheet.Cells[row, 2].Value = item.Code;
                    sheet.Cells[row, 3].Value = item.CusName + "-" + item.CusPhone;
                    sheet.Cells[row, 4].Value = item.BuyerName + "-" + item.BuyerPhone;
                    sheet.Cells[row, 5].Value = item.BuyerAddress.Trim() + "," + item.WardName + "," + item.DistrictName + "," + item.ProvinceName;
                    sheet.Cells[row, 6].Value = item.TotalPrice;

                    //if (item.TypeGift == SystemParam.TYPE_GIFT_GIFT)
                    //{
                    //    sheet.Cells[row, 3].Value = "Quà tặng";
                    //}
                    //else if (item.TypeGift == SystemParam.TYPE_GIFT_VOUCHER)
                    //{
                    //    sheet.Cells[row, 3].Value = "Voucher";
                    //}
                    //else if (item.TypeGift == SystemParam.TYPE_GIFT_CARD)
                    //{
                    //    sheet.Cells[row, 3].Value = "Thẻ cào";
                    //}
                    if (item.Status == SystemParam.STATUS_ORDER_PENDING)
                    {
                        sheet.Cells[row, 7].Value = "Chờ xác nhận";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_PROCESS)
                    {
                        sheet.Cells[row, 7].Value = "Đang thực hiện";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_CANCEL)
                    {
                        sheet.Cells[row, 7].Value = "Hủy";
                    }
                    else if (item.Status == SystemParam.STATUS_ORDER_DONE)
                    {
                        sheet.Cells[row, 7].Value = "Đã hoàn thành";
                    }
                    sheet.Cells[row, 8].Value = item.CreateDate.ToString("dd/MM/yyyy");
                    row++;
                    stt++;
                }
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }
        }

        public List<ListProductTableModel> SearchListProductTable(int orderID)
        {
            var query = from od in cnn.Orders
                        join odi in cnn.OrderItems on od.ID equals odi.OrderID
                        join its in cnn.ItemsSales on odi.ItemSaleID equals its.ID into itsi
                        join it in cnn.Items on odi.ItemID equals it.ID
                        join gr in cnn.GroupItems on it.GroupItemID equals gr.ID
                        where od.IsActive.Equals(SystemParam.ACTIVE)
                        orderby odi.OrderID descending
                        select new ListProductTableModel
                        {
                            ID = od.ID,
                            ItemCode = it.Code,
                            ItemName = it.Name,
                            GroupID = gr.ID,
                            GroupItemName = gr.Name,
                            OrderItemQTY = odi.QTY,
                            ItemID = it.ID,
                            OrderItemID = odi.ID,
                            SumPrice = (long)(itsi.FirstOrDefault() == null ? odi.SumPrice : (itsi.FirstOrDefault().Price * odi.QTYSale)),
                        };
            var data = query.Where(x => x.ID == orderID).ToList();
            return data;
        }
        public List<ListOrderHistoryTableModel> SearchListOrderHistoryTable(int orderID, int userID, string username)
        {
            var query = from od in cnn.Orders
                        join odh in cnn.OrderHistories on od.ID equals odh.OrderID
                        //join us in cnn.Users on odh.CreateBy equals us.UserID
                        where od.IsActive.Equals(SystemParam.ACTIVE)
                        orderby odh.OrderID descending
                        select new ListOrderHistoryTableModel
                        {
                            ID = od.ID,
                            OrderID = orderID,
                            Status = odh.Status,
                            CreateDate = odh.CreateDate,
                            CreateBy = odh.CreateBy,
                            CreateByName = username,
                        };
            var data = query.Where(x => x.ID == orderID).ToList();
            return data;
        }
    }
}
