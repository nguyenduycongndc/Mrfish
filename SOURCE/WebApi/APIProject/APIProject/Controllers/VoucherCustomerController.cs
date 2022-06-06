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
    public class VoucherCustomerController : BaseController
    {
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.ranks = voucherCustomerBusiness.LoadRankCustomer();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchVoucherCustomer(int Page,string nameVoucher, string fromDate, string toDate, int? typeDiscount, int? status, int? rank)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.nameVoucher = nameVoucher;
                ViewBag.status = status;
                ViewBag.typeDiscount = typeDiscount;
                ViewBag.rank = rank;
                DateTime? startDate = Util.ConvertDate1(fromDate);
                DateTime? endDate = Util.ConvertDate1(toDate);
                //DateTime? startDate = Util.ConvertDate(fromDate);
                //DateTime? endDate = Util.ConvertDate(toDate);
                List<VoucherCustomerNewModel> lstVoucherCustomer = voucherCustomerBusiness.SearchVoucherCustomer(Page, nameVoucher, startDate, endDate, typeDiscount, status, rank);
                return PartialView("TableVoucherCustomer", lstVoucherCustomer.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("TableVoucherCustomer", new List<ListBannerModel>().ToPagedList(1, 1));
            }
        }

        [UserAuthenticationFilter]
        public ActionResult AddVoucherCustomer()
        {
            ViewBag.Rank = voucherCustomerBusiness.LoadRank();
            return View();
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public JsonResult CreateVoucherCustomer(string codeVoucherCustomerCreate, string nameVoucherCustomerCreate,
            string quantityVoucherCustomerCreate, int slTypeDiscountCreate, string discountCreate,
            int slStatusVoucherCustomerCreate, string[] testRank, string AddLogoVoucherCus,
            DateTime fromDateVoucherCreate, DateTime toDateVoucherCreate, string NoteCreateVoucherCustomer)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                var result = voucherCustomerBusiness.CreateVoucherCustomer(codeVoucherCustomerCreate, nameVoucherCustomerCreate,
                    quantityVoucherCustomerCreate, slTypeDiscountCreate, discountCreate, slStatusVoucherCustomerCreate,
                    testRank, AddLogoVoucherCus, fromDateVoucherCreate, toDateVoucherCreate, NoteCreateVoucherCustomer);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
        // DetailOrder
        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public ActionResult ShowEditVoucherCustomer(int ID)
        {
            ViewBag.rank = voucherCustomerBusiness.LoadRank();
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_ACCOUNTANT || UserLogins.Role == SystemParam.ROLE_CUSTOMER)
            {
                Session[Sessions.LOGIN] = null;
                return View("DetailVoucherCustomer", new VoucherCustomerNewModel());
            }
            else
                return View("DetailVoucherCustomer", voucherCustomerBusiness.DetailVoucherCustomer(ID));
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int SaveEditVoucherCustomer(int ID, int statusVouEdit, int quantityVouEdit, DateTime fromDateVoucherEdit, DateTime toDateVoucherEdit, string imgVou, string NoteVoucherEdit)
        {
            try
            {
                return voucherCustomerBusiness.saveEditVoucherCustomer(ID, statusVouEdit, quantityVouEdit, fromDateVoucherEdit, toDateVoucherEdit, imgVou, NoteVoucherEdit);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteVoucherCustomer(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN && UserLogins.Role != SystemParam.ROLE_EDITOR)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return voucherCustomerBusiness.DeleteVoucherCustomer(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}