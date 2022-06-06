using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class NotifyBusiness : GenericBusiness
    {
        public NotifyBusiness(MrFishEntities context = null) : base()
        {
        }

        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        public void CreateNoti(int cusID, int type, string title, string content, int objectID)
        {
            Notification noti = new Notification();
            noti.CustomerID = cusID;
            noti.Viewed = 0;
            noti.CreateDate = DateTime.Now;
            noti.IsActive = SystemParam.ACTIVE;
            noti.Type = type;
            noti.NewsID = objectID;
            noti.Title = title;
            noti.Content = content;
            cnn.Notifications.Add(noti);
            cnn.SaveChanges();
        }



        public void CreateNoties(int cusID, int type, int newsID, string content, string title, MrFishEntities cnns)
        {
            Notification noti = new Notification();
            noti.CustomerID = cusID;
            noti.Viewed = 0;
            noti.CreateDate = DateTime.Now;
            noti.IsActive = SystemParam.ACTIVE;
            noti.Type = type;
            if (type == SystemParam.NOTIFY_NAVIGATE_NEWS)
            {
                noti.NewsID = newsID;
            }
            noti.Title = title;
            noti.Content = content;
            cnns.Notifications.Add(noti);
        }
        public IPagedList<NotificationModel> GetListNotification(int cusID,int page,int limit)
        {
            try
            {
                var model = cnn.Notifications.Where(x => x.CustomerID.Value.Equals(cusID) && !x.IsAdmin.Value.Equals(1)).Select(x => new NotificationModel
                {
                    ID = x.ID,
                    Title = x.Title,
                    IsRead = x.Viewed,
                    Type = x.Type.HasValue ? x.Type.Value : 0,
                    CouponID = x.CouponID.HasValue ? x.CouponID.Value : 0,
                    OrderID = x.OrderID.HasValue ? x.OrderID.Value : 0,
                    NewsID = x.NewsID.HasValue ? x.NewsID.Value : 0,
                    CreateDate = x.CreateDate
                }).OrderBy(x => x.IsRead).ThenByDescending(x => x.ID).ToPagedList(page, limit);
                return model;
            }catch(Exception ex)
            {
                ex.ToString();
                return new List<NotificationModel>().ToPagedList(1, 1);
            }
        }
        public void ReadNotifiction(int ID)
        {
            try
            {
                var model = cnn.Notifications.FirstOrDefault(x => x.ID.Equals(ID));
                model.Viewed = 1;
                cnn.SaveChanges();
            }catch(Exception ex)
            {
                ex.ToString();
            }
           
        }
        public List<NotificationModel> GetListNotificationWeb()
        {
            try
            {
                var model = cnn.Notifications.Where(x => x.IsAdmin.Value.Equals(SystemParam.ROLE_ADMIN)).Select(x => new NotificationModel
                {
                    ID = x.ID,
                    Title = x.Title,
                    IsRead = x.Viewed,
                    Type = x.Type.HasValue ? x.Type.Value : 0,
                    OrderID = x.OrderID.HasValue ? x.OrderID.Value : 0,
                    CustomerID = x.CustomerID.HasValue ? x.CustomerID.Value : 0,
                    CreateDate = x.CreateDate
                }).OrderBy(x => x.IsRead).ThenByDescending(x => x.ID).ToList();
                return model;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<NotificationModel>();
            }
        }
        public int countNotiNotView()
        {
            var model = cnn.Notifications.Where(x => x.IsAdmin.Value.Equals(SystemParam.ROLE_ADMIN) && x.Viewed.Equals(SystemParam.NOTI_NOT_VIEWED)).Count();
            return model;
        }

    }
}
