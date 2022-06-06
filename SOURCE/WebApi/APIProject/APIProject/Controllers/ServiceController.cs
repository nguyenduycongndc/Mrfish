using APIProject.Models;
using Data.Business;
using Data.DB;
using Data.Model;
using Data.Model.APIApp;
//using APIProject.Models.APIApp;
using Data.Utils;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIProject.Controllers
{
    public class ServiceController : BaseAPIController
    {
        MrFishEntities cnn = new MrFishEntities();


        // đăng nhập
        [HttpPost]
        public JsonResultModel LoginApp(JObject input)
        {
            try
            {
                LoginAppInputModel item = JsonConvert.DeserializeObject<LoginAppInputModel>(input.ToString());
                string json, name = "", id = "", output = SystemParam.SERVER_ERROR, avatar = "", email = "";
                id = Util.ConvertPhone(item.Value);
                name = item.Password;
                var cus = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Phone == id && c.Status.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                if (cus == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PHONE_NOT_FOUND, SystemParam.MESSAGE_ERROR_PHONE_NOT_FOUND, "");
                if (id.Length > 0)
                {
                    var check = 1;
                    var lscus = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
                    var cu = lscus.Where(c => c.Phone.Equals(item.Value)).FirstOrDefault();
                    if (cu != null)
                    {
                        if (Util.CheckPass(name, cu.PasswordReset))
                        {
                            check = 2;
                        }
                    }
                    output = lgBus.CheckLoginApp(id,/* item.Type,*/ name, avatar, email, item.DeviceID);
                    if (output.Equals(SystemParam.LOGIN_FAIL))
                    {
                        return response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, output, null);
                    }
                    else if (output.Equals(SystemParam.MESSAGE_LOGIN_ACCOUNT_FAIL))
                    {
                        return response(SystemParam.ERROR, SystemParam.ERROR_LOGIN_ACCOUNT_FAIL, output, "");
                    }
                    int cusID = cnn.Customers.Where(u => u.Token.Equals(output)).FirstOrDefault().ID;

                    var outputdata = lgBus.GetuserInfor(cusID);
                    outputdata.CheckLoginPass = check;


                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, outputdata);
                }
                else
                {
                    return serverError();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }


        // lấy token request
        public string getTokenApp()
        {
            if (Request.Headers.Contains("token"))
            {
                return Request.Headers.GetValues("token").FirstOrDefault();
            }
            return "";
        }

        public JsonResultModel response(int status, int code, string message, object data)
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = status;
            result.Code = code;
            result.Message = message;
            result.Data = data;
            return result;
        }


        public JsonResultModel serverError()
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = SystemParam.ERROR;
            result.Code = SystemParam.ERROR;
            result.Message = SystemParam.SERVER_ERROR;
            result.Data = "";
            return result;
        }


        // thông tin người đăng nhập
        [HttpGet]
        public JsonResultModel UserInfor()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                //UserInforOutputModel query = new UserInforOutputModel();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, "", lgBus.GetuserInfor(cusID.Value));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }




        // thông tin màn trang chủ
        [HttpGet]
        public JsonResultModel Home()
        {
            try
            {
                string token = getTokenApp();

                int? customerID = null;
                if (token.Length > 0)
                {
                    customerID = Util.checkTokenApp(token);
                }

                HomeScreenOutPutModel query = new HomeScreenOutPutModel();

                if (customerID != null)
                {
                    Customer cus = cnn.Customers.Find(customerID.Value);
                    query.Point = cus.Point;
                    query.CouponCount = couponBus.GetCouponCount(customerID.Value);
                }
                query.ListCategory = cateBus.GetListCategory();
                query.ListBanner = newsBus.GetListBanner().Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                query.ListFlashSale = itemBus.GetListItemFlashSale().Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                query.ListHotItems = itemBus.GetListItemHot().Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, query);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // danh sách danh mục trên app
        [HttpGet]
        public JsonResultModel GetListCategory()
        {
            try
            {
                var data = cateBus.GetListCategory();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // danh sách sản phẩm trên app
        [HttpGet]
        public JsonResultModel GetListProduct(int page = 1, int limit = 10, string search = null, int? categoryID = null, int? isHot = null, int? orderPrice = null, int? isFlashSale = null)
        {
            try
            {
                DataPageListOutputModel model = new DataPageListOutputModel();
                var data = itemBus.GetListItem(page, limit, search, categoryID, isHot, orderPrice, isFlashSale);
                model.limit = limit;
                model.page = page;
                model.totalCountItem = data.TotalItemCount;
                model.data = data;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, model);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Chi tiết sản phẩm trên app
        [HttpGet]
        public JsonResultModel GetProductDetail(int ID)
        {
            try
            {
                string token = getTokenApp();
                int? cusID = Util.checkTokenApp(token);
                var data = itemBus.GetItemDetail(ID,cusID.GetValueOrDefault());
                if (data == null || data.ID ==0)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_NOT_EXIST_PRODUCT, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // danh sách sản phẩm tương tự trên app
        [HttpGet]
        public JsonResultModel GetProductRelative(int ID)
        {
            try
            {
                var data = itemBus.GetListItemRelative(ID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // danh sách giỏ hàng trên app
        [HttpGet]
        public JsonResultModel GetListCart()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                if (cartBus.CheckCartChange(cusID.Value))
                {
                    var data = cartBus.GetListCart(cusID.Value);
                    return response(SystemParam.SUCCESS, SystemParam.ERROR_CART_UPDATED, SystemParam.MESSAGE_CART_UPDATED, data);
                }
                else
                {
                    var data = cartBus.GetListCart(cusID.Value);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy số lượng số sản phẩm trong giỏ hàng
        [HttpGet]
        public JsonResultModel GetCartQuantity()
        {
            try
            {
                string token = getTokenApp();
                int? cusID = Util.checkTokenApp(token);
                var data = cartBus.GetCartCount(cusID.GetValueOrDefault());
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Thêm vào giỏ hàng trên app
        [HttpPost]
        public JsonResultModel AddCart([FromBody]AddCartModel model)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = cartBus.AddCart(model, cusID.Value);
                if (data == SystemParam.SUCCESS)
                {
                    var listCart = cartBus.GetListCart(cusID.Value);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, listCart);
                }
                else if (data == SystemParam.ERROR_ITEM_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_ITEM_NOT_FOUND, SystemParam.MESSAGE_ITEM_NOT_FOUND, "");
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_ADD_CART_FAIL, "");
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Cập nhật giỏ hàng trên app
        [HttpPost]
        public JsonResultModel UpdateCart([FromBody]UpdateCartModel model)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                if (cartBus.CheckCartChange(cusID.Value))
                {
                    return response(SystemParam.SUCCESS, SystemParam.ERROR_CART_UPDATED, SystemParam.MESSAGE_CART_UPDATED, "");
                }
                var data = cartBus.UpdateCart(model);
                if (data == SystemParam.SUCCESS)
                {
                    var listCart = cartBus.GetListCart(cusID.Value);
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, listCart);
                }
                else if (data == SystemParam.ERROR_CART_UPDATED)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_CART_UPDATED, SystemParam.MESSAGE_CART_UPDATED, "");
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_UPDATE_CART_FAIL, "");
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Xóa giỏ hàng trên app
        [HttpPost]
        public JsonResultModel DeleteCart([FromBody]DeleteCartModel model)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = cartBus.DeleteCart(model);
                if (data == SystemParam.SUCCESS)
                {
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
                }
                else
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_UPDATE_CART_FAIL, "");
                }

            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Danh sách thông tin người nhận
        [HttpGet]
        public JsonResultModel GetListReceiveAddress()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.GetListReceiveAddress(cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy thông tin người nhận mặc định
        [HttpGet]
        public JsonResultModel GetReceiveAddressDefault()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.GetReceiveAddressDefault(cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy chi tiết thông tin người nhận 
        [HttpGet]
        public JsonResultModel GetReceiveAddressDetail(int ID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.GetReceiveAddressDetail(ID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Thêm thông tin người nhận
        [HttpPost]
        public JsonResultModel AddReceiveAddress([FromBody] AddReceiveAddressModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.AddReceiveAddress(input, cusID.Value);
                if (data == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_ADD_RECEIVE_ADDRESS_FAIL, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Cập nhật thông tin người nhận
        [HttpPost]
        public JsonResultModel UpdateReceiveAddress([FromBody] UpdateReceiveAddressModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.UpdateReceiveAddress(input, cusID.Value);
                if (data == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_UPDATE_RECEIVE_ADDRESS_FAIL, "");
                }
                else if (data == SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND, SystemParam.MESSAGE_RECEIVE_ADDRESS_NOT_FOUND, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Xóa thông tin người nhận
        [HttpPost]
        public JsonResultModel DeleteReceiveAddress(DeleteReceiveAddressModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = receiveAddressBus.DeleteReceiveAddress(input.ID);
                if (data == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_UPDATE_RECEIVE_ADDRESS_FAIL, "");
                }
                else if (data == SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND, SystemParam.MESSAGE_RECEIVE_ADDRESS_NOT_FOUND, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Đặt hàng
        [HttpPost]
        public JsonResultModel CreateOrder(CreateOrderModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = orderBus.CreateOrder(input, cusID.Value);
                if (data == SystemParam.ERROR)
                {
                    return response(SystemParam.ERROR, SystemParam.FAIL, SystemParam.MESSAGE_CREATE_ORDER_FAIL, "");
                }
                else if (data == SystemParam.ERROR_ORDER_RECEIVE_ADDRESS_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND, SystemParam.MESSAGE_RECEIVE_ADDRESS_NOT_FOUND, "");
                }
                else if (data == SystemParam.ERROR_ORDER_CART_UPDATED)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_CART_UPDATED, SystemParam.MESSAGE_CART_UPDATED, "");
                }
                else if (data == SystemParam.ERROR_ORDER_CART_EMPTY)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_CART_EMPTY, SystemParam.MESSAGE_CART_EMPTY, "");
                }
                else if (data == SystemParam.ERROR_ORDER_COUPON_NOT_VALID)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_COUPON_NOT_VALID, SystemParam.MESSAGE_COUPON_NOT_VALID, "");
                }
                else if (data == SystemParam.ERROR_VNPAY_MONEY_EXCEED)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_VNPAY_MONEY_EXCEED, SystemParam.MESSAGE_VNPAY_MONEY_EXCEED, "");
                }
                if (input.PaymentType == SystemParam.PAYMENT_TYPE_VNPAY)
                {
                    var url = vnPayBus.GetUrl(data);
                    CreateOrderVNPayOutputModel output = new CreateOrderVNPayOutputModel
                    {
                        ID = data,
                        Url = url
                    };
                    var order = cnn.Orders.FirstOrDefault(x => x.ID.Equals(data));
                    order.UrlVnpay = url;
                    cnn.SaveChanges();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, output);
                }
                else
                {
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách đơn hàng
        [HttpGet]
        public JsonResultModel GetListOrder(int page = 1, int limit = SystemParam.LIMIT_DEFAULT, int? status = 0)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                DataPageListOutputModel model = new DataPageListOutputModel();
                var data = orderBus.GetListOrder(page, limit, status.GetValueOrDefault(), cusID.Value);
                model.limit = limit;
                model.page = page;
                model.totalCountItem = data.TotalItemCount;
                model.data = data;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy chi tiết đơn hàng
        [HttpGet]
        public JsonResultModel GetOrderDetail(int ID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = orderBus.GetOrderDetail(ID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Hủy đơn hàng
        [HttpPost]
        public JsonResultModel CancleOrder(CancleOrderInputModel input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = orderBus.ChangeStatusOrder(input.ID, SystemParam.STATUS_ORDER_CANCEL, null);
                if (data == SystemParam.ERROR_ORDER_NOT_FOUND)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_ORDER_NOT_FOUND, SystemParam.MESSAGE_ORDER_NOT_FOUND, "");
                }
                else if (data == SystemParam.ERROR_ORDER_STATUS_UPDATED)
                {
                    return response(SystemParam.ERROR, SystemParam.ERROR_ORDER_STATUS_UPDATED, SystemParam.MESSAGE_ORDER_STATUS_UPDATED, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách mã giảm giá
        [HttpGet]
        public JsonResultModel GetListCoupon()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                var data = couponBus.GetListCoupon(cusID.Value);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy chi tiết mã giảm giá
        [HttpGet]
        public JsonResultModel GetCouponDetail(int ID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                
                var data = couponBus.GetCouponDetail(ID, cusID.Value);
                if (data == null || data.ID == 0)
                {
                    return response(SystemParam.SUCCESS, SystemParam.FAIL, SystemParam.MESSAGE_NOT_EXIST_COUPON, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách bài viết
        [HttpGet]
        public JsonResultModel GetListNews(int type)
        {
            try
            {
                var data = newsBus.GetListNews(type);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy chi tiết bài viết
        [HttpGet]
        public JsonResultModel GetNewsDetail(int ID)
        {
            try
            {
                var data = newsBus.getNewsDetailApp(ID);
                if(data == null || data.ID == 0)
                {
                    return response(SystemParam.SUCCESS, SystemParam.FAIL, SystemParam.MESSAGE_NOT_EXIST_NEWS, "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, data);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách điểm tích lũy
        [HttpGet]
        public JsonResultModel GetListPointHistory(int page = 1, int limit = 10, string fromDate = "", string toDate = "")
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                DataPageListOutputModel model = new DataPageListOutputModel();
                var data = pointBus.GetListMemberPointHistory(page, limit, fromDate, toDate, cusID.Value);
                model.limit = limit;
                model.page = page;
                model.totalCountItem = data.TotalItemCount;
                model.data = data;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, model);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách lịch sử giới thiệu
        [HttpGet]
        public JsonResultModel GetListCustomerRefer(int page = 1, int limit = 10)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                DataPageListOutputModel model = new DataPageListOutputModel();
                var data = cusBus.GetListCustomerRefer(page, limit, cusID.Value);
                model.limit = limit;
                model.page = page;
                model.totalCountItem = data.TotalItemCount;
                model.data = data;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, model);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Lấy danh sách thông báo
        [HttpGet]
        public JsonResultModel GetListNotification(int page = 1, int limit = 10)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                DataPageListOutputModel model = new DataPageListOutputModel();
                var data = notiBus.GetListNotification(cusID.Value,page, limit );
                model.limit = limit;
                model.page = page;
                model.totalCountItem = data.TotalItemCount;
                model.data = data;
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, model);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // Đọc thông báo
        [HttpPost]
        public JsonResultModel ReadNotification(int ID)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
                notiBus.ReadNotifiction(ID);
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }
        // cập nhật thông tin người dùng
        [HttpPost]
        public JsonResultModel UpdateUser(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                UserInforOutputModel item = JsonConvert.DeserializeObject<UserInforOutputModel>(input.ToString());

                int res = lgBus.UpdateUser(item, cusID.Value);
                switch (res)
                {
                    case SystemParam.ERROR_EXIST_EMAIL:
                        return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_EXIST_EMAIL, "");
                    case SystemParam.ERROR_EXIST_PHONE:
                        return response(SystemParam.ERROR, SystemParam.ERROR_EXIST_PHONE, SystemParam.MESSAGE_ERROR_EXIST_PHONE, "");
                    case SystemParam.ERROR:
                        return response(SystemParam.ERROR, SystemParam.ERROR, SystemParam.MESSAGE_ERROR, "");
                    default:
                        return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lgBus.GetuserInfor(cusID.Value));
                }
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        [HttpPost]
        public JsonResultModel uploadImage()
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? customerID = Util.checkTokenApp(token);
                if (customerID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                Customer customer = cnn.Customers.Find(customerID.Value);
                if (customer.Role != SystemParam.ROLE_AGENT)
                    return response(SystemParam.ERROR, SystemParam.ERROR, "Tài khoản của bạn không phải đại lý. Không thể tải ảnh lên.", "");

                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count > 0)
                {
                    string rootFolder = HttpContext.Current.Server.MapPath(@"\Uploads\agent\");
                    var docfiles = new List<string>();
                    string urlFile = "";
                    foreach (string file in httpRequest.Files)
                    {
                        var postedFile = httpRequest.Files[file];
                        string name = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".jpg";
                        var filePath = HttpContext.Current.Server.MapPath(@"\Uploads\agent\" + name);
                        urlFile = "http://" + HttpContext.Current.Request.Url.Authority + "/Uploads/agent/" + name;
                        postedFile.SaveAs(filePath);
                        docfiles.Add(urlFile);

                    }
                    cnn.SaveChanges();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, docfiles);
                }
                else
                {
                    return serverError();
                }
            }
            catch (Exception e)
            {
                if (e.Message == "Maximum request length exceeded.")
                    return response(SystemParam.ERROR, SystemParam.ERROR, "Kích thước ảnh quá lớn. Tối đa 4MB", "");
                return serverError();
            }
        }




        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vnp_Amount"></param>
        ///// <param name="vnp_BankCode"></param>
        ///// <param name="vnp_CardType"></param>
        ///// <param name="vnp_OrderInfo"></param>
        ///// <param name="vnp_PayDate"></param>
        ///// <param name="vnp_ResponseCode"></param>
        ///// <param name="vnp_TmnCode"></param>
        ///// <param name="vnp_TransactionNo"></param>
        ///// <param name="vnp_TxnRef"></param>
        ///// <param name="vnp_SecureHashType"></param>
        ///// <param name="vnp_SecureHash"></param>
        ///// <param name="vnp_BankTranNo"></param>
        ///// <returns></returns>
        [HttpGet]
        public VNPayOutputModel vnp_ipn(string vnp_Amount, string vnp_BankCode, string vnp_CardType, string vnp_OrderInfo, string vnp_PayDate, string vnp_ResponseCode, string vnp_TmnCode, string vnp_TransactionNo, string vnp_TxnRef, string vnp_SecureHashType, string vnp_SecureHash, string vnp_BankTranNo = "")
        {
            VnpOutputModel vnp = new VnpOutputModel();
            vnp.vnp_Amount = vnp_Amount;
            vnp.vnp_BankCode = vnp_BankCode;
            vnp.vnp_BankTranNo = vnp_BankTranNo;
            vnp.vnp_CardType = vnp_CardType;
            vnp.vnp_OrderInfo = vnp_OrderInfo;
            vnp.vnp_PayDate = vnp_PayDate;
            vnp.vnp_ResponseCode = vnp_ResponseCode;
            vnp.vnp_TmnCode = vnp_TmnCode;
            vnp.vnp_TransactionNo = vnp_TransactionNo;
            vnp.vnp_TxnRef = vnp_TxnRef;
            vnp.vnp_SecureHashType = vnp_SecureHashType;
            vnp.vnp_SecureHash = vnp_SecureHash;
            return vnPayBus.GetVnpIpn(vnp);
        }
        [HttpPost]
        public JsonResultModel TestSocket(string content, int type = 1, int? orderID = null, int? customerID = null)
        {
            pushNotifyBus.testSocket(content,type,orderID,customerID);
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }


        // đăng xuất
        [HttpGet]
        public JsonResultModel Logout()
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? cusID = Util.checkTokenApp(token);
            if (cusID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

            Customer cust = cnn.Customers.Find(cusID.Value);
            cust.Token = "";
            cust.DeviceID = "";
            cnn.SaveChanges();
            return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, "");
        }



        // đăng ký tài khoản
        [HttpPost]
        public JsonResultModel register(JObject input)
        {
            try
            {
                RegisterModel item = JsonConvert.DeserializeObject<RegisterModel>(input.ToString());
                if (String.IsNullOrEmpty(item.Phone)
                    || String.IsNullOrEmpty(item.Password)
                    || String.IsNullOrEmpty(item.CustomerName)
                    || item.Phone.Length == 0
                    || item.Password.Length == 0
                    || item.CustomerName.Length == 0)
                {
                    return response(SystemParam.ERROR, SystemParam.REQUIRE_REGISTER, SystemParam.MESSAGE_REQUIRE_REGISTER, "");
                }
                item.Address = !String.IsNullOrEmpty(item.Address) ? item.Address : "";
                item.Email = !String.IsNullOrEmpty(item.Email) ? item.Email : "";
                if (item.Sex == null) item.Sex = SystemParam.GENDER_MEN;

                int res = register(item.Phone, item.Password, item.CustomerName, item.Email, item.Code, item.Address, item.ProvinceID, item.DistrictID, item.Sex.Value, item.WardID, item.DeviceID);
                if (res == SystemParam.ERROR_INVALID_PHONE)
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_PHONE, SystemParam.MESSAGE_ERROR_INVALID_PHONE, "");
                if (res == SystemParam.EXIST_PHONE_REGISTER)
                    return response(SystemParam.ERROR, SystemParam.EXIST_PHONE_REGISTER, SystemParam.MESSAGE_ERROR_EXIST_PHONE, "");
                if (res == SystemParam.EXIST_EMAIL_REGISTER)
                    return response(SystemParam.ERROR, SystemParam.EXIST_EMAIL_REGISTER, SystemParam.MESSAGE_ERROR_EXIST_EMAIL, "");
                if (res == SystemParam.NOT_FOUND)
                    return response(SystemParam.ERROR, SystemParam.NOT_FOUND, "Số điện thoại giới thiệu không chính xác!", "");
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, lgBus.GetuserInfor(res));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return serverError();
            }
        }

        public int register(string phone, string password, string name, string email, string code, string address, int? provinceID, int? districtID, int Sex, int? WardID, string DeviceID)
        {
            try
            {
                phone = Util.ConvertPhone(phone);
                if (!Util.validPhone(phone))
                    return SystemParam.ERROR_INVALID_PHONE;
                var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone.Equals(phone));
                if (customer != null && customer.Count() > 0)
                    return SystemParam.EXIST_PHONE_REGISTER;
                if (code != "")
                {
                    var checkPhone = cnn.Customers.FirstOrDefault(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone.Equals(code));
                    if (checkPhone == null)
                        return SystemParam.NOT_FOUND;

                }
                if (email != null && email.Length > 0)
                {
                    var customerEmail = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Email.Equals(email));
                    if (customerEmail != null && customerEmail.Count() > 0)
                        return SystemParam.EXIST_EMAIL_REGISTER;
                }
                var RankDefault = cnn.Rankings.FirstOrDefault(x => x.Level.Equals(1));
                Customer cus = new Customer();
                cus.Address = address;
                cus.DOB = DateTime.Now;
                cus.Name = name;
                cus.Code = code;
                cus.Type = SystemParam.TYPE_LOGIN_PHONE;
                cus.Phone = phone;
                cus.Password = Util.GenPass(password);
                cus.Token = Util.CreateMD5(DateTime.Now.ToString());
                cus.IsActive = SystemParam.ACTIVE;
                cus.Email = email;
                cus.ProvinceCode = provinceID;
                cus.DistrictCode = districtID;
                cus.WardID = WardID;
                cus.Point = 0;
                cus.Sex = Sex;
                if (RankDefault != null)
                {
                    cus.RankID = RankDefault.ID;
                }
                cus.Role = SystemParam.ROLE_CUSTOMER;
                cus.AvatarUrl = "";
                cus.LastLoginDate = DateTime.Now;
                cus.ExpireTocken = DateTime.Now.AddYears(1);
                cus.DeviceID = DeviceID;
                cus.Status = SystemParam.ACTIVE;
                cus.CraeteDate = DateTime.Now;
                cnn.Customers.Add(cus);
                cnn.SaveChanges();
                

                return cnn.Customers.OrderByDescending(u => u.ID).FirstOrDefault().ID;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }





        // đổi mật khẩu khi quên mk
        [HttpPost]
        public JsonResultModel ChangePassForgot(JObject input)
        {
            try
            {
                ChangePasswordForgotModel item = JsonConvert.DeserializeObject<ChangePasswordForgotModel>(input.ToString());
                if (String.IsNullOrEmpty(item.newPass) || item.newPass.Length == 0 || item.CustomerId == null || item.CustomerId == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
                Customer customer = cnn.Customers.Find(item.CustomerId);
                customer.Password = Util.GenPass(item.newPass);
                customer.ExpPasswordReset = null;
                customer.PasswordReset = null;
                customer.OTP = null;
                if (customer.Role == SystemParam.ROLE_ADMIN && customer.UserID != null)
                {
                    customer.User.PassWord = Util.GenPass(item.newPass);
                }
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_CHANGE_PASSWORD_SUCCESS, lgBus.GetuserInfor(item.CustomerId));
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }
        // đổi mật khẩu
        [HttpPost]
        public JsonResultModel ChangePass(JObject input)
        {
            try
            {
                string token = getTokenApp();
                if (token.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
                int? cusID = Util.checkTokenApp(token);
                if (cusID == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");

                ChangePasswordModel item = JsonConvert.DeserializeObject<ChangePasswordModel>(input.ToString());
                if (String.IsNullOrEmpty(item.password) || String.IsNullOrEmpty(item.newPassword) || item.password.Length == 0 || item.newPassword.Length == 0)
                    return response(SystemParam.ERROR, SystemParam.DATA_NOT_FOUND, SystemParam.DATA_NOT_FOUND_MESSAGE, "");
                if (item.password == item.newPassword)
                    return response(SystemParam.ERROR, SystemParam.ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD, SystemParam.MESSAGE_ERROR_NEW_PASSWORD_SAME_OLD_PASSWORD, "");
                Customer customer = cnn.Customers.Find(cusID.Value);

                if (Util.CheckPass(item.password, customer.Password) || (Util.CheckPass(item.password, customer.PasswordReset) && customer.ExpPasswordReset.HasValue ? DateTime.Compare(DateTime.Now, DateTime.Parse(customer.ExpPasswordReset.Value.ToString())) < 0 : false))
                {
                    customer.Password = Util.GenPass(item.newPassword);
                    customer.ExpPasswordReset = null;
                    customer.PasswordReset = null;
                    if (customer.Role == SystemParam.ROLE_ADMIN && customer.UserID != null)
                    {
                        customer.User.PassWord = Util.GenPass(item.newPassword);
                    }
                    cnn.SaveChanges();
                    return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_CHANGE_PASSWORD_SUCCESS, lgBus.GetuserInfor(cusID.Value));
                }
                return response(SystemParam.ERROR, SystemParam.ERROR_WRONG_PASSWORD, SystemParam.MESSAGE_ERROR_WRONG_PASSWORD, "");

            }
            catch (Exception ex)
            {
                return serverError();
            }
        }





        // quên mật khẩu
        [HttpPost]
        public JsonResultModel ForgotPass(JObject input)
        {
            try
            {
                UserInforOutputModel item = JsonConvert.DeserializeObject<UserInforOutputModel>(input.ToString());
                if (!item.Email.Contains('@') || item.Email.Length < 6)
                    return response(SystemParam.ERROR, SystemParam.ERROR_INVALID_EMAIL, SystemParam.MESSAGE_ERROR_INVALID_EMAIL, "");
                Customer cus = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Email.Equals(item.Email) && u.Role != SystemParam.ROLE_ADMIN).FirstOrDefault();
                if (cus == null)
                    return response(SystemParam.ERROR, SystemParam.ERROR_NOT_EXIST_EMAIL, SystemParam.MESSAGE_ERROR_NOT_EXIST_EMAIL, "");
                EmailBusiness email = new EmailBusiness();
                string OTP = "123456";
                Random rand = new Random();
                OTP = rand.Next(0, 1000000).ToString("D6");
                email.configClient(item.Email, "[THAY ĐỔI MẬT KHẨU]", " Mã OTP của bạn là: " + OTP);
                //cus.PasswordReset = Util.GenPass(SystemParam.PASS_DEFAULT);
                var datenow = DateTime.Now;
                var expdate = datenow.AddMinutes(2);
                cus.ExpPasswordReset = expdate;
                cus.OTP = Convert.ToInt32(OTP);
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.MESSAGE_OTP_SENT, "");
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }

        //Kiểm tra mã OTP
        [HttpGet]
        public JsonResultModel CheckOTP(int otp)
        {
            try
            {
                UserInforOutputModel data = new UserInforOutputModel();
                data = lgBus.CheckOTP(otp);
                if (data.OTP == 0)
                {
                    return response(SystemParam.ERROR, SystemParam.NOT_FOUND, "Mã OTP không hợp lệ. Vui lòng kiểm tra lại", "");
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, "", data);
            }
            catch (Exception ex)
            {
                return serverError();
            }
        }




        // danh sach tinh 
        [HttpGet]
        public JsonResultModel ListProvince()
        {
            try
            {
                List<ProvinceModel> pr = new List<ProvinceModel>();
                var province = (from pro in cnn.Provinces
                                orderby pro.Name ascending
                                select new ProvinceModel
                                {
                                    ProvinceID = pro.Code,
                                    Province_name = pro.Name,
                                    Type = pro.Type
                                }).ToList();
                if (province != null)
                {
                    pr = province.ToList();
                    ;
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, pr);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // danh sach quan huyen theo tinh
        [HttpGet]
        public JsonResultModel ListDistrict(int ProvinceID)
        {
            try
            {
                List<DistrictModel> districts = new List<DistrictModel>();
                var dstr = (from d in cnn.Districts
                            where d.ProvinceCode.Equals(ProvinceID)
                            orderby d.Name ascending
                            select new DistrictModel
                            {
                                ProvinceID = d.ProvinceCode,
                                DistrictID = d.Code,
                                DistrictName = d.Name,
                                Type = d.Type
                            }).ToList();
                if (dstr != null)
                {
                    districts = dstr.ToList();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, districts);

            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        // danh sach xa
        [HttpGet]
        public JsonResultModel ListWards(int DistrictID)
        {
            try
            {
                List<WardModel> ListWard = new List<WardModel>();
                var ward = (from w in cnn.Wards
                            where w.District_id.Equals(DistrictID)
                            orderby w.Name ascending
                            select new WardModel
                            {
                                DistrictID = w.District_id,
                                WardID = w.ID,
                                WardName = w.Name,
                                Type = w.Type
                            }).ToList();
                if (ward != null)
                {
                    ListWard = ward.ToList();
                }
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, ListWard);
            }
            catch
            {
                return serverError();
            }
        }


        // người dùng thay đổi avata
        [HttpPost]
        public JsonResultModel UploadAvartar()
        {
            string token = getTokenApp();
            if (token.Length == 0)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_NOT_FOUND, "");
            int? customerID = Util.checkTokenApp(token);
            if (customerID == null)
                return response(SystemParam.ERROR, SystemParam.ERROR_PASS_API, SystemParam.TOKEN_INVALID, "");
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                var cus = cnn.Customers.Where(c => c.ID.Equals(customerID.Value)).FirstOrDefault();
                var postedFile = httpRequest.Files[0];
                string name = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + ".jpg";
                var filePath = HttpContext.Current.Server.MapPath(@"\Uploads\files\" + name);
                postedFile.SaveAs(filePath);
                cus.AvatarUrl = name;
                cnn.SaveChanges();
                return response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCESS_MESSAGE, name);
            }
            else
            {
                return serverError();
            }
        }



    }
}