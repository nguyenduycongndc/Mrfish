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
    public class VoucherCustomerBusiness : GenericBusiness
    {
        public VoucherCustomerBusiness(MrFishEntities context = null) : base()
        {

        }
        public string fullUrl = Util.getFullUrl();
        public List<Ranking> LoadRankCustomer()
        {
            List<Ranking> listCity = new List<Ranking>();
            var query = from p in cnn.Rankings
                        orderby p.DiscountPercent
                        select p;

            if (query != null && query.Count() > 0)
            {
                listCity = query.ToList();
                return listCity;
            }
            else
                return new List<Ranking>();
        }

        public JsonResultsModel CreateVoucherCustomer(string codeVoucherCustomerCreate, string nameVoucherCustomerCreate,
            string quantityVoucherCustomerCreate, int slTypeDiscountCreate, string discountCreate,
            int slStatusVoucherCustomerCreate, string[] testRank, string AddLogoVoucherCus,
            DateTime fromDateVoucherCreate, DateTime toDateVoucherCreate, string NoteCreateVoucherCustomer)
        {
            try
            {
                var _quantityVoucherCustomerCreate = Convert.ToInt32((quantityVoucherCustomerCreate).ToString().Replace(",", ""));
                var _discountCreate = Convert.ToInt32((discountCreate).ToString().Replace(",", ""));
                if (fromDateVoucherCreate > toDateVoucherCreate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);
                var data = cnn.Coupons.Where(x => x.Code == codeVoucherCustomerCreate && x.IsActive.Equals(SystemParam.ACTIVE) && x.Name.Equals(nameVoucherCustomerCreate)).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if (fromDateVoucherCreate == data[i].FromDate && toDateVoucherCreate == data[i].ToDate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);
                    if (fromDateVoucherCreate > data[i].FromDate && fromDateVoucherCreate < data[i].ToDate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);
                    if (toDateVoucherCreate > data[i].FromDate && toDateVoucherCreate < data[i].ToDate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);
                }
                if (fromDateVoucherCreate > toDateVoucherCreate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);
                if (fromDateVoucherCreate == toDateVoucherCreate) return rpBus.ErrorResult("Error", SystemParam.EXIST_TIME_INPUT);

                if (slTypeDiscountCreate == 2 && _discountCreate > 100) return rpBus.ErrorResult("Error", SystemParam.CHECK_PERCENT);

                var checkCount = cnn.Coupons.Where(x => x.Code == codeVoucherCustomerCreate && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                if (checkCount > 0) return rpBus.ErrorResult("Error", SystemParam.ERROR_CODE);
                var resVoucherCustomer = new VoucherCustomerNewModel();
                Coupon voucherCustomer = new Coupon();
                CouponRanking rankVoucherCustomer = new CouponRanking();
                voucherCustomer.Code = resVoucherCustomer.Code = codeVoucherCustomerCreate;
                voucherCustomer.Name = resVoucherCustomer.Name = nameVoucherCustomerCreate;
                voucherCustomer.Quantity = resVoucherCustomer.Quantity = _quantityVoucherCustomerCreate;
                voucherCustomer.Remain = resVoucherCustomer.Quantity = _quantityVoucherCustomerCreate;
                voucherCustomer.TypeDiscount = resVoucherCustomer.TypeDiscount = slTypeDiscountCreate;
                voucherCustomer.Discount = resVoucherCustomer.Discount = _discountCreate;
                voucherCustomer.Status = resVoucherCustomer.Status = slStatusVoucherCustomerCreate;
                voucherCustomer.FromDate = resVoucherCustomer.FromDate = fromDateVoucherCreate;
                voucherCustomer.ToDate = resVoucherCustomer.ToDate = toDateVoucherCreate;
                voucherCustomer.Note = resVoucherCustomer.Note = NoteCreateVoucherCustomer;
                voucherCustomer.Type = resVoucherCustomer.Type = SystemParam.COUPON_TYPE_RANK;

                if (AddLogoVoucherCus != null)
                {
                    var img = AddLogoVoucherCus.Split(',').ToList();
                    var imgSlice = "";
                    for (int i = 0; i < img.Count; i++)
                    {
                        var index = img[i].IndexOf("files/");
                        imgSlice += (img[i].Substring(index + 6) + ",");
                    }

                    voucherCustomer.ImageUrl = resVoucherCustomer.ImageUrl = imgSlice.TrimEnd(',');
                }
                voucherCustomer.CreateDate = DateTime.Now;
                voucherCustomer.IsActive = resVoucherCustomer.IsActive = SystemParam.ACTIVE;
                cnn.Coupons.Add(voucherCustomer);
                cnn.SaveChanges();
                if (testRank != null)
                {
                    var coutRank = testRank.ToList();
                    for (int i = 0; i < coutRank.Count; i++)
                    {
                        rankVoucherCustomer.CouponID = voucherCustomer.ID;
                        rankVoucherCustomer.RankID = int.Parse(coutRank[i]);
                        cnn.CouponRankings.Add(rankVoucherCustomer);
                        cnn.SaveChanges();
                    }
                }
                return rpBus.SuccessResult("Thành Công", resVoucherCustomer);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }


        public int saveEditVoucherCustomer(int ID, int statusVouEdit, int quantityVouEdit, DateTime fromDateVoucherEdit, DateTime toDateVoucherEdit, string imgVou, string NoteVoucherEdit)
        {
            try
            {
                var voucherCus = cnn.CouponCustomers.Where(x => x.CouponID.Equals(ID)).ToList();
                if (fromDateVoucherEdit > toDateVoucherEdit) return SystemParam.EXIST_TIME;
                if (quantityVouEdit < voucherCus.Count) return SystemParam.EXIST_QUAMTITY;
                Coupon _voucherCus = cnn.Coupons.Find(ID);
                _voucherCus.Status = statusVouEdit;
                if (voucherCus.Count == 0)
                {
                    _voucherCus.Remain = quantityVouEdit;
                }
                else { _voucherCus.Remain = quantityVouEdit - voucherCus.Count; }
                _voucherCus.Quantity = quantityVouEdit;
                _voucherCus.FromDate = fromDateVoucherEdit;
                _voucherCus.ToDate = toDateVoucherEdit;

                if (imgVou != null)
                {
                    var index = imgVou.IndexOf("files/");
                    var imgSlice = imgVou.Substring(index + 6);
                    _voucherCus.ImageUrl = imgSlice;
                }
                _voucherCus.Note = NoteVoucherEdit;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public VoucherCustomerNewModel DetailVoucherCustomer(int ID)
        {
            try
            {
                var query = cnn.Coupons.Find(ID);
                var dataCus = cnn.CouponRankings.Where(x => x.CouponID.Equals(query.ID)).ToList();
                var voucherCus = cnn.CouponCustomers.Where(x => x.CouponID.Equals(query.ID)).ToList();

                VoucherCustomerNewModel detailCustomerNewModel = new VoucherCustomerNewModel();
                detailCustomerNewModel.ID = ID;
                detailCustomerNewModel.Code = query.Code;
                detailCustomerNewModel.Name = query.Name;
                detailCustomerNewModel.Quantity = query.Quantity;
                detailCustomerNewModel.Remain = query.Remain;
                detailCustomerNewModel.TypeDiscount = query.TypeDiscount;
                detailCustomerNewModel.Discount = query.Discount;
                detailCustomerNewModel.Status = query.Status;
                detailCustomerNewModel.FromDate = query.FromDate;
                detailCustomerNewModel.ToDate = query.ToDate;
                detailCustomerNewModel.DetailFromDate = query.FromDate.Value.ToString("yyyy-MM-dd");
                detailCustomerNewModel.DetailToDate = query.ToDate.Value.ToString("yyyy-MM-dd");
                detailCustomerNewModel.Note = query.Note;
                detailCustomerNewModel.IsActive = query.IsActive;
                detailCustomerNewModel.CreateDate = query.CreateDate;
                detailCustomerNewModel.AmountUsed = voucherCus.Count;

                for (int i = 0; i < dataCus.Count; i++)
                {
                    switch (dataCus[i].RankID)
                    {
                        case SystemParam.RANK_MEMBERSHIP: detailCustomerNewModel.RankTV = 1; break;
                        case SystemParam.RANK_SILVER: detailCustomerNewModel.RankB = 1; break;
                        case SystemParam.RANK_GOLDEN: detailCustomerNewModel.RankV = 1; break;
                        case SystemParam.RANK_DIAMOND: detailCustomerNewModel.RankKC = 1; break;

                    }
                    //detailCustomerNewModel.ListRank += dataCus[i].RankID.ToString() + ",";
                }
                if (query.ImageUrl != null)
                {
                    detailCustomerNewModel.ImageUrl = fullUrl + query.ImageUrl;
                }
                return detailCustomerNewModel;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new VoucherCustomerNewModel();
            }
        }

        public int DeleteVoucherCustomer(int ID)
        {
            try
            {

                Coupon coupon = cnn.Coupons.Find(ID);

                coupon.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<VoucherCustomerNewModel> SearchVoucherCustomer(int Page, string nameVoucher, DateTime? fromDate, DateTime? toDate, int? typeDiscount, int? status, int? rank)
        {
            try
            {
                if (toDate.HasValue)
                    //toDate = toDate.Value.AddDays(1);
                toDate = toDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<VoucherCustomerNewModel> lstVoucherCustomer = new List<VoucherCustomerNewModel>();
                var query = from n in cnn.Coupons
                            join cor in cnn.CouponRankings on n.ID equals cor.CouponID
                            join ran in cnn.Rankings on cor.RankID equals ran.ID
                            where n.IsActive.Equals(SystemParam.ACTIVE)
                              && (fromDate.HasValue ? n.FromDate >= fromDate.Value : true)
                              && (toDate.HasValue ? n.ToDate <= toDate.Value : true)
                              && (status != null ? n.Status == status : true)
                              && (typeDiscount != null ? n.TypeDiscount == typeDiscount : true)
                              //&& (rank != null ? cor.RankID == rank : true)
                              && n.Type.Equals(SystemParam.COUPON_TYPE_RANK)
                            orderby n.ID descending
                            select new VoucherCustomerNewModel
                            {
                                ID = n.ID,
                                Code = n.Code,
                                Name = n.Name,
                                Discount = n.Discount,
                                TypeDiscount = n.TypeDiscount,
                                Quantity = n.Quantity,
                                Remain = n.Remain,
                                Note = n.Note,
                                Status = n.Status,
                                Type = n.Type,
                                FromDate = n.FromDate,
                                ToDate = n.ToDate,
                                IsActive = n.IsActive,
                                CreateDate = n.CreateDate,
                                RankID = cor.RankID,
                                RankName = ran.RankName,
                                DiscountPercent = ran.DiscountPercent,
                            };


                foreach (var item in query.ToList())
                {
                    if (lstVoucherCustomer.Count(x => x.Code == item.Code && x.TypeDiscount == item.TypeDiscount && x.Type.Equals(SystemParam.COUPON_TYPE_RANK)) > 0) continue;

                    var rankName = new List<string>();
                    var listRank = query.Where(x => x.Code == item.Code && x.TypeDiscount == item.TypeDiscount && x.Type.Equals(SystemParam.COUPON_TYPE_RANK)).OrderBy(x=>x.DiscountPercent).Select(x => x.RankID).ToList();

                    foreach (var rk in listRank)
                    {
                        switch (rk)
                        {
                            case SystemParam.RANK_MEMBERSHIP: rankName.Add("Thành viên"); break;
                            case SystemParam.RANK_SILVER: rankName.Add("Bạc"); break;
                            case SystemParam.RANK_GOLDEN: rankName.Add("Vàng"); break;
                            case SystemParam.RANK_DIAMOND: rankName.Add("Kim cương"); break;
                        }
                    }
                    item.RankName = string.Join(", ", rankName);

                    if (lstVoucherCustomer.Count(x => x.Code == item.Code && x.TypeDiscount == item.TypeDiscount) == 0)
                    {
                        lstVoucherCustomer.Add(item);
                    }
                    if (rank.HasValue)
                    {
                        lstVoucherCustomer = lstVoucherCustomer.Where(u => u.RankID == rank).ToList();
                    }
                }
                if (!String.IsNullOrEmpty(nameVoucher))
                    lstVoucherCustomer = lstVoucherCustomer.Where(u => Util.Converts(u.Code.ToLower()).Contains(Util.Converts(nameVoucher.ToLower())) || u.Name.ToLower().Contains(nameVoucher.ToLower())).ToList();

                return lstVoucherCustomer;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<VoucherCustomerNewModel>();
            }
        }
        public List<Ranking> LoadRank()
        {
            List<Ranking> listCity = new List<Ranking>();
            var query = from p in cnn.Rankings
                        orderby p.DiscountPercent
                        select p;

            if (query != null && query.Count() > 0)
            {
                listCity = query.ToList();
                return listCity;
            }
            else
                return new List<Ranking>();
        }
    }
}
