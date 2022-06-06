using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpRaven;
using SharpRaven.Data;

namespace Data.Business
{
    public class UserBusiness : GenericBusiness
    {
        public UserBusiness(MrFishEntities context = null) : base()
        {

        }
        public IPagedList<UserDetailOutputModel> Search(int Page, string Phone, string FromDate, string ToDate, int? Status, int? Role)
        {
            try
            {
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                if (endDate.HasValue)
                    //endDate = endDate.Value.AddDays(1);
                    endDate = endDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                var query = (from u in cnn.Users
                            join c in cnn.Customers on u.UserID equals c.UserID
                            where u.IsActive.Equals(SystemParam.ACTIVE)
                            && (/*u.Role.Equals(SystemParam.ROLE_ADMIN)||*/ u.Role.Equals(SystemParam.ROLE_EDITOR) || u.Role.Equals(SystemParam.ROLE_ACCOUNTANT))
                            && c.IsActive.Equals(SystemParam.ACTIVE)
                            && (c.Role.Equals(SystemParam.ROLE_ADMIN) || c.Role.Equals(SystemParam.ROLE_EDITOR) || c.Role.Equals(SystemParam.ROLE_ACCOUNTANT))
                             && ((!String.IsNullOrEmpty(Phone) ? u.UserName.Contains(Phone) : true)
                            || (!String.IsNullOrEmpty(Phone) ? u.Phone.Contains(Phone) : true))
                            && (startdate.HasValue ? u.CreateDate >= startdate.Value : true)
                            && (endDate.HasValue ? u.CreateDate <= endDate.Value : true)
                            && (Status.HasValue ? c.Status.Equals(Status.Value) : true)
                            && (Role.HasValue ? c.Role.Equals(Role.Value) : true)
                             orderby u.UserID descending
                            select new UserDetailOutputModel
                            {
                                UserID = u.UserID,
                                Role = u.Role,
                                UserName = u.UserName,
                                Phone = u.Phone,
                                CreateDate = u.CreateDate,
                                Status = c.Status,
                            }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query;
            }
            catch
            {
                return new List<UserDetailOutputModel>().ToPagedList(1, 1);
            }
        }

        public int ChangePassword(int ID, string CurrentPassword, string NewPassword)
        {
            try
            {
                //Lấy tài khoản muốn thay đổi mât khẩu
                var use = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.UserID.Equals(ID)).FirstOrDefault();
                //Kiểm tra mật khẩu cũ
                if (Util.CheckPass(CurrentPassword, use.PassWord))
                {
                    User user = cnn.Users.Find(ID);
                    user.PassWord = Util.GenPass(NewPassword);
                    cnn.SaveChanges();
                    return SystemParam.SUCCESS;
                }

                return SystemParam.WRONG_PASSWORD;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }

        public Boolean CheckDuplicatePhone(string Phone)
        {
            var check = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Phone == Phone).FirstOrDefault();
            if (check != null)
                return false;
            return true;
        }
        public JsonResultsModel CreateUser(string UserName, string Phone, string DOB, string Address,
            int Sex, string Email, int Role, /*int Status,*/
            string password, string AvatarUrl, string Note)
        {
            try
            {
                if (CheckDuplicatePhone(Phone) == false)
                {
                    return rpBus.ErrorResult("", SystemParam.PROCESS_ERROR);
                }

                var currentUser = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Phone.Equals(Phone));
                if (currentUser != null && currentUser.Count() > 0)
                {
                    return rpBus.ErrorResult("Lỗi dữ liệu đầu vào", SystemParam.PROCESS_ERROR);
                }

                var resCus = new CustomerModel();

                User user = new User();
                Customer c = new Customer();
                user.Phone = Phone;
                user.PassWord = Util.GenPass(password);
                user.UserName = UserName;
                user.Role = Role;
                user.IsActive = SystemParam.ACTIVE;
                user.CreateDate = DateTime.Now;
                cnn.Users.Add(user);
                cnn.SaveChanges();
                c.Code = resCus.Code = "";
                c.Phone = resCus.Phone = Phone;
                c.Token = resCus.Token = "";
                c.IsActive = resCus.IsActive = SystemParam.ACTIVE;
                c.Address = resCus.Address = Address;
                c.DOB = resCus.DOB = DateTime.Parse(DOB);
                c.Type = resCus.Type = SystemParam.TYPE_LOGIN_PHONE;
                c.Password = resCus.Password = Util.GenPass(password);
                c.Name = resCus.Name = UserName;
                c.Email = resCus.Email = Email;
                //c.ProvinceCode = resCus.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
                c.DistrictCode = resCus.DistrictCode = SystemParam.DISTRICT_DEFAULT;
                c.Sex = resCus.Sex = Sex;
                c.Role = resCus.Role = Role;
                c.AvatarUrl = resCus.AvatarUrl = AvatarUrl.Length != null  ? AvatarUrl : "https://st.quantrimang.com/photos/image/072015/22/avatar.jpg";
                c.LastLoginDate = resCus.LastLoginDate = DateTime.Now;
                c.ExpireTocken = resCus.ExpireTocken = DateTime.Now;
                c.Point = resCus.Point = 0;
                c.DeviceID = resCus.DeviceID = "";
                c.Status = resCus.Status = SystemParam.ACTIVE;
                c.CraeteDate = resCus.CraeteDate = DateTime.Now;
                c.UserID = resCus.UserID = user.UserID;
                c.Note = resCus.Note = Note;
                c.ProvinceCode = resCus.ProvinceCode = SystemParam.PROVINCE_DEFAULT;
                cnn.Customers.Add(c);
                cnn.SaveChanges();

                
                

                return rpBus.SuccessResult("Thành Công", resCus);
            }
            catch (Exception e)
            {
                return rpBus.ErrorResult(e.Message, SystemParam.PROCESS_ERROR);
            }
        }

