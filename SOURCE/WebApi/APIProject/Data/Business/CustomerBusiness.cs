using Data.DB;
using Data.Model.APIWeb;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class CustomerBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        NotifyBusiness notiBus = new NotifyBusiness();
        PushNotifyBusiness pushNotiBus = new PushNotifyBusiness();
        public CustomerBusiness(MrFishEntities context = null) : base()
        {
        }

        //public List<CustomerTopOutputModel> GetCustomerTop()
        //{
        //    List<CustomerTopOutputModel> query = new List<CustomerTopOutputModel>();
        //    var listCus = from c in cnn.Customers
        //                  where c.IsActive.Equals(SystemParam.ACTIVE) && c.Status.Equals(SystemParam.STATUS_RUNNING)
        //                  orderby c.Point descending
        //                  select new CustomerTopOutputModel
        //                  {
        //                      CustomerName = c.Name,
        //                      Point = c.Point,
        //                      UrlImage = c.AvatarUrl
        //                  };
        //    if (listCus != null && listCus.Count() > 0) {
        //        query = listCus.ToList();
        //    }
        //    return query;
        //}
        public List<Province> LoadCityCustomer()
        {
            List<Province> listCity = new List<Province>();
            var query = from p in cnn.Provinces
                        orderby p.Name
                        select p;

            if (query != null && query.Count() > 0)
            {
                listCity = query.ToList();
                return listCity;
            }
            else
                return new List<Province>();
        }
        public List<Ranking> LoadRankCustomer()
        {
            List<Ranking> listCity = new List<Ranking>();
            var query = from p in cnn.Rankings
                        orderby p.RankName
                        select p;

            if (query != null && query.Count() > 0)
            {
                listCity = query.ToList();
                return listCity;
            }
            else
                return new List<Ranking>();
        }

        public List<District> loadDistrict(int ProvinceID)
        {
            List<District> listDistrict = new List<District>();
            var query = from d in cnn.Districts
                        where d.ProvinceCode.Equals(ProvinceID)
                        select d;
            if (query != null && query.Count() > 0)
            {
                //listDistrict = query.ToList();
                return query.ToList();
            }
            else
                return new List<District>();
        }

        public List<ListCustomerOutputModel> Search(DateTime? startdate, DateTime? endDate, int? City, string Phone, int? Rank, int? Status)
        {
            List<ListCustomerOutputModel> listCustomer = new List<ListCustomerOutputModel>();
            if (endDate.HasValue)
                endDate = endDate.Value.AddDays(1);
            //endDate = endDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);
            var query = from cus in cnn.Customers
                        where cus.IsActive.Equals(SystemParam.ACTIVE)
                        && cus.Role.Equals(SystemParam.ROLE_CUSTOMER)
                        && (startdate.HasValue ? cus.CraeteDate >= startdate.Value : true)
                        && (endDate.HasValue ? cus.CraeteDate <= endDate.Value : true)
                        && (Status != null ? cus.Status == Status : true)
                        && (Rank != null ? cus.RankID == Rank : true)
                        && (City != null ? cus.ProvinceCode == City : true)
                        orderby cus.ID descending
                        select new ListCustomerOutputModel
                        {
                            CustomerID = cus.ID,
                            CustomerName = cus.Name,
                            PhoneNumber = cus.Phone,
                            Role = cus.Role,
                            Point = cus.Point,
                            Status = cus.Status,
                            DOB = cus.DOB,
                            Email = cus.Email,
                            Address = cus.Address,
                            ProvinceCode = cus.ProvinceCode,
                            DistrictCode = cus.DistrictCode,
                            WardID = cus.WardID,
                            TypeLogin = cus.Type,
                            CreateDate = cus.CraeteDate,
                            Sex = cus.Sex,
                            RankID = cus.RankID,
                            ProvinceName = cus.Province.Name,
                        };
            if (query != null && query.Count() > 0)
            {
                listCustomer = query.ToList();
                if (!String.IsNullOrEmpty(Phone))
                    listCustomer = listCustomer.Where(u => Util.Converts(u.CustomerName.ToLower()).Contains(Util.Converts(Phone.ToLower())) || u.PhoneNumber.Contains(Phone)).ToList();

            }
            return listCustomer;
        }
        public IPagedList<CustomerReferModel> GetListCustomerRefer(int page,int limit,int ID)
        {
            try
            {
                var cus = cnn.Customers.FirstOrDefault(x => x.ID.Equals(ID));
                var model = cnn.Customers.Where(x => x.Code.Equals(cus.Phone) && x.Role.Equals(SystemParam.ROLE_CUSTOMER) && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => new CustomerReferModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Phone = x.Phone,
                    ImageUrl = x.AvatarUrl != "" ? (fullUrl + x.AvatarUrl) : "",
                    CreateDate = x.CraeteDate,
                }).OrderByDescending(x => x.ID).ToPagedList(page,limit);
                return model;
            }catch(Exception ex)
            {
                ex.ToString();
                return new List<CustomerReferModel>().ToPagedList(1, 1);
            }
        }

        public List<ListOrderNewModel> SearchOrderNew(int cusID)
        {
            var query = from od in cnn.Orders
                        join cus in cnn.Customers on od.CustomerID equals cus.ID
                        join dis in cnn.Districts on od.DistrictID equals dis.Code
                        join wa in cnn.Wards on od.WardID equals wa.ID
                        where od.IsActive.Equals(SystemParam.ACTIVE)
                        orderby od.CustomerID descending
                        select new ListOrderNewModel
                        {
                            ID = od.ID,
                            Code = od.Code,
                            CustomerID = od.CustomerID,
                            CustomerName = cus.Name,
                            TotalPrice = od.TotalPrice,
                            DiscountCoupon = od.DiscountCoupon,
                            DiscountRank = od.DiscountRank,
                            CouponID = od.CouponID,
                            BuyerName = od.BuyerName,
                            BuyerPhone = od.BuyerPhone,
                            BuyerAddress = od.BuyerAddress,
                            Note = od.Note,
                            ProvinceID = od.ProvinceID,
                            ProvinceName = cus.Province.Name,
                            DistrictID = od.DistrictID,
                            DistrictName = dis.Name,
                            WardID = od.WardID,
                            WardName = wa.Name,
                            Status = od.Status,
                            PaymentType = od.PaymentType,
                            IsPayment = od.IsPayment,
                            IsActive = od.IsActive,
                            CreateDate = od.CreateDate,
                            ProcessDate = od.ProcessDate,
                            CompleteDate = od.CompleteDate,
                            CancelDate = od.CancelDate,
                            RefundDate = od.RefundDate,
                        };
            var data = query.Where(x => x.CustomerID == cusID).ToList();
            return data;
        }
        public List<MemberPointHistoryModel> searchMemberPointHistory(int cusID)
        {
            var query = from mph in cnn.MembersPointHistories
                        join cus in cnn.Customers on mph.CustomerID equals cus.ID
                        where mph.IsActive.Equals(SystemParam.ACTIVE)
                        orderby mph.CustomerID descending
                        select new MemberPointHistoryModel
                        {
                            ID = mph.ID,
                            OrderID = mph.OrderID,
                            CustomerID = mph.CustomerID,
                            Point = mph.Point,
                            OrderCode = mph.AddPointCode,
                        };
            var data = query.Where(x => x.CustomerID == cusID).ToList();
            return data;
        }

        public List<MemberPointHistoryWebModel> searchMemberPointHistoryWeb(int cusID)
        {
            var query = from mph in cnn.MembersPointHistories
                        join cus in cnn.Customers on mph.CustomerID equals cus.ID
                        where mph.IsActive.Equals(SystemParam.ACTIVE)
                        orderby mph.CreateDate descending
                        select new MemberPointHistoryWebModel
                        {
                            ID = mph.ID,
                            CustomerID = mph.CustomerID,
                            OrderID = mph.OrderID,
                            Point = mph.Point,
                            OrderCode = mph.AddPointCode,
                            Comment = mph.Comment,
                            Title = mph.Title,
                            IsActive = mph.IsActive,
                            Balance = mph.Balance,
                            Date = mph.CreateDate,
                        };
            var data = query.Where(x => x.CustomerID == cusID).ToList();
            return data;
        }
        public List<ListReceiveAddressModel> searchReceiveAddress(int cusID)
        {
            var query = from ra in cnn.ReceiveAddresses
                        join cus in cnn.Customers on ra.CustomerID equals cus.ID
                        join pro in cnn.Provinces on ra.ProvinceID equals pro.Code
                        join dis in cnn.Districts on ra.DistrictID equals dis.Code
                        join wa in cnn.Wards on ra.WardID equals wa.ID
                        where ra.IsActive.Equals(SystemParam.ACTIVE)
                        && ra.CustomerID == cusID
                        orderby ra.CustomerID descending
                        select new ListReceiveAddressModel
                        {
                            ID = ra.ID,
                            CustomerID = ra.CustomerID,
                            Name = ra.Name,
                            Phone = ra.Phone,
                            ProvinceID = ra.ProvinceID,
                            ProvinceName = pro.Name,
                            DistrictID = ra.DistrictID,
                            DistrictName = dis.Name,
                            WardID = ra.WardID,
                            WardName = wa.Name,
                            Address = ra.Address,
                            CreateDate = ra.CreateDate,
                            IsActive = ra.IsActive,
                            IsDefault = ra.IsDefault,
                            
                        };
            var data = query.Where(x => x.CustomerID == cusID).ToList();
            return data;
        }
        public List<ListReferralCodeNewModel> SearchReferralCode(int cusID)
        {
            var query = from cr in cnn.CouponRefers
                        join cus in cnn.Customers on cr.CustomerID equals cus.ID
                        join cusRef in cnn.Customers on cr.CusRefID equals cusRef.ID
                        join cop in cnn.Coupons on cr.CouponID equals cop.ID
                        where cus.IsActive.Equals(SystemParam.ACTIVE)
                        && cr.CustomerID == cusID
                        orderby cr.CustomerID descending
                        select new ListReferralCodeNewModel
                        {
                            ID = cr.ID,
                            CustomerID = cr.CustomerID,
                            CusRefID = cr.CusRefID,
                            CusRefName = cusRef.Name,
                            CusRefPhone = cusRef.Phone,
                            CouponCode = cop.Code,
                            CouponName = cop.Name,
                            CouponFromDate = cop.FromDate.HasValue ? cop.FromDate : null,
                        };
            var data = query.Where(x => x.CustomerID == cusID).ToList();
            return data;
        }
        public ListCustomerDetailNewModel BindingCus (int ID)
        {
            var query = from cus in cnn.Customers
                        where cus.IsActive.Equals(SystemParam.ACTIVE)
                        && cus.Role.Equals(SystemParam.ROLE_CUSTOMER)
                        orderby cus.ID descending
                        select new ListCustomerDetailNewModel
                        {
                            CustomerID = cus.ID,
                            CustomerName = cus.Name,
                            PhoneNumber = cus.Phone,
                            Role = cus.Role,
                            Point = cus.Point,
                            Status = cus.Status,
                            DOB = cus.DOB,
                            Email = cus.Email,
                            Address = cus.Address,
                            ProvinceCode = cus.ProvinceCode,
                            DistrictCode = cus.DistrictCode,
                            WardID = cus.WardID,
                            TypeLogin = cus.Type,
                            CreateDate = cus.CraeteDate,
                            Sex = cus.Sex,
                            RankID = cus.RankID,
                            ProvinceName = cus.Province.Name,
                        };
            var data = query.Where(x => x.CustomerID == ID).FirstOrDefault();

            return data;
        }
        // cộng điểm nhiều khách hàng


        public Customer getCustomerByPhone(string Phone)
        {
            try
            {
                Customer cusDetail = cnn.Customers.Where(p => p.Phone.Equals(Phone)).SingleOrDefault();
                return cusDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Customer();
            }
        }

        public Customer cusDetail(int? ID)
        {
            try
            {
                Customer cusDetail = cnn.Customers.Where(p => p.ID == ID).SingleOrDefault();
                return cusDetail;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new Customer();
            }
        }
        public int ChangeRole(int ID)
        {
            try
            {
                Customer cus = cnn.Customers.Find(ID);
                cus.Role = SystemParam.ROLE_CUSTOMER_LOYAL;

                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int ChangeRoleBT(int ID)
        {
            try
            {
                Customer cus = cnn.Customers.Find(ID);
                cus.Role = SystemParam.ROLE_CUSTOMER;

                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public List<GetListHistoryMemberPointInputModel> SearchHistoryPoint(int cusID, string FromDate, string ToDate)
        {
            try
            {
                var query = from MBH in cnn.MembersPointHistories
                            where MBH.IsActive.Equals(SystemParam.ACTIVE)
                            && MBH.CustomerID == cusID
                            select new GetListHistoryMemberPointInputModel
                            {
                                HistoryID = MBH.ID,
                                AddPointCode = MBH.AddPointCode,
                                Point = MBH.Point,
                                Comment = MBH.Comment,
                            };
                if (FromDate != null && FromDate != "")
                {
                    DateTime? fd = Util.ConvertDate(FromDate);
                    query = query.Where(p => p.CreateDate >= fd);
                }
                if (ToDate != null && ToDate != "")
                {
                    DateTime? td = Util.ConvertDate(ToDate);
                    td = td.Value.AddDays(1);
                    query = query.Where(p => p.CreateDate <= td);
                }
                if (query != null && query.Count() > 0)
                    return query.OrderByDescending(x => x.CreateDate).ToList();
                else
                    return new List<GetListHistoryMemberPointInputModel>();
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<GetListHistoryMemberPointInputModel>();
            }
        }
        public int SaveEditCus(int ID, int RankCus, int StatusCus)
        {
            try
            {
                Customer cus = cnn.Customers.Find(ID);
                if(RankCus != cus.RankID.GetValueOrDefault())
                {
                    var rankNew = cnn.Rankings.FirstOrDefault(x => x.ID.Equals(RankCus) && x.IsActive.Equals(SystemParam.ACTIVE));
                    var title = "Bạn vừa được thăng lên hạng " + rankNew.RankName;
                    pushNotiBus.PushNotifyCustomer(title, SystemParam.NOTIFICATION_TYPE_RANK, cus, null, null,null);
                }
                cus.RankID = RankCus;
                cus.Status = StatusCus;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteCustomer(int ID)
        {
            try
            {
                var cusDelete = cnn.Customers.Find(ID);
                //if (cusDelete.Status == 1)
                //{
                //    return 2;
                //}
                cusDelete.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public List<Customer> TopPoint()
        {
            Customer p = new Customer();
            var topPoint = (from tp in cnn.Customers
                            where tp.IsActive.Equals(SystemParam.ACTIVE)
                            orderby tp.Point descending
                            select tp).Take(10).ToList();
            return topPoint;
            //var p = (from c in cnn.Customers
            //        orderby c.Point descending
            //        select c).ToList();
            // return cnn.Customers.Where(x => x.IsActive.Equals(SystemParam.ACTIVE)).Take(10).ToList();
        }
        public string countCustomer()
        {

            var listCustomer = cnn.Customers.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && (u.Role.Equals(SystemParam.ROLE_CUSTOMER)/* || u.Role.Equals(SystemParam.ROLE_CUSTOMER_LOYAL)*/));
            int activeCustomer = listCustomer.Where(u => u.Status.Equals(1)).Count();
            int countCustomer = listCustomer.Count();
            return "" /*+ activeCustomer + " / "*/ + countCustomer;
        }
        // Reset MK khách hàng
        public int RefreshCustomer(int id)
        {
            try
            {
                Customer cu = cnn.Customers.Find(id);
                cu.Password = Util.GenPass(cu.Phone);
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
        public async Task<int> SumPoint(int cusID)
        {
            try
            {

                var sum = cnn.MembersPointHistories.Where(x => x.CustomerID == cusID && x.IsActive.Equals(SystemParam.ACTIVE)).Sum(x => (int?)x.Point) ?? 0;

                return sum;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
