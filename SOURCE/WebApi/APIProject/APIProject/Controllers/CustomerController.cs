using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using Data.Utils;
using Data.Model;
using Data.Model.APIWeb;
using Data.DB;

namespace APIProject.Controllers
{
    public class CustomerController : BaseController
    {

        // GET: Customer
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.listProvince = cusBusiness.DetailProvince();
            ViewBag.listCity = cusBusiness.LoadCityCustomer();
            ViewBag.rank = cusBusiness.LoadRankCustomer();
            return View();

        }
        [UserAuthenticationFilter]
        public PartialViewResult LoadDistrict(int ProvinceID)
        {
            ViewBag.listDistrict = cusBusiness.loadDistrict(ProvinceID);
            return PartialView("_ListDistrict");
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string FromDate, string ToDate, int? City, string Phone, int? Rank, int? Status)
        {
            ViewBag.FromDateCus = FromDate;
            ViewBag.ToDateCus = ToDate;
            ViewBag.PhoneSearch = Phone;
            ViewBag.City = City;
            ViewBag.Status = Status;
            ViewBag.Rank = Rank;
            DateTime? startDate = Util.ConvertDate1(FromDate);
            DateTime? endDate = Util.ConvertDate1(ToDate);
            IPagedList<ListCustomerOutputModel> list = cusBusiness.Search(startDate, endDate, City, Phone, Rank, Status).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            //ViewBag.listCustomer = cusBusiness.Search(FromDate, ToDate, City, District, Phone).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListCustomer", list);
        }


        [UserAuthenticationFilter]
        public PartialViewResult CustomerDetail(int? ID)
        {
            ViewBag.rank = cusBusiness.LoadRankCustomer();
            ViewBag.listProvince = cusBusiness.DetailProvince();

            ViewBag.CusDetail = cusBusiness.cusDetail(ID);
            ViewBag.MemberRank = rankBusiness.getRankByLever(1);
            ViewBag.SliverRank = rankBusiness.getRankByLever(3);
            ViewBag.GoldRank = rankBusiness.getRankByLever(3);
            ViewBag.PlatinumRank = rankBusiness.getRankByLever(4);
            return PartialView("_CustomerDetail");
        }
        
        [UserAuthenticationFilter]
        public ActionResult CustomerDetailNew(int ID)
        {
            ViewBag.rank = cusBusiness.LoadRankCustomer();
            ViewBag.listProvince = cusBusiness.DetailProvince();
            ViewBag.sumPoint = cusBusiness.SumPoint(ID);
            ListCustomerDetailNewModel data = cusBusiness.BindingCus(ID);
            return View("_CustomerDetailNew", data);
        }

        public int ChangeRole(int ID)
        {
            return cusBusiness.ChangeRole(ID);
        }
        public int ChangeRoleBT(int ID)
        {
            return cusBusiness.ChangeRoleBT(ID);
        }
        [UserAuthenticationFilter]
        public PartialViewResult SearchHistoryPoint(int Page, int cusID, string FromDate, string ToDate)
        {
            ViewBag.cusID = cusID;
            ViewBag.FromDateHis = FromDate;
            ViewBag.ToDateHis = ToDate;
            IPagedList<GetListHistoryMemberPointInputModel> list = cusBusiness.SearchHistoryPoint(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_ListHistoryPoint", list);
        }
        [UserAuthenticationFilter]
        public PartialViewResult SearchMemberPointHistory(int Page, int cusID)
        {
            ViewBag.cusID = cusID;
            IPagedList<MemberPointHistoryWebModel> list = cusBusiness.searchMemberPointHistoryWeb(cusID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("MemberPointHistory", list);
        }
        [UserAuthenticationFilter]
        public PartialViewResult searchReceiveAddress(int Page, int cusID)
        {
            ViewBag.cusID = cusID;
            IPagedList<ListReceiveAddressModel> list = cusBusiness.searchReceiveAddress(cusID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("ReceiveAddress", list);
        }        
        
        [UserAuthenticationFilter]
        public PartialViewResult searchOrderNew(int Page, int cusID)
        {
            ViewBag.cusID = cusID;
            IPagedList<ListOrderNewModel> list = cusBusiness.SearchOrderNew(cusID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("OrderNew", list);
        }
        
        [UserAuthenticationFilter]
        public PartialViewResult searchReferralCode(int Page, int cusID)
        {
            ViewBag.cusID = cusID;
            IPagedList<ListReferralCodeNewModel> list = cusBusiness.SearchReferralCode(cusID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("ReferralCode", list);
        }

        [UserAuthenticationFilter]
        public int DeleteCustomer(int ID)
        {
            return cusBusiness.DeleteCustomer(ID);
        }
        [UserAuthenticationFilter]
        //[ValidateInput(false)]
        public int SaveEditCustomer(int ID, int RankCus, int StatusCus)
        {
            try
            {
                return cusBusiness.SaveEditCus(ID, RankCus, StatusCus);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        public ActionResult GoHome()
        {
            return Json(Url.Action("Index", "Customer"));
        }
        // Reset mật khẩu
        public int RefreshCustomer(int id)
        {
            try
            {
                return cusBusiness.RefreshCustomer(id);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
    }
}