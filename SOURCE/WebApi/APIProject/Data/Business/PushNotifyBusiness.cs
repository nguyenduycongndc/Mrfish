using Data.DB;
using Data.Model;
using Data.Utils;
using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebSockets;
using WebSocketSharp;

namespace Data.Business
{
    public class PushNotifyBusiness : GenericBusiness
    {
        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int length = 6;
        public PushNotifyBusiness(MrFishEntities context = null) : base()
        {
        }
        public RequestAPIBusiness requestAPIBus = new RequestAPIBusiness();
        // push noti cho app
        public void PushNotifyAllCustomer(string Title, int Type, int? OrderId, int? NewsID, int? CouponID)
        {
            var listCustomer = cnn.Customers.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE)).ToList();
            List<string> listDevice = new List<string>();
            foreach (var item in listCustomer)
            {
                Notification notify = new Notification
                {
                    Title = Title,
                    Content = Title,
                    Type = Type,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                    CouponID = CouponID,
                    NewsID = NewsID,
                    OrderID = OrderId,
                    CustomerID = item.ID
                };
                cnn.Notifications.Add(notify);
                cnn.SaveChanges();
                if (item.DeviceID != null && item.DeviceID.Length > 10)
                {
                    listDevice.Add(item.DeviceID);
                }
            }
            NotifyDataModel notifyData = new NotifyDataModel();
            notifyData.id = OrderId.HasValue ? OrderId.Value : NewsID.HasValue ? NewsID.Value : CouponID.HasValue ? CouponID.Value : 0;
            notifyData.type = Type;
            notifyData.content = Title;


            string value = PushNotify(notifyData, listDevice, SystemParam.NOTI_MR_FISH, notifyData.content);
            PushOneSignal(value);
        }
        public void PushNotifyCustomer(string Title, int Type, Customer customer, int? OrderId, int? NewsID, int? CouponID)
        {
            try
            {
                List<string> listDevice = new List<string>();
                if (customer != null)
                {

                    Notification notify = new Notification
                    {
                        Title = Title,
                        Content = Title,
                        Type = Type,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        CouponID = CouponID,
                        NewsID = NewsID,
                        OrderID = OrderId,
                        CustomerID = customer.ID
                    };
                    cnn.Notifications.Add(notify);
                    cnn.SaveChanges();
                    if (customer.DeviceID != null && customer.DeviceID.Length > 10)
                    {
                        listDevice.Add(customer.DeviceID);
                    }

                    NotifyDataModel notifyData = new NotifyDataModel();
                    notifyData.id = OrderId.HasValue ? OrderId.Value : NewsID.HasValue ? NewsID.Value : CouponID.HasValue ? CouponID.Value : 0;
                    notifyData.type = Type;
                    notifyData.content = Title;
                    string value = PushNotify(notifyData, listDevice, SystemParam.NOTI_MR_FISH, notifyData.content);
                    PushOneSignal(value);
                }
            }catch(Exception ex)
            {
                ex.ToString();
            }
            

        }

        public string PushNotify(object obj, List<string> deviceID, string title, string contents)
        {
            // check 1 : app khachs hang --- check 2 : app tài xế
            var appid = SystemParam.APP_ID;
            var channelid = SystemParam.ANDROID_CHANNEL_ID;
            OneSignalInputs input = new OneSignalInputs();

            TextInput header = new TextInput();
            header.en = contents.Length > 0 ? title : SystemParam.NOTI_MR_FISH;
            TextInput content = new TextInput();
            content.en = contents.Length > 0 ? contents : title;
            input.app_id = appid;
            input.data = obj;
            input.headings = header;
            input.contents = content;
            input.android_channel_id = channelid;
            input.include_player_ids = deviceID;
            return JsonConvert.SerializeObject(input);
        }


        public string PushOneSignal(string value)
        {
            string url = SystemParam.URL_ONESIGNAL;

            var req = HttpWebRequest.Create(string.Format(url));
            var author = SystemParam.Authorization;
            req.Headers["Authorization"] = author;
            req.Headers["https"] = SystemParam.URL_BASE_https;
            var byteData = Encoding.UTF8.GetBytes(value);
            req.ContentType = "application/json";
            req.Method = "POST";
            try
            {
                using (var stream = req.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)req.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException e)
            {
                return e.ToString();
            }
        }
        public void SaveLog(string content, string body)
        {
            var reportDirectory = string.Format("~/text/{0}/", DateTime.Now.ToString("yyyy-MM-dd"));
            reportDirectory = System.Web.Hosting.HostingEnvironment.MapPath(reportDirectory);
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }
            var dailyReportFullPath = string.Format("{0}text_{1}.log", reportDirectory, DateTime.Now.Day);
            var logContent = string.Format("{0}-{1}-{2}", DateTime.Now, "noti: " + content + " / " + body, Environment.NewLine);
            File.AppendAllText(dailyReportFullPath, logContent);
        }
        
        public void testSocket(string content,int type =1 ,int? orderID = null ,int? customerID =null)
        {
            try
            {
                //using (var ws = new WebSocket("ws://localhost:3001/socket.io/?EIO=4&transport=websocket"))
                //{
                //    ws.OnMessage += (sender, e) =>
                //      Console.WriteLine("New message from controller: " + e.Data);

                //    ws.Connect();
                //    ws.Send("avc");
                //}
                if(type == 1)
                {
                    var data = SystemParam.URL_WEB_SOCKET + "?content=" + content + "&order_id=" + orderID + "&type="+ type;
                    requestAPIBus.GetJson(data);
                }
                else
                {
                    var data = SystemParam.URL_WEB_SOCKET + "?content=" + content + "&customer_id=" + customerID + "&type=" + type;
                    requestAPIBus.GetJson(data);
                }
                
                //Client socket = new Client(SystemParam.URL_WEB_SOCKET);
                //var data = new
                //{
                //    content = "test123"
                //};

                //socket.On("connect", (fn) =>
                //{

                //    string jsonData = JsonConvert.SerializeObject(data);
                //    socket.Emit("test", jsonData);
                //});

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
          

        }
    }
}
