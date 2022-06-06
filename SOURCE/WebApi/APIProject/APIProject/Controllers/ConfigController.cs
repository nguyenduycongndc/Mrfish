using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;

namespace APIProject.Controllers
{
    public class ConfigController : BaseController
    {
        // GET: Config
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            DetailConfigModel data = configBusiness.BindingConfig();
            return View(data);
        }
        [UserAuthenticationFilter]
        public PartialViewResult listRank(int Page)
        {
            IPagedList<RankNewModel> list = configBusiness.ListRankNew().ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_listRank", list);
        }
        [UserAuthenticationFilter]
        public PartialViewResult ListContactInfoTable()
        {
            ListContactInfoNewModel list = configBusiness.listContactInfoTable();
            return PartialView("_listContactInfo", list);
        }
        [UserAuthenticationFilter]
        public int SaveEditConfig(double valuePoint/*, string descriptionPoint, string contactInfo, string descriptionContact*/)
        {
            try
            {
                return configBusiness.saveEditConfig(valuePoint/*, descriptionPoint, contactInfo, descriptionContact*/);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        [UserAuthenticationFilter]
        public int SaveEditRank(int ID, string MaxPoint, string DiscountPercent, string Descriptions)
        {
            try
            {
                return configBusiness.saveEditRank(ID, MaxPoint, DiscountPercent, Descriptions);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        [UserAuthenticationFilter]
        public int SaveEditContactInfo(int key, string TextContactInfoZalo, string TextDescriptionContactInfoZalo, string TextContactInfoMess, string TextDescriptionContactInfoMess, string TextContactInfoHotline, string TextDescriptionContactInfoHotline)
        {
            try
            {
                return configBusiness.saveEditContactInfo(key, TextContactInfoZalo, TextDescriptionContactInfoZalo, TextContactInfoMess, TextDescriptionContactInfoMess, TextContactInfoHotline, TextDescriptionContactInfoHotline);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        [UserAuthenticationFilter]
        public PartialViewResult ModalEditRank(int ID)
        {
            try
            {
                RankEditNewModel DetailRank = configBusiness.ModalEditRank(ID);
                return PartialView("EditRank", DetailRank);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("EditRank", new RankEditNewModel());
            }
        }

        [UserAuthenticationFilter]
        public PartialViewResult LoadContactInfo(int keycheck)
        {
            try
            {
                ListContactInfoNewModel DetailContactInfo = configBusiness.loadContactInfo(keycheck);
                return PartialView("EditContactInfo", DetailContactInfo);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("EditContactInfo", new ListContactInfoNewModel());
            }
        }

    }
}