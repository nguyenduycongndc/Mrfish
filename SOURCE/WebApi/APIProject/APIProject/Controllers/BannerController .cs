using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using PagedList.Mvc;
using Data.DB;

namespace APIProject.Controllers
{
    public class BannerController : BaseController
    {
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchBanner(int Page, string title, string fromDate, string toDate, int? status, int? typeSend, int? type)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.title = title;
                ViewBag.status = status;
                ViewBag.typeSend = typeSend;
                ViewBag.type = type;
                DateTime? startDate = Util.ConvertDate1(fromDate);
                DateTime? endDate = Util.ConvertDate1(toDate);
                //DateTime? startDate = Util.ConvertDate(fromDate);
                //DateTime? endDate = Util.ConvertDate(toDate);
                List<ListBannerModel> lstProduct = bannerBusiness.SearchBanner(Page, title, startDate, endDate, status, typeSend, type);
                return PartialView("TableBanner", lstProduct.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("TableBanner", new List<ListBannerModel>().ToPagedList(1, 1));
            }
        }

        [UserAuthenticationFilter]
        public ActionResult AddBanner()
        {
            return View();
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public JsonResult CreateBanner(string title, int typeSend, int type, string content, string imageUrl, int icheck)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                var result = bannerBusiness.CreateBanner(title, typeSend, type, content, imageUrl, icheck, userLogin.UserID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        [UserAuthenticationFilter]
        public PartialViewResult LoadBanner(int ID)
        {
            try
            {
                var Banner = bannerBusiness.LoadBanner(ID);
                return PartialView("EditBanner", Banner);
            }
            catch
            {
                return PartialView("EditBanner", new ListBannerModel());
            }
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int SaveEditBanner(int ID, string title, int typeSend, int type, string imageUrl, string content, int status, int icheck)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return bannerBusiness.SaveEditBannerData(ID, title, typeSend, type, imageUrl, content, status, icheck, userLogin.UserID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteBanner(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN && UserLogins.Role != SystemParam.ROLE_EDITOR)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return bannerBusiness.DeleteBanner(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
    }
}