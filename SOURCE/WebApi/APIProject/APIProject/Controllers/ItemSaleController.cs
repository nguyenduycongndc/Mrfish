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
    public class ItemSaleController : BaseController
    {
        public MrFishEntities cnn = new MrFishEntities();
        // GET: Products
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            ViewBag.listCategory = itemSaleBusiness.getListCategory();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult SearchItemSaleWeb(int Page, string fromDateCreate, string toDateCreate, string itemSaleName, string category)
        {
            try
            {
                ViewBag.fromDateCreate = fromDateCreate;
                ViewBag.toDateCreate = toDateCreate;
                ViewBag.itemSaleName = itemSaleName;
                ViewBag.category = category;
                DateTime? startDate = Util.ConvertDate1(fromDateCreate);
                DateTime? endDate = Util.ConvertDate1(toDateCreate);
                //DateTime? startDate = Util.ConvertDate(fromDateCreate);
                //DateTime? endDate = Util.ConvertDate(toDateCreate);
                List<ListItemSaleWebModel> lstProduct = itemSaleBusiness.SearchItemSaleWeb(Page, startDate, endDate, itemSaleName, category);
                return PartialView("TableItemSale", lstProduct.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB));
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("TableItemSale", new List<ListItemSaleWebModel>().ToPagedList(1, 1));
            }
        }

        [UserAuthenticationFilter]
        public ActionResult AddItemSale()
        {
            ViewBag.listCategory = itemSaleBusiness.getListCategory();
            return View();
        }

        public JsonResult LoadNameItem(int? CategryId)
        {
            if (CategryId.HasValue)
            {
                var lstD = (from i in cnn.Items where i.GroupItemID == CategryId && i.IsActive == 1 orderby i.Name select new { i.ID, i.Name }).ToList();
                return Json(lstD, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [UserAuthenticationFilter]
        [ValidateInput(false)]
        public int CreateItemSale(int CategoryID, int itemId, int quantity, string Price, DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return itemSaleBusiness.CreateItemSale(CategoryID, itemId, quantity, Price, fromDateTime, toDateTime);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult LoadItemSale(int ID)
        {
            try
            {
                ViewBag.listCategory = itemSaleBusiness.getListCategoryEdit();
                var ItemSale = itemSaleBusiness.LoadItemSale(ID);
                return PartialView("EditItemSale", ItemSale);
            }
            catch
            {
                return PartialView("EditItemSale", new ItemSaleEditModel());
            }
        }

        [UserAuthenticationFilter]
        //[ValidateInput(false)]
        public int SaveEditItemSale(int ID, int CategoryID, int itemId, int quantity, string Price, DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                return itemSaleBusiness.SaveEditItemSale(ID, CategoryID, itemId, quantity, Price, fromDateTime, toDateTime);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public int DeleteItemSale(int ID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                if (UserLogins.Role != SystemParam.ROLE_ADMIN && UserLogins.Role != SystemParam.ROLE_EDITOR)
                {
                    Session[Sessions.LOGIN] = null;
                    return SystemParam.ERROR;
                }
                return itemSaleBusiness.DeleteItemSale(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}