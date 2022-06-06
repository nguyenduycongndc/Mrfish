using APIProject.App_Start;
using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class OrderController : BaseController
    {
        public MrFishEntities cnn = new MrFishEntities();
        // GET: Order
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            @ViewBag.Revenue = orderBus.sumSale();
            return View();
        }
        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, int? Status, string FromDate, string ToDate, string Phone, int? PaymentType)
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.Role = UserLogins.Role;
            ViewBag.fromDate = FromDate;
            ViewBag.toDate = ToDate;
            ViewBag.Phone = Phone;
            ViewBag.Status = Status;
            ViewBag.PaymentType = PaymentType;
            DateTime? startDate = Util.ConvertDate1(FromDate);
            DateTime? endDate = Util.ConvertDate1(ToDate);
            return PartialView("_List", orderBus.Search(Status, startDate, endDate, Phone, PaymentType).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
        }

        [UserAuthenticationFilter]
        // ShowEdit
        public PartialViewResult ShowEditOrder(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_CUSTOMER)
            {
                Session[Sessions.LOGIN] = null;
                return PartialView("_EditForm", new OrderDetailEditOutput());
            }
            else
                return PartialView("_EditForm", orderBus.ItemEdit(ID));
        }
        [UserAuthenticationFilter]
        // DetailOrder
        public ActionResult ShowEditOrderNew(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_CUSTOMER)
            {
                Session[Sessions.LOGIN] = null;
                return View("DetailOrder", new OrderDetailEditOutput());
            }
            else
                return View("DetailOrder", orderBus.OrderDetailNew(ID));
        }

        //Save Edit
        [UserAuthenticationFilter]
        public int SaveEditOrder(int ID, int Status, int? AddPoint, string BuyerName, string BuyerPhone, string BuyerAddress, long TotalPrice, int Discount, string Note = "")
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role == SystemParam.ROLE_CUSTOMER)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                var detailOrder = cnn.Orders.Find(ID);
                if (Status == detailOrder.Status) { return SystemParam.RETURN_TRUE; }
                else { return orderBus.SaveEdit(ID, Status, AddPoint, BuyerName, BuyerPhone, BuyerAddress, TotalPrice, Discount, Note);}
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }

        }

        // Delete Order
        [UserAuthenticationFilter]
        public int DeleteOrder(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            if (UserLogins.Role == SystemParam.ROLE_CUSTOMER)
            {
                Session[Sessions.LOGIN] = null;
                return SystemParam.ERROR;
            }
            return orderBus.DeleteOrder(ID);
        }

        //[UserAuthenticationFilter]
        //public FileResult ExportBill(int ID)
        //{
        //    try
        //    {
        //        UserDetailOutputModel userLogin = UserLogins;
        //        return File(orderBus.ExportBill(ID, userLogin.UserName).GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BillForm.xlsx");

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return null;
        //    }
        //}
        [UserAuthenticationFilter]
        public FileResult ExportOneOrder(int ID)
        {
            try
            {
                //UserDetailOutputModel userLogin = UserLogins;
                return File(orderBus.ExportOneOrder(ID).GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Donhang_Print_MRFISH.xlsx");

            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }
        //export file excel
        [UserAuthenticationFilter]
        public FileResult ExportOrder(int? Status, DateTime? FromDate, DateTime? ToDate, string Phone, int? PaymentType)
        {
            return File(orderBus.ExportExcelOrder(Status, FromDate, ToDate, Phone, PaymentType).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Order.xlsx");
        }

        [UserAuthenticationFilter]
        public PartialViewResult searchListProductTable(int Page, int orderID)
        {
            ViewBag.orderID = orderID;
            IPagedList<ListProductTableModel> list = orderBus.SearchListProductTable(orderID).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("ListProductTable", list);
        }
        [UserAuthenticationFilter]
        public PartialViewResult searchListOrderHistory(int Page, int orderID)
        {
            ViewBag.orderID = orderID;
            UserDetailOutputModel userLogin = UserLogins;
            IPagedList<ListOrderHistoryTableModel> list = orderBus.SearchListOrderHistoryTable(orderID, userLogin.UserID, userLogin.UserName).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("ListOrderHistoryTable", list);
        }
        public int SaveEditOrderNew(int ID, int StatusOrder)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return orderBus.ChangeStatusOrder(ID, StatusOrder, userLogin.UserID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

    }
}