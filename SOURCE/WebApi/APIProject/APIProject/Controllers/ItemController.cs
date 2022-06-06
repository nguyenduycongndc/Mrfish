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

namespace APIProject.Controllers
{
    public class ItemController : BaseController
    {
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.listCategory = itemBusiness.getListCategoryEdit();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchWeb(int Page, string fromDate, string toDate, string itemName, string category, int? Status, int? IsHot)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.ItemName = itemName;
                ViewBag.Category = category;
                ViewBag.Status = Status;
                ViewBag.IsHot = IsHot;
                DateTime? startDate = Util.ConvertDate1(fromDate);
                DateTime? endDate = Util.ConvertDate1(toDate);
                //DateTime? startDate = Util.ConvertDate(fromDate);
                //DateTime? endDate = Util.ConvertDate(toDate);
                List<ListItemWebModel> lstProduct = itemBusiness.SearchWeb(Page, startDate, endDate, itemName, category, Status, IsHot);
                return PartialView("_TableItem", lstProduct.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableItem", new List<ListItemWebModel>().ToPagedList(1, 1));
            }
        }
        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string fromDate, string toDate, string itemName, string category, int? Status, int? IsHot)
        {
            try
            {
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.ItemName = itemName;
                ViewBag.Category = category;
                ViewBag.Status = Status;
                ViewBag.IsHot = IsHot;
                DateTime? startDate = Util.ConvertDate(fromDate);
                DateTime? endDate = Util.ConvertDate(toDate);
                List<ListItemOutputModel> lstProduct = itemBusiness.Search(Page, startDate, endDate, itemName, category, Status, IsHot);
                return PartialView("_TableItem", lstProduct.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableItem", new List<ListItemOutputModel>().ToPagedList(1, 1));
            }
        }
        [UserAuthenticationFilter]
        public ActionResult AddItem()
        {
            ViewBag.listCategory = itemBusiness.getListCategory();
            return View();
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int CreateItem(int? CategoryID, int? Special, string Code, string Name, string Price, string ImageUrl,/* string Tech,*/ string Note , int IsHot)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return itemBusiness.CreateItem(CategoryID.Value, Special, Code, Name,  Price,  ImageUrl,/* Tech,*/ Note, IsHot);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult LoadItem(int ID)
        {
            try
            {
                ViewBag.listCategoryEdit = itemBusiness.getListCategoryEdit();
                var Item = itemBusiness.LoadItem(ID);
                return PartialView("_EditItem", Item);
            }
            catch
            {
                return PartialView("_EditItem", new CreateItemInputModel());
            }
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        // Lưu lại cập nhật sản phẩm
        public int SaveEditItem(int ID,/*int? Special,*/ string Code, string Name, int? Status, int? IsHot, int? CategoryID, string ImageUrl, string Note,/*string Tech,*/ string Price)
        {
            try
            {
                return itemBusiness.SaveEditItem(ID,/*Special,*/ Code, Name, Status.Value, IsHot.Value, CategoryID.Value, ImageUrl, Note,/* Tech,*/ Price);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteItem(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN && UserLogins.Role != SystemParam.ROLE_EDITOR)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return itemBusiness.DeleteItem(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}