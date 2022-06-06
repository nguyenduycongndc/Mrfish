using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class LoginBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        public LoginBusiness(MrFishEntities context = null) : base()
        {

        }
        //PointBusiness pBus = new PointBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        CouponBusiness couponBus = new CouponBusiness();
        /// <summary>
        /// đăng nhập vào app
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string CheckLoginApp(string value,/* int typeLogin,*/ string name, string url, string email, string deviceID)
        {
            var lscus = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            
            // đăng nhập bămg sdt
            //if (typeLogin == SystemParam.TYPE_LOGIN_PHONE)
            //{
                var cus = lscus.Where(c => c.Phone == value).FirstOrDefault();
                if (Util.CheckPass(name, cus.PasswordReset))
                {
                    var datetime2 = cus.ExpPasswordReset.Value.ToString();
                    if (datetime2 != null)
                    {
                        DateTime dt = DateTime.Parse(datetime2);
                        int res = DateTime.Compare(DateTime.Now, dt);
                        if (res < 0)
                        {
                            string token = Util.GenerateJwt(cus);
                            cus.Token = token;
                            cus.DeviceID = deviceID;
                            cnn.SaveChanges();
                            return token;
                        }
                    }                    
                
                }

                if (Util.CheckPass(name,cus.Password))
                {
                    string token = Util.GenerateJwt(cus);
                    //string token = Util.CreateMD5(DateTime.Now.ToString());
                    cus.Token = token;
                //cus.Type = typeLogin;
                cus.DeviceID = deviceID;
                cnn.SaveChanges();
                    return token;
                }                
                 return SystemParam.MESSAGE_LOGIN_ACCOUNT_FAIL;
        }

        public UserInforOutputModel CheckOTP(int otp)
        {
            UserInforOutputModel query = new UserInforOutputModel();
            var cus = cnn.Customers.Where(u => u.IsActive == SystemParam.ACTIVE && (u.OTP == otp)).FirstOrDefault();
            if (cus == null) { return query; }
            query.Token = cus.Token;
            query.UserID = cus.ID;
            query.Address = cus.Address;
            query.DOB = cus.DOB;
            query.DOBStr = cus.DOB.ToString(SystemParam.CONVERT_DATETIME);
            query.Email = cus.Email;
            query.Code = cus.Code;
            query.TypeLogin = cus.Type;
            query.CustomerName = cus.Name;
            query.Point = cus.Point;
            query.Phone = cus.Phone;
            query.Sex = cus.Sex;
            query.UrlAvatar = fullUrl + cus.AvatarUrl;
            query.Role = cus.Role;
            query.OTP = (int)cus.OTP;
            query.DeviceID = cus.DeviceID;
            query.RankName = cus.RankID.HasValue ? cus.Ranking.RankName : "";
            query.CouponCount = couponBus.GetCouponCount(cus.ID);
            query.RankLevel = cus.RankID.HasValue ? cus.Ranking.Level : 0;
            query.DiscountRank = cus.RankID.HasValue ? cus.Ranking.DiscountPercent : 0;
            return query;
        }

        public UserInforOutputModel GetuserInfor(int cusID)
        {
            UserInforOutputModel query = new UserInforOutputModel();
            Customer cus = cnn.Customers.Find(cusID);
            //query.Code = Util.CheckNullString(cus.Code);
            Province pr = cnn.Provinces.Find(cus.ProvinceCode);
            District dt = cnn.Districts.Find(cus.DistrictCode);
            Ward wd = cnn.Wards.Find(cus.WardID);
            if (pr != null)
            {
                query.ProvinceID = cus.ProvinceCode.Value;
                query.ProvinceName = pr.Name;
            }
            else
            {
                query.ProvinceName = "Chưa cập nhập";
            }
            if (dt != null)
            {
                query.DistrictID = cus.DistrictCode.Value;
                query.DistrictName = dt.Name;
            }
            else
            {
                query.DistrictName = "Chưa cập nhập";
            }
            if (wd != null)
            {
                query.WardID = cus.WardID.Value;
                query.WardName = wd.Name;
            }
            else
            {
                query.WardName = "Chưa cập nhập";
            }
            query.UserID = cusID;
            query.Address = cus.Address;
            query.DOB = cus.DOB;
            query.DOBStr = cus.DOB.ToString(SystemParam.CONVERT_DATETIME);
            query.Email = cus.Email;
            query.Code = cus.Code;
            query.TypeLogin = cus.Type;
            query.CustomerName = cus.Name;
            query.RankName = cus.RankID.HasValue ? cus.Ranking.RankName : "";
            query.CouponCount = couponBus.GetCouponCount(cusID);
            query.RankLevel = cus.RankID.HasValue ? cus.Ranking.Level : 0;
            query.DiscountRank = cus.RankID.HasValue ? cus.Ranking.DiscountPercent : 0;
            query.Point = cus.Point;
            query.Phone = cus.Phone;
            query.Sex = cus.Sex;
            query.UrlAvatar = fullUrl +  cus.AvatarUrl;
            query.Token = cus.Token;
            query.Role = cus.Role;
            query.DeviceID = cus.DeviceID;
            
            return query;
        }



        /// <summary>
        /// Tạo Khách hàng mới
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public void CreateCustomer(string value, string name, int type, string token, string url, string email)
        {
            try
            {
                Customer cus = new Customer();

                cus.Code = value;
                cus.Phone = "";
                cus.Token = token;
                cus.IsActive = SystemParam.ACTIVE;
                cus.Address = "";
                cus.DOB = DateTime.Now;

                if (type == SystemParam.TYPE_LOGIN_GOOGLE)
                {
                    cus.Name = name.Split('@').FirstOrDefault();
                }
                else
                {
                    cus.Name = name;
                }
                cus.Email = email;
                //cus.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
                //cus.DistrictCode = SystemParam.DISTRICT_DEFAULT;
                //cus.WardID = SystemParam.Ward_DEFAULT;           
                cus.Sex = SystemParam.GENDER_MEN;
                cus.Role = SystemParam.ROLE_CUSTOMER;
                if (url.Length == 0)
                {
                    cus.AvatarUrl = "https://st.quantrimang.com/photos/image/072015/22/avatar.jpg";
                }

                cus.AvatarUrl = url;
                cus.LastLoginDate = DateTime.Now;
                cus.ExpireTocken = DateTime.Now.AddYears(1);
                //cus.Point = SystemParam.POINT_START;
                cus.Point = 0;
                cus.DeviceID = "";
                cus.Type = type;
                cus.Status = SystemParam.ACTIVE;
                cus.CraeteDate = DateTime.Now;
                cnn.Customers.Add(cus);
                cnn.SaveChanges();
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
            }
        }
        /// <summary>
        /// Tạo Khách hàng mới
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int UpdateUser(UserInforOutputModel item, int CusID)
        {
            try
            {
                if(item.Email!= null && item.Email.Length > 0)
                {
                    int res = checkEmail(CusID, item.Email);
                    if (res != SystemParam.SUCCESS)
                        return res;
                }
                else
                {
                    item.Email = "";
                }
                if(item.Phone != null && item.Phone.Length > 0)
                {
                    int checkphone = checkExistPhoneEmail(CusID, item.Phone, "");
                    if (checkphone != SystemParam.SUCCESS)
                        return checkphone;
                }
                else
                {
                    item.Phone = "";
                }

                Customer cus = cnn.Customers.Find(CusID);
                cus.Name = item.CustomerName;
                cus.Address = item.Address;
                try
                {
                    cus.DOB = DateTime.ParseExact(item.DOBStr, SystemParam.CONVERT_DATETIME, null);
                }
                catch { }
                cus.Email = item.Email;
                cus.Phone = item.Phone;
                cus.Sex = item.Sex;
                cus.LastLoginDate = DateTime.Now;
                cus.ExpireTocken = DateTime.Now.AddYears(1);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }
        public int checkEmail(int cusID ,string email)
        {
            var customerUpdate = cnn.Customers.Find(cusID);
            var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            if (email.Length > 0 && customerUpdate.Email != email)
            {
                var customerEmail = customer.Where(u => u.Email.Equals(email));
                if (customerEmail != null && customerEmail.Count() > 0)
                {
                    if (customerEmail.FirstOrDefault().ID != cusID)
                    {
                        return SystemParam.ERROR_EXIST_EMAIL;
                    }
                }
            }
            return SystemParam.SUCCESS;
        }


        public int checkExistPhoneEmail(int cusID, string phone, string email)
        {
            var customerUpdate = cnn.Customers.Find(cusID);
            var customer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE));
            var customerPhone = customer.Where(u => u.Phone.Equals(phone));
            if (customerPhone != null && customerPhone.Count() > 0)
            {
                if (customerPhone.FirstOrDefault().ID != cusID)
                {
                    return SystemParam.ERROR_EXIST_PHONE;
                }
            }
            if (email.Length > 0 && customerUpdate.Email != email)
            {
                var customerEmail = customer.Where(u => u.Email.Equals(email));
                if (customerEmail != null && customerEmail.Count() > 0)
                {
                    if (customerEmail.FirstOrDefault().ID != cusID)
                    {
                        return SystemParam.ERROR_EXIST_EMAIL;
                    }
                }
            }
            return SystemParam.SUCCESS;
        }



        public UserDetailOutputModel CheckLoginWeb(string phone, string password)
        {
            UserDetailOutputModel query = new UserDetailOutputModel();
            var checkStatus = cnn.Customers.Where(c => c.IsActive == SystemParam.ACTIVE && (c.Phone == phone || c.Name == phone) && c.Status == 0).FirstOrDefault();
            if (checkStatus != null)
                return query;
            var passUser = cnn.Users.Where(u => u.IsActive == SystemParam.ACTIVE && (u.Phone == phone || u.UserName == phone)).FirstOrDefault();
            if (passUser == null)
                return query;
            if (Util.CheckPass(password, passUser.PassWord))
            {
                query = cnn.Users.Where(u => u.IsActive == SystemParam.ACTIVE && (u.Phone == phone)).Select(u => new UserDetailOutputModel { UserID = u.UserID, UserName = u.UserName, Role = u.Role, Phone = u.Phone }).FirstOrDefault();
            }
            else
            {
                query = null;
            }
            return query;
        }

        public CustomerDetailOutputModel CusDetail(int cusID)
        {
            Customer cus = cnn.Customers.Find(cusID);
            CustomerDetailOutputModel query = new CustomerDetailOutputModel();
            //query.Code = Util.CheckNullString(cus.Code);
            query.DistrictName = cus.District.Name;
            query.ProvinceName = cus.Province.Name;
            query.ProvinceID = cus.ProvinceCode.Value;
            query.DistrictID = cus.DistrictCode.Value;
            query.Address = cus.Address;
            query.DOB = cus.DOB;
            query.DOBStr = cus.DOB.ToString(SystemParam.CONVERT_DATETIME);
            query.Email = cus.Email;
            query.TypeLogin = cus.Type;
            query.CustomerName = cus.Name;
            query.Phone = cus.Phone;
            query.Sex = cus.Sex;
            //query.ConfirmCode = Util.CheckNullString(cus.ConfirmCode);
            //query.Point = cus.Point;
            query.UrlAvatar = cus.AvatarUrl;
            return query;
        }
        

        //public int RemainPoint(Customer cus)
        //{
        //    int totalPointPerDay = pBus.getTotalPointPerDay(cus.ID);
        //    int maxpoint = maxPointPerDay();
        //    int minpoint = minPointAccount();
        //    return cus.Point >= minpoint ? (cus.Point > minpoint + maxpoint ? maxpoint - totalPointPerDay : cus.Point - minpoint) : 0;
        //    //return cus.Point >= minpoint ? (cus.Point > minpoint + maxpoint ? maxpoint - totalPointPerDay : cus.Point - totalPointPerDay - minpoint) : 0;
        //}



    }
}
