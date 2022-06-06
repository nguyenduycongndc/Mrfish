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
    public class NewsController : BaseController
    {
        // GET: News


        [UserAuthenticationFilter]
        public ActionResult CreateNews()
        {
            ViewBag.lstItem = newsBusiness.ListItem();
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult UpdateNews(int id)
        {
            ViewBag.lstItem = newsBusiness.ListItem();
            return View(newsBusiness.GetNewsDetail(id));
        }


        [ValidateInput(false)]
        [HttpPost]
        [UserAuthenticationFilter]
        public int CreateNewsDekko(string Description, string Content, string Title, int Type, int TypeSend, string UrlImage, int Status, int SendNotify, string timePush, int? itemID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return newsBusiness.CreateNewsDekko(Description, Content, Title, Type, TypeSend, UrlImage, Status, SendNotify, userLogin.UserID, timePush, itemID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        [UserAuthenticationFilter]
        public int UpdateNewsDekko(int ID, int Status, string Content, string Title, int Type, string UrlImage, string Description, int TypeSend, string timePush, int? itemID)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;
                return newsBusiness.UpdateNewsDekko(ID, Status, Content, Title, Type, UrlImage, Description, TypeSend,timePush, itemID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string Title, int? CreateUserID, int? Type, int? Status, string FromDate, string ToDate)
        {
            try
            {
                ViewBag.PageCurrentNews = Page;
                return PartialView("_TableNews", newsBusiness.Search(Page, Title, CreateUserID, Type, Status, FromDate, ToDate));
            }
            catch
            {
                return PartialView("_TableNews", new List<ListNewsWebOutputModel>().ToPagedList(1, 1));
            }
        }


        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            List<ListUserWebOutputModel> listAuthor = newsBusiness.GetListAuthor();
            ViewBag.ListAuthor = listAuthor;
            return View();
        }


        [UserAuthenticationFilter]
        public int DeleteNews(int ID)
        {
            try
            {
                return newsBusiness.DeleteNews(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult GetNewsDetail(int ID)
        {
            try
            {
                ListNewsWebOutputModel newsDetail = newsBusiness.GetNewsDetail(ID);
                return PartialView("_UpdateNews", newsDetail);
            }
            catch
            {
                return PartialView("_UpdateNews", new ListNewsWebOutputModel());
            }
        }
    }
}