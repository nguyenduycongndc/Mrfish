using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class BannerBusiness : GenericBusiness
    {
        public BannerBusiness(MrFishEntities context = null) : base()
        {

        }
        public string fullUrl = Util.getFullUrl();
        PushNotifyBusiness pushNotiBus = new PushNotifyBusiness();

        public JsonResultsModel CreateBanner(string title, int typeSend, int type, string content, string imageUrl, int icheck, int userId)
        {
            try
            {
                var coutBanner = cnn.News.Where(x => x.Title == title && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                if (coutBanner > 0) return rpBus.ErrorResult("Error", SystemParam.ERROR_CODE);
                var resBanner = new ListBannerModel();
                News banner = new News();
                banner.CreateUserID = userId;
                banner.CategoryID = 6;
                banner.Title = resBanner.Title = title;
                banner.Description = "";
                banner.CountLike = 0;
                banner.CountComment = 0;
                banner.TypeSend = resBanner.TypeSend = typeSend;
                banner.Type = resBanner.Type = type;
                banner.Icheck = resBanner.icheck = icheck;

                var index = imageUrl.IndexOf("files/");
                var imgIndex = imageUrl.Substring(index + 6);
                banner.UrlImage = resBanner.AvatarUrl = imgIndex;
                banner.Content = resBanner.Content = content;
                banner.CreateDate = DateTime.Now;
                banner.IsActive = resBanner.IsActive = SystemParam.ACTIVE;
                banner.Status = resBanner.Status = SystemParam.ACTIVE;
                cnn.News.Add(banner);
                cnn.SaveChanges();
                if (icheck == 1)
                {
                    pushNotiBus.PushNotifyAllCustomer(title, SystemParam.NOTIFICATION_TYPE_NEWS, null, banner.ID, null);
                    banner.TimePush = DateTime.Now;
                    cnn.SaveChanges();
                }
               
                //if (icheck == 1)
                //{
                //    pushNotiBus.PushNotifyAllCustomer(title, SystemParam.NOTIFICATION_TYPE_NEWS, null, banner.ID, null);
                //    banner.TimePush = DateTime.Now;
                //    cnn.SaveChanges();
                //}
                return rpBus.SuccessResult("Thành Công", resBanner);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }

        public ListBannerModel LoadBanner(int ID)
        {
            try
            {
                var obj = cnn.News.Find(ID);
                ListBannerModel Banner = new ListBannerModel();
                Banner.ID = obj.ID;
                Banner.IsActive = obj.IsActive;
                Banner.Title = obj.Title;
                Banner.Type = obj.Type;
                Banner.TypeSend = obj.TypeSend;
                Banner.Status = obj.Status;
                Banner.icheck = obj.Icheck;
                if (obj.UrlImage != null)
                {
                    Banner.AvatarUrl = fullUrl + obj.UrlImage;
                }
                Banner.Content = obj.Content;
                return Banner;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListBannerModel();
            }
        }


        //Lưu lại cập nhật sản phẩm
        public int SaveEditBannerData(int ID, string title, int typeSend, int type, string imageUrl, string content, int status, int icheck, int userId)
        {
            try
            {
                News banner = cnn.News.Find(ID);
                banner.Title = title;
                banner.TypeSend = typeSend;
                banner.CreateUserID = userId;
                banner.CategoryID = 6;
                banner.Description = "";
                banner.CountLike = 0;
                banner.CountComment = 0;
                banner.TypeSend = typeSend;
                banner.Type = type;
                if (imageUrl != null)
                {
                    var index = imageUrl.IndexOf("files/");
                    var imgSlice = imageUrl.Substring(index + 6);
                    banner.UrlImage = imgSlice;
                }
                banner.Content = content;
                
                if (icheck == 1 && !banner.TimePush.HasValue)
                {
                    pushNotiBus.PushNotifyAllCustomer(title, SystemParam.NOTIFICATION_TYPE_NEWS, null, banner.ID, null);
                    banner.TimePush = DateTime.Now;
                    cnn.SaveChanges();
                }
                banner.Status = status;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteBanner(int ID)
        {
            try
            {

                News _banner = cnn.News.Find(ID);

                _banner.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<ListBannerModel> SearchBanner(int Page, string title, DateTime? fromDate, DateTime? toDate, int? status, int? typeSend, int? type)
        {
            try
            {
                if (toDate.HasValue)
                    toDate = toDate.Value.AddDays(1);
                    //toDate = toDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<ListBannerModel> lstBannerOutput = new List<ListBannerModel>();
                var query = from n in cnn.News
                            where n.IsActive.Equals(SystemParam.ACTIVE)
                              && (fromDate.HasValue ? n.CreateDate >= fromDate.Value : true)
                              && (toDate.HasValue ? n.CreateDate <= toDate.Value : true)
                              && (toDate.HasValue ? n.CreateDate <= toDate.Value : true)
                              && (status != null ? n.Status == status : true)
                              && (typeSend != null ? n.TypeSend == typeSend : true)
                              && (type != null ? n.Type == type : true)
                            orderby n.ID descending
                            select new ListBannerModel
                            {
                                ID = n.ID,
                                Title = n.Title,
                                CreateDate = n.CreateDate,
                                IsActive = n.IsActive,
                                Status = n.Status,
                                TypeSend = n.TypeSend,
                                Type = n.Type,
                            };
                lstBannerOutput = query.ToList();

                if (!String.IsNullOrEmpty(title))
                    lstBannerOutput = lstBannerOutput.Where(u => Util.Converts(u.Title.ToLower()).Contains(Util.Converts(title.ToLower()))).ToList();

                return lstBannerOutput;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListBannerModel>();
            }
        }

    }
}