        public CustomerModel GetUserDetail(int userID)
        {
            try
            {
                CustomerModel userDetail = new CustomerModel();

                var query = (from u in cnn.Users
                             join c in cnn.Customers on u.UserID equals c.UserID
                             where u.IsActive.Equals(SystemParam.ACTIVE) && u.UserID.Equals(userID)
                             select new CustomerModel
                             {
                                 UserID = u.UserID,
                                 Role = u.Role,
                                 Name = u.UserName,
                                 Phone = u.Phone,
                                 Email = c.Email,
                                 DOB = c.DOB,
                                 Sex = c.Sex,
                                 AvatarUrl = c.AvatarUrl,
                                 ProvinceCode = c.ProvinceCode,
                                 DistrictCode = c.DistrictCode,
                                 Address = c.Address,
                                 Status = c.Status,
                                 Type = c.Type,
                                 IsActive = c.IsActive,
                                 Password = c.Password,
                                 OTP = c.OTP,
                                 Note = c.Note,
                             }).FirstOrDefault();
                if (query != null && query.UserID > 0)
                {
                    return userDetail = query;
                }
                return userDetail;
            }
            catch
            {
                return new CustomerModel();
            }
        }

        //public int UpdateUser(int ID, string userName, string phone)
        //{
        //    try
        //    {
        //        Customer cus = cnn.Customers.Where(x => x.UserID == ID && x.IsActive == 1 && x.Role == SystemParam.ROLE_ADMIN).FirstOrDefault();
        //        cus.Name = userName;
        //        User user = cnn.Users.Find(ID);
        //        user.UserName = userName;
        //        user.Phone = phone;
        //        cnn.SaveChanges();
        //        return SystemParam.SUCCESS;
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}
        public JsonResultsModel UpdateUser(int ID, string UserName, string Phone, string DOB, string Address,
            int Sex, string Email, int Role, int Status, string AvatarUrl, string Note, int Province)
        {
            try
            {
                var data = new CustomerModel();

                Customer cus = cnn.Customers.Where(x => x.UserID == ID && x.IsActive == 1).FirstOrDefault();
                cus.Name = data.Name = UserName;
                User user = cnn.Users.Find(ID);
                user.UserName = UserName;
                user.Phone = Phone;
                cus.DOB = data.DOB = DateTime.Parse(DOB);
                cus.Address = data.Address = Address;
                cus.Sex = data.Sex = Sex;
                cus.Email = data.Email = Email;
                cus.Role = user.Role = data.Role = Role;
                cus.Status = data.Status = Status;
                cus.AvatarUrl = data.AvatarUrl = AvatarUrl;
                cus.Note = data.Note = Note;
                cus.ProvinceCode = data.ProvinceCode = Province;

                cnn.SaveChanges();
                return rpBus.SuccessResult("Thành Công", data);
            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }

        public int DeleteUser(int ID)
        {
            try
            {

                User user = cnn.Users.Find(ID);
                var cus = cnn.Customers.Where(x => x.IsActive == SystemParam.ACTIVE && x.UserID == ID).FirstOrDefault();
                Customer c = cnn.Customers.Find(cus.ID);
                c.IsActive = SystemParam.NO_ACTIVE_DELETE;
                user.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        public int RefreshUser(int ID)
        {
            try
            {
                Customer cus = cnn.Customers.Where(c => c.UserID == ID && c.IsActive == SystemParam.ACTIVE).FirstOrDefault();

                User user = cnn.Users.Find(ID);
                user.PassWord = Util.GenPass(user.Phone);
                cus.Password = user.PassWord;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<ListProvinceModel> DetailProvince()
        {

            try
            {
                List<ListProvinceModel> detailProvince = new List<ListProvinceModel>();
                var query = from p in cnn.Provinces
                            orderby p.Name
                            select new ListProvinceModel
                            {
                                Code = p.Code,
                                Name = p.Name,
                                Type = p.Type
                            };

                if (query != null && query.Count() > 0)
                {
                    detailProvince = query.ToList();
                    return detailProvince;
                }
                else
                    return detailProvince;
            }
            catch (Exception)
            {
                return new List<ListProvinceModel>();
            }
        }

    }
}
