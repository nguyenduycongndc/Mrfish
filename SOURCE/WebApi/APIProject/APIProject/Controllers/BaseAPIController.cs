using Data.Business;
using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class BaseAPIController : ApiController
    {
        protected MrFishEntities Context;
        public CategoryBusiness cateBus;
        public CouponBusiness couponBus;
        public VNPayBusiness vnPayBus;
        public ReceiveAddressBusiness receiveAddressBus;
        public MemberPointHistoryBusiness pointBus;
        public CartBusiness cartBus;
        public LoginBusiness lgBus;
        public NewsBusiness newsBus;
        public NotifyBusiness notiBus;
        public CustomerBusiness cusBus;
        public RequestAPIBusiness apiBus;
        public ItemBusiness itemBus;
        public OrderBusiness orderBus;
        public PackageBusiness packageBusiness;
        public NotifyBusiness notifyBusiness;
        public PushNotifyBusiness pushNotifyBus = new PushNotifyBusiness();




        public BaseAPIController() : base()
        {
            cateBus = new CategoryBusiness(this.GetContext());
            couponBus = new CouponBusiness(this.GetContext());
            vnPayBus = new VNPayBusiness(this.GetContext());
            receiveAddressBus = new ReceiveAddressBusiness(this.GetContext());
            pointBus = new MemberPointHistoryBusiness(this.GetContext());
            cartBus = new CartBusiness(this.GetContext());
            lgBus = new LoginBusiness(this.GetContext());
            newsBus = new NewsBusiness(this.GetContext());
            notiBus = new NotifyBusiness(this.GetContext());
            cusBus = new CustomerBusiness(this.GetContext());
            apiBus = new RequestAPIBusiness(this.GetContext());
            itemBus = new ItemBusiness(this.GetContext());
            orderBus = new OrderBusiness(this.GetContext());
            packageBusiness = new PackageBusiness(this.GetContext());
            notifyBusiness = new NotifyBusiness(this.GetContext());
            pushNotifyBus = new PushNotifyBusiness(this.GetContext());
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
    }
}