using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class CouponBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        public CouponBusiness(MrFishEntities context = null) : base()
        {
        }
        public int GetCouponCount(int CusID)
        {
            try
            {
                var cusRank = cnn.Customers.Where(x => x.ID == CusID).Select(x => x.RankID).FirstOrDefault();
                var listCouponUsed = cnn.CouponCustomers.Where(x => x.CustomerID == CusID).Select(x => x.CouponID).ToList();
                var countCouponRanking = cnn.Coupons.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && (x.CouponRankings.Count > 0 ? x.CouponRankings.Select(cr => cr.RankID).Contains(cusRank.Value) : true)
                                        && x.Remain > 0 && x.Type == SystemParam.COUPON_TYPE_RANK && x.Status.Equals(SystemParam.ACTIVE) && !listCouponUsed.Contains(x.ID)
                                        && (x.FromDate.HasValue ? x.FromDate.Value <= DateTime.Now : true) && (x.ToDate.HasValue ? x.ToDate.Value >= DateTime.Now : true)).Count();
                var countCouponRefer = cnn.CouponRefers.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Coupon.IsActive.Equals(SystemParam.ACTIVE) && x.EndDate > DateTime.Now).Count();
                return countCouponRefer + countCouponRanking;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public bool CheckCoupon(int ID, int CusID)
        {
            var cusRank = cnn.Customers.Where(x => x.ID == CusID).Select(x => x.RankID).FirstOrDefault();
            var listCouponUsed = cnn.CouponCustomers.Where(x => x.CustomerID == CusID).Select(x => x.CouponID).ToList();
            var checkCoupon = cnn.Coupons.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.ID.Equals(ID)
                                && ((!listCouponUsed.Contains(x.ID) && x.Remain > 0 && x.Type == SystemParam.COUPON_TYPE_RANK && x.Status.Equals(SystemParam.ACTIVE)
                                && (x.CouponRankings.Count > 0 ? x.CouponRankings.Select(cr => cr.RankID).Contains(cusRank.Value) : true)
                                && (x.FromDate.HasValue ? x.FromDate.Value <= DateTime.Now : true) && (x.ToDate.HasValue ? x.ToDate.Value >= DateTime.Now : true))
                                || ((x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE)  && cr.EndDate > DateTime.Now).Count() > 0) && x.Type == SystemParam.COUPON_TYPE_REFER))).FirstOrDefault();
            if (checkCoupon != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<CouponModel> GetListCoupon(int CusID)
        {
            try
            {
                var cusRank = cnn.Customers.Where(x => x.ID == CusID).Select(x => x.RankID).FirstOrDefault();
                var listCouponUsed = cnn.CouponCustomers.Where(x => x.CustomerID == CusID).Select(x => x.CouponID).ToList();
                var model = cnn.Coupons.Where(x => x.IsActive.Equals(SystemParam.ACTIVE)
                                && ((!listCouponUsed.Contains(x.ID) && x.Remain > 0 && x.Type == SystemParam.COUPON_TYPE_RANK && x.Status.Equals(SystemParam.ACTIVE)
                                && (x.CouponRankings.Count > 0 ? x.CouponRankings.Select(cr => cr.RankID).Contains(cusRank.Value) : true)
                                && (x.FromDate.HasValue ? x.FromDate.Value <= DateTime.Now : true) && (x.ToDate.HasValue ? x.ToDate.Value >= DateTime.Now : true))
                                || ((x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE)  && cr.EndDate > DateTime.Now).Count() > 0) && x.Type == SystemParam.COUPON_TYPE_REFER))).Select(x => new CouponModel
                                {
                                    ID = x.ID,
                                    Code = x.Code,
                                    Discount = x.Discount,
                                    TypeDiscount = x.TypeDiscount,
                                    FromDate = x.FromDate,
                                    ToDate = x.Type == SystemParam.COUPON_TYPE_REFER ? x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE) && cr.EndDate > DateTime.Now).Max(cr => cr.EndDate) : x.ToDate,
                                    Remain = x.Type == SystemParam.COUPON_TYPE_REFER ? x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE) && cr.EndDate > DateTime.Now).Count() : 1,
                                    ImageUrl = fullUrl + x.ImageUrl,
                                    Name = x.Name,
                                    Note = x.Note,
                                    ListRank = x.CouponRankings.OrderBy(cr => cr.RankID).Select(cr => cr.Ranking.RankName).ToList()
                                }).ToList();
                return model;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<CouponModel>();
            }
        }
        public CouponModel GetCouponDetail(int ID, int CusID)
        {
            try
            {
                var model = cnn.Coupons.Where(x => x.ID.Equals(ID) && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => new CouponModel
                {
                    ID = x.ID,
                    Code = x.Code,
                    Discount = x.Discount,
                    TypeDiscount = x.TypeDiscount,
                    FromDate = x.FromDate,
                    ToDate = x.Type == SystemParam.COUPON_TYPE_REFER ? x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE) && cr.EndDate > DateTime.Now).Max(cr => cr.EndDate) : x.ToDate,
                    Remain = x.Type == SystemParam.COUPON_TYPE_REFER ? x.CouponRefers.Where(cr => cr.CustomerID.Equals(CusID) && cr.IsActive.Equals(SystemParam.ACTIVE) && cr.EndDate > DateTime.Now).Count() : 1,
                    ImageUrl = fullUrl + x.ImageUrl,
                    Name = x.Name,
                    Note = x.Note,
                    ListRank = x.CouponRankings.OrderBy(cr => cr.RankID).Select(cr => cr.Ranking.RankName).ToList()
                }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

    }
}
