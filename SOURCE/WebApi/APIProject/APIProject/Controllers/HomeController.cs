using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Business;
using Data.Utils;
using APIProject.App_Start;
using Newtonsoft.Json;

namespace APIProject.Controllers
{
    public class HomeController : BaseController
    {

        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.CountCus = cusBusiness.countCustomer();
            ViewBag.countItem = itemBusiness.countItem();

            ViewBag.countOrder = orderBus.countOrder();
            return View();
        }

        //lưu lại thông tin đối tượng vừa đăng nhập
        [UserAuthenticationFilter]
        public JsonResult GetUserLogin()
        {
            try
            {
                if(Session[Sessions.LOGIN] != null)
                {
                    UserDetailOutputModel userLogin = (UserDetailOutputModel) Session[Sessions.LOGIN];
                    return Json(userLogin, JsonRequestBehavior.AllowGet);
                }
                return Json(new UserDetailOutputModel(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new UserDetailOutputModel(), JsonRequestBehavior.AllowGet);
            }
        }
        public int CountNewOrder()
        {
            return orderBus.countNewOrder();
        }

        public ActionResult Login()
        {
            ViewBag.Title = "Login Page";
            return View();
        }
        public PartialViewResult _ListNotification()
        {
            ViewBag.countNoti = notifyBusiness.countNotiNotView();
            var model = notifyBusiness.GetListNotificationWeb();
            return PartialView(model);
        }
        public ActionResult ReadNotification(int ID , int type , int? orderID = null ,int? customerID = null)
        {
            notifyBusiness.ReadNotifiction(ID);
            if(type == SystemParam.NOTIFICATION_TYPE_ORDER)
            {
                return RedirectToAction("ShowEditOrderNew", "Order", new { ID = orderID.Value });
            }
            else
            {
                return RedirectToAction("CustomerDetailNew", "Customer", new { ID = customerID.Value });
            }
        }

        //đăng nhập web
        public int UserLogin(string phone, string password)
        {
            try
            {
                UserDetailOutputModel userLogin = loginBusiness.CheckLoginWeb(phone, password);
                if (userLogin != null && userLogin.UserID > 0)
                {
                    Session[Sessions.LOGIN] = userLogin;

                    HttpCookie ccRoles = new HttpCookie(Coockies.ROLES);
                    ccRoles.Value = userLogin.Role.ToString();
                    Response.Cookies.Add(ccRoles);

                    return SystemParam.SUCCESS;
                }
                else
                {
                    return SystemParam.FAIL_LOGIN;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        //đăng xuất
        public int Logout()
        {
            try
            {
                Session[Sessions.LOGIN] = null;
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        
 

    }
}
