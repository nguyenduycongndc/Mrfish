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
    public class VoucherIntroduceController : BaseController
    {
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchVoucherIntroduce(int Page,string nameVoucher, string fromDate, string toDate, int? typeDiscountVoucherIntroduce)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.nameVoucher = nameVoucher;
                ViewBag.typeDiscountVoucherIntroduce = typeDiscountVoucherIntroduce;
                DateTime? startDate = Util.ConvertDate1(fromDate);
                DateTime? endDate = Util.ConvertDate1(toDate);
                //DateTime? startDate = Util.ConvertDate(fromDate);
                //DateTime? endDate = Util.ConvertDate(toDate);
                List<VoucherIntroduceNewModel> lstVoucherIntroduce = voucherIntroduceBusiness.SearchVoucherIntroduce(Page, nameVoucher, startDate, endDate, typeDiscountVoucherIntroduce);
                return PartialView("TableVoucherIntroduce", lstVoucherIntroduce.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("TableVoucherIntroduce", new List<ListBannerModel>().ToPagedList(1, 1));
            }
        }

        [UserAuthenticationFilter]
        public ActionResult AddVoucherIntroduce()
        {
            return View();
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public JsonResult CreateVoucherIntroduce(string codeVouCreate, string nameVouCreate, int typeDiscountVouCreate, string discountVouCreate, string Note, string AddLogoVoucherCus)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                var result = voucherIntroduceBusiness.CreateVoucherIntroduce(codeVouCreate, nameVouCreate, typeDiscountVouCreate, discountVouCreate, Note, AddLogoVoucherCus);
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
        public ActionResult ShowEditVoucherIntroduce(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_ACCOUNTANT || UserLogins.Role == SystemParam.ROLE_CUSTOMER)
            {
                Session[Sessions.LOGIN] = null;
                return View("EditVoucherIntroduce", new VoucherIntroduceNewModel());
            }
            else
                return View("EditVoucherIntroduce", voucherIntroduceBusiness.DetailVoucherIntroduce(ID));
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int SaveEditVoucherIntroduce(int ID, string imgVoucherIntroduceEdit, string NoteVoucherIntroduceEdit, int status, string nameVouEdit, int typeDiscountVouEdit, string discountVouEdit)
        {
            try
            {
                return voucherIntroduceBusiness.saveEditVoucherIntroduce(ID, imgVoucherIntroduceEdit, NoteVoucherIntroduceEdit, status, nameVouEdit, typeDiscountVouEdit, discountVouEdit);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteVoucherIntroduce(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN && UserLogins.Role != SystemParam.ROLE_EDITOR)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return voucherIntroduceBusiness.DeleteVoucherIntroduce(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}