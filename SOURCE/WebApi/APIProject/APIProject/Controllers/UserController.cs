using APIProject.App_Start;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        [UserAuthenticationFilter]
        public ActionResult Index()
        {

            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role != SystemParam.ROLE_ADMIN)
            {
                Session[Sessions.LOGIN] = null;
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult Detail()
        {
            ViewBag.listProvince = userBusiness.DetailProvince();

            UserDetailOutputModel userLogin = UserLogins;
            //if (UserLogins.Role != SystemParam.ROLE_ACCOUNTANT)
            //{
            //    Session[Sessions.LOGIN] = null;
            //    return RedirectToAction("Login", "Home");
            //}
            CustomerModel userDetail = userBusiness.GetUserDetail(userLogin.UserID);

            return View(userDetail);
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string Phone, string FromDate, string ToDate, int? Status, int? Role)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN)
                {
                    Session[Sessions.LOGIN] = null;
                }
                ViewBag.Phone = Phone;
                ViewBag.PageCurrent = Page;
                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                ViewBag.Status = Status;
                ViewBag.Role = Role;
                return PartialView("_TableUser", userBusiness.Search(Page, Phone, FromDate, ToDate, Status, Role));
            }
            catch
            {
                return PartialView("_TableUser", new List<UserDetailOutputModel>().ToPagedList(1, 1));
            }
        }
        [HttpPost]
        public JsonResult CreateUser(string UserName, string Phone, string DOB, string Address,
            int Sex, string Email, int Role,/* int Status,*/
            string password, string AvatarUrl, string Note)
        {
            var result = userBusiness.CreateUser(UserName, Phone, DOB, Address, Sex, Email, Role,/* Status,*/ password, AvatarUrl, Note);
            return Json(result, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult addUser()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public int ChangePassword(string CurrentPassword, string NewPassword)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return userBusiness.ChangePassword(userLogin.UserID, CurrentPassword, NewPassword);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        //[UserAuthenticationFilter]
        public PartialViewResult GetUserDetail(int ID)
        {
            ViewBag.listProvince = userBusiness.DetailProvince();

            try
            {
                CustomerModel userDetail = userBusiness.GetUserDetail(ID);
                return PartialView("_UserDetail", userDetail);
            }
            catch
            {
                return PartialView("_UserDetail", new CustomerModel());
            }
        }

        [UserAuthenticationFilter]
        //public int UpdateUser(int ID, string userName, string phone)
        //{
        //    try
        //    {
        //        UserDetailOutputModel userLogin = UserLogins;
        //        if (UserLogins.Role != SystemParam.ROLE_ADMIN)
        //        {
        //            Session[Sessions.LOGIN] = null;
        //            return SystemParam.ERROR;
        //        }
        //        return userBusiness.UpdateUser(ID, userName, phone);
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}
        public JsonResult UpdateUser(int ID, string UserName, string Phone, string DOB, string Address,
            int Sex, string Email, int Role, int Status, string AvatarUrl, string Note, int Province)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role == SystemParam.ROLE_CUSTOMER)
                {
                    Session[Sessions.LOGIN] = null;
                    return null;
                }
                var result = userBusiness.UpdateUser(ID, UserName, Phone, DOB, Address, Sex, Email, Role, Status, AvatarUrl, Note, Province);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                throw;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteUser(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return userBusiness.DeleteUser(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int RefreshUser(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return userBusiness.RefreshUser(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}