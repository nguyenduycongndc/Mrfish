using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using log4net;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Business
{
    public class NewsBusiness : GenericBusiness
    {
        public NewsBusiness(MrFishEntities context = null) : base()
        {
        }

        public string fullUrl = Util.getFullUrl();
        public NewsAppOutputModel getNewsDetailApp(int newsID)
        {
            try
            {
                var news = (from n in cnn.News
                            where n.IsActive.Equals(SystemParam.ACTIVE) && n.ID.Equals(newsID) && n.Status.Equals(SystemParam.ACTIVE) && n.TypeSend.Equals(SystemParam.TYPESEND_BANNER)
                            orderby n.DisplayOrder
                            orderby n.ID
                            select new NewsAppOutputModel
                            {
                                ID = n.ID,
                                Content = n.Content,
                                Description = n.Description,
                                CreateDate = n.CreateDate,
                                Title = n.Title,
                                Type = n.Type,
                                UrlImage = fullUrl + n.UrlImage,
                            }).FirstOrDefault();
                return news;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return null;
            }
        }
        // danh sách bài viết 
        public List<NewsAppOutputModel> GetListNews(int type)
        {
            try
            {
                var ListNews = (from n in cnn.News
                                where n.Type.Equals(type) && n.IsActive.Equals(SystemParam.ACTIVE) && n.Status.Equals(SystemParam.ACTIVE) && n.TypeSend.Equals(SystemParam.TYPESEND_BANNER)
                                orderby n.ID descending
                                select new NewsAppOutputModel
                                {
                                    ID = n.ID,
                                    Type = n.Type,
                                    Title = n.Title,
                                    Content = n.Content,
                                    Description = n.Description,
                                    CreateDate = n.CreateDate,
                                    UrlImage = fullUrl + n.UrlImage,
                                }).ToList();

                return ListNews;
            }
            catch (Exception e)
            {
                e.ToString();
                return new List<NewsAppOutputModel>();
            }
        }
        // danh sách banner
        public List<BannerModel> GetListBanner()
        {
            try
            {
                var ListNews = (from n in cnn.News
                                where n.Type.Equals(SystemParam.NEWS_TYPE_BANNER) && n.IsActive.Equals(SystemParam.ACTIVE) && n.Status.Equals(SystemParam.ACTIVE) && n.TypeSend.Equals(SystemParam.TYPESEND_BANNER)
                                orderby n.ID descending
                                select new BannerModel
                                {
                                    ID = n.ID,
                                    ImageUrl = fullUrl + n.UrlImage,
                                }).ToList();
                return ListNews;
            }
            catch (Exception e)
            {
                e.ToString();
                return new List<BannerModel>();
            }
        }



        // tạo bài viết tin tức
        public int CreateNewsDekko(string Description, string Content, string Title, int Type, int TypeSend, string UrlImage, int Status, int SendNotify, int CreateUserID, string timePush, int? itemID)
        {
            try
            {
                DateTime? timeSend = Util.ConvertDate(timePush, SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                if (timeSend.HasValue && timeSend < DateTime.Now)
                    return -1;
                News news = new News();
                news.CreateUserID = CreateUserID;
                news.CategoryID = GetCategoryByName(Type).ID;
                news.Title = Title;
                news.Description = Description;
                news.Content = Content;
                news.Type = Type;
                /*news.TypeSend = SystemParam.TYPE_SEND_ALL;*/
                /* news.TypeSend = GetTypeSend(TypeSend).ID;*/
                news.TypeSend = TypeSend;
                news.Status = !timeSend.HasValue ? Status : 0;
                news.UrlImage = UrlImage;
                news.CreateDate = DateTime.Now;
                news.IsActive = SystemParam.ACTIVE;
                news.TimePush = timeSend;
                news.ItemID = itemID;
                cnn.News.Add(news);
                cnn.SaveChanges();
                if (news.Status == SystemParam.STATUS_NEWS_ACTIVE && Type != 6 && TypeSend != SystemParam.TYPE_NO_SEND && !timeSend.HasValue)
                {
                    int newsID = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.ID).FirstOrDefault().ID;
                    sendNotifyNews(Title, Description, newsID, TypeSend);
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        NotifyBusiness notifyBusiness = new NotifyBusiness();
        PackageBusiness packageBusiness = new PackageBusiness();

        public void sendNotifyNews(string title, string Description, int newsID, int TypeSend)
        {
            var news = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(newsID)).FirstOrDefault();
            NotifyDataModel notifyData = new NotifyDataModel();
            notifyData.id = newsID;
            notifyData.type = SystemParam.NOTIFY_NAVIGATE_NEWS;
            List<Customer> listCustomer;
            List<List<Customer>> listCustomers = new List<List<Customer>>();
            if (TypeSend == SystemParam.TYPE_SEND_ALL)
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && (u.Role.Equals(SystemParam.ROLE_CUSTOMER_LOYAL) || u.Role.Equals(SystemParam.ROLE_CUSTOMER))).OrderBy(u => u.ID).ToList();
            }
            else if (TypeSend == SystemParam.TYPE_SEND_CUSTOMER_LOYAL)
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Role.Equals(SystemParam.ROLE_CUSTOMER_LOYAL)).OrderBy(u => u.ID).ToList();
            }
            else
            {
                listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderBy(u => u.ID).ToList();
            }

            //Tạo thông báo cho khách hàng
            if (listCustomer != null && listCustomer.Count() > 0)
            {
                List<Notification> lstnoti = listCustomer.Select(n => new Notification
                {
                    CustomerID = n.ID,
                    Viewed = 0,
                    CreateDate = DateTime.Now,
                    IsActive = SystemParam.ACTIVE,
                    Type = SystemParam.NOTIFY_NAVIGATE_NEWS,
                    NewsID = newsID,
                    Content = Description,
                    Title = title

                }).ToList();
                cnn.Notifications.AddRange(lstnoti);
                cnn.SaveChanges();
                //Gửi thông báo đến app
                List<string> lstDeviceID = listCustomer.Where(c => !String.IsNullOrEmpty(c.DeviceID) && c.DeviceID.Length > 10).Select(c => c.DeviceID).ToList();

               
            }
        }
        public News GetTypeSend(int TypeSend)
        {
            switch (TypeSend)
            {
                case SystemParam.TYPE_SEND_ALL:
                    TypeSend = SystemParam.TYPE_SEND_ALL;

                    break;
                case SystemParam.TYPE_SEND_CUSTOMER:
                    TypeSend = SystemParam.TYPE_SEND_CUSTOMER;
                    break;
                case SystemParam.TYPE_SEND_AGENCY:
                    TypeSend = SystemParam.TYPE_SEND_AGENCY;
                    break;
            }
            var typeSend = cnn.News.Where(u => u.TypeSend == TypeSend).FirstOrDefault();
            return typeSend;
        }


        public CategoryNew GetCategoryByName(int Type)
        {
            try
            {
                string categoryName = "";
                switch (Type)
                {
                    case SystemParam.NEWS_TYPE_BANNER:
                        categoryName = "Banner";
                        break;
                    default:
                        break;
                }
                var categoryNews = cnn.CategoryNews.Where(u => u.Name.Equals(categoryName)).FirstOrDefault();
                return categoryNews;
            }
            catch
            {
                return new CategoryNew();
            }
        }

        //chỉnh sửa bài viết
        public int UpdateNewsDekko(int ID, int Status, string Content, string Title, int Type, string UrlImage, string Description, int TypeSend, string timePush, int? itemID)
        {
            try
            {
                DateTime? timeSend = Util.ConvertDate(timePush, SystemParam.CONVERT_DATETIME_HAVE_HOUR);
                if (timeSend.HasValue && timeSend < DateTime.Now && !Status.Equals(SystemParam.ACTIVE))
                    return -1;
                News news = cnn.News.Find(ID);
                var statusold = news.Status;
                news.CategoryID = GetCategoryByName(Type).ID;
                news.Title = Title;
                news.Description = Description;
                news.Content = Content;
                news.Type = Type;
                news.DisplayOrder = 1;
                news.Status = !timeSend.HasValue || news.Status.Equals(SystemParam.ACTIVE) ? Status : 0;
                news.UrlImage = UrlImage;
                news.TypeSend = TypeSend;
                news.TimePush = timeSend;
                news.ItemID = itemID;
                cnn.SaveChanges();
                if (news.Status == SystemParam.STATUS_NEWS_ACTIVE && statusold != news.Status && Type != 6 && TypeSend != SystemParam.TYPE_NO_SEND && !timeSend.HasValue)
                {
                    int newsID = cnn.News.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.ID).FirstOrDefault().ID;
                    sendNotifyNews(Title, Description, newsID, TypeSend);
                }
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        //xóa bài viết
        public int DeleteNews(int ID)
        {
            try
            {
                News news = cnn.News.Find(ID);
                news.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        // tìm kiếm
        public IPagedList<ListNewsWebOutputModel> Search(int Page, string Title, int? CreateUserID, int? Type, int? Status, string FromDate, string ToDate)
        {
            try
            {
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1);

                var data = (from n in cnn.News
                            where n.IsActive.Equals(SystemParam.ACTIVE)
                            && (!String.IsNullOrEmpty(Title) ? n.Title.Contains(Title) : true)
                            && (CreateUserID.HasValue ? n.CreateUserID == CreateUserID.Value : true)
                            && (Type.HasValue ? n.Type == Type.Value : true)
                            && (Status.HasValue ? n.Status == Status.Value : true)
                            && (startdate.HasValue ? n.CreateDate >= startdate.Value : true)
                            && (endDate.HasValue ? n.CreateDate <= endDate.Value : true)
                            orderby n.ID descending
                            select new ListNewsWebOutputModel
                            {
                                ID = n.ID,
                                CategoryName = n.CategoryNew.Name,
                                Title = n.Title,
                                Status = n.Status,
                                Type = n.Type,
                                CreateUserID = n.User.UserID,
                                CreateUserName = n.User.UserName,
                                CreateDate = n.CreateDate,
                                CountLike = n.CountLike,
                                CountComment = n.CountComment,
                                timePush = n.TimePush
                            }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch
            {
                return new List<ListNewsWebOutputModel>().ToPagedList(1, 1);
            }
        }


        //lấy danh sách tác giả
        public List<ListUserWebOutputModel> GetListAuthor()
        {
            try
            {
                var ListUser = from news in cnn.News
                               join user in cnn.Users on news.CreateUserID equals user.UserID
                               where news.IsActive == SystemParam.ACTIVE
                               select new ListUserWebOutputModel()
                               {
                                   ID = user.UserID,
                                   Name = user.UserName,
                                   CreateDate = user.CreateDate,
                                   Status = news.Status
                               };
                if (ListUser != null && ListUser.Count() > 0)
                {
                    ListUser = ListUser.Distinct();
                    return ListUser.ToList();
                }
                else
                {
                    return new List<ListUserWebOutputModel>();
                }
            }
            catch
            {
                return new List<ListUserWebOutputModel>();
            }
        }

        //lấy thông tin chi tiết 1 bài viết
        public ListNewsWebOutputModel GetNewsDetail(int ID)
        {
            try
            {
                ListNewsWebOutputModel newsDetail = new ListNewsWebOutputModel();
                var query = (from n in cnn.News
                             join u in cnn.Users on n.CreateUserID equals u.UserID
                             where n.IsActive.Equals(SystemParam.ACTIVE) && n.ID.Equals(ID)
                             select new ListNewsWebOutputModel
                             {
                                 ID = n.ID,
                                 Type = n.Type,
                                 CategoryID = n.CategoryID,
                                 TypeSend = n.TypeSend,
                                 Status = n.Status,
                                 Title = n.Title,
                                 Depcription = n.Description,
                                 Content = n.Content,
                                 UrlImage = n.UrlImage,
                                 CreateUserID = n.CreateUserID,
                                 CreateUserName = u.UserName,
                                 timePush = n.TimePush,
                                 itemID = n.ItemID
                             }).FirstOrDefault();
                if (query != null && query.ID > 0)
                {
                    return newsDetail = query;
                }
                return newsDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListNewsWebOutputModel();
            }
        }


        //Lấy danh sách sản phẩm
        public List<ListItemOutputModel> ListItem()
        {
            return cnn.Items.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE))
                .Select(i => new ListItemOutputModel
                {
                    ID = i.ID,
                    Name = i.Name
                }).ToList();
        }

        public void CheckTimePushNews()
        {
            try
            {
                DateTime fromTime = DateTime.Now.AddMinutes(-1);
                DateTime toTime = fromTime.AddMinutes(1);
                List<News> lstNews = cnn.News.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && n.TimePush.HasValue && n.TimePush.Value >= fromTime && n.TimePush.Value <= toTime).ToList();
                if (lstNews != null && lstNews.Count() > 0)
                    foreach (var dt in lstNews)
                    {
                        if (!dt.TypeSend.Equals(SystemParam.TYPE_NO_SEND))
                            sendNotifyNews(dt.Title, dt.Description, dt.ID, dt.TypeSend);
                        dt.Status = SystemParam.ACTIVE;
                    }
                cnn.SaveChanges();
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(NewsBusiness));
                log.Error("App_Error_" + DateTime.Now.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR), ex);
            }

        }

    }
}
