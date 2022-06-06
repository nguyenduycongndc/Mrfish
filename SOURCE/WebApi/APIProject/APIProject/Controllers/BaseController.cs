using APIProject.App_Start;
using Data.Business;
using Data.DB;
using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    [AllowCrossSiteJson]
    public class BaseController:Controller
    {
        protected MrFishEntities Context;
        public LoginBusiness loginBusiness;
        public NewsBusiness newsBusiness;
        public CustomerBusiness cusBusiness;
        public ConfigBusiness configBusiness;
        public VNPayBusiness vnPayBus;
        public RankBusiness rankBusiness;
        public OrderBusiness orderBus;
        public UserBusiness userBusiness;
        public ItemBusiness itemBusiness;
        public BannerBusiness bannerBusiness;
        public VoucherCustomerBusiness voucherCustomerBusiness;
        public VoucherIntroduceBusiness voucherIntroduceBusiness;
        public ItemBusiness productsBusiness;
        public NotifyBusiness notifyBusiness;
        public CategoryBusiness categoryBusiness;
        public ItemSaleBusiness itemSaleBusiness;
        public BaseController() : base()
        {
            loginBusiness = new LoginBusiness(this.GetContext());
            newsBusiness = new NewsBusiness(this.GetContext());
            vnPayBus = new VNPayBusiness(this.GetContext());
            cusBusiness = new CustomerBusiness(this.GetContext());
            configBusiness = new ConfigBusiness(this.GetContext());
            rankBusiness = new RankBusiness(this.GetContext());
            orderBus = new OrderBusiness(this.GetContext());
            userBusiness = new UserBusiness(this.GetContext());
            itemBusiness = new ItemBusiness(this.GetContext());
            bannerBusiness = new BannerBusiness(this.GetContext());
            voucherCustomerBusiness = new VoucherCustomerBusiness(this.GetContext());
            voucherIntroduceBusiness = new VoucherIntroduceBusiness(this.GetContext());
            productsBusiness = new ItemBusiness(this.GetContext());
            notifyBusiness = new NotifyBusiness(this.GetContext());
            categoryBusiness = new CategoryBusiness(this.GetContext());
            itemSaleBusiness = new ItemSaleBusiness(this.GetContext());
        }


        /// <summary>
        /// Create new context if null
        /// </summary>
        public MrFishEntities GetContext()
        {
            if (Context == null)
            {
                Context = new MrFishEntities();
            }
            return Context;
        }
       
        public UserDetailOutputModel UserLogins
        {
            get
            {
                return Session[Data.Utils.Sessions.LOGIN] as UserDetailOutputModel; 
            }
        }



    }
}