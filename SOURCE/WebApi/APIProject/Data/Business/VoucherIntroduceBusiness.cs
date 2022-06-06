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
    public class VoucherIntroduceBusiness : GenericBusiness
    {
        public VoucherIntroduceBusiness(MrFishEntities context = null) : base()
        {

        }
        public string fullUrl = Util.getFullUrl();
        public JsonResultsModel CreateVoucherIntroduce(string codeVouCreate, string nameVouCreate, int typeDiscountVouCreate, string discountVouCreate, string Note, string AddLogoVoucherCus)
        {
            try
            {
                var _discountVouCreate = Convert.ToInt32((discountVouCreate).ToString().Replace(",", ""));
                if(typeDiscountVouCreate == 2 && _discountVouCreate > 100) return rpBus.ErrorResult("Mức giảm không được lớn hơn 100%", SystemParam.CHECK_PERCENT);
                var resVoucherIntroduce = new VoucherIntroduceNewModel();
                Coupon VoucherIntroduce = new Coupon();
                //CouponRanking rankVoucherIntroduce = new CouponRanking();
                VoucherIntroduce.Code = resVoucherIntroduce.Code = codeVouCreate;
                VoucherIntroduce.Name = resVoucherIntroduce.Name = nameVouCreate;
                VoucherIntroduce.Quantity = resVoucherIntroduce.Quantity = 0;
                VoucherIntroduce.Remain = resVoucherIntroduce.Quantity = 0;
                VoucherIntroduce.TypeDiscount = resVoucherIntroduce.TypeDiscount = typeDiscountVouCreate;
                VoucherIntroduce.Discount = resVoucherIntroduce.Discount = _discountVouCreate;
                VoucherIntroduce.Status = resVoucherIntroduce.Status = 0;
                VoucherIntroduce.FromDate = resVoucherIntroduce.FromDate = DateTime.Now;
                VoucherIntroduce.ToDate = resVoucherIntroduce.ToDate = DateTime.Now;
                VoucherIntroduce.CreateDate = resVoucherIntroduce.CreateDate = DateTime.Now;
                VoucherIntroduce.Note = resVoucherIntroduce.Note = Note;
                VoucherIntroduce.Type = resVoucherIntroduce.Type = SystemParam.COUPON_TYPE_REFER;
                
                if (AddLogoVoucherCus != null)
                {
                    var img = AddLogoVoucherCus.Split(',').ToList();
                    var imgSlice = "";
                    for (int i = 0; i < img.Count; i++)
                    {
                        var index = img[i].IndexOf("files/");
                        imgSlice += (img[i].Substring(index + 6) + ",");
                    }

                    VoucherIntroduce.ImageUrl = resVoucherIntroduce.ImageUrl = imgSlice.TrimEnd(',');
                }
                VoucherIntroduce.IsActive = resVoucherIntroduce.IsActive = SystemParam.ACTIVE;
                cnn.Coupons.Add(VoucherIntroduce);
                cnn.SaveChanges();
                
                return rpBus.SuccessResult("Thành Công", resVoucherIntroduce);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }


        public int saveEditVoucherIntroduce(int ID, string imgVoucherIntroduceEdit, string NoteVoucherIntroduceEdit, int status, string nameVouEdit, int typeDiscountVouEdit, string discountVouEdit)
        {
            try
            {
                var _discountCreate = Convert.ToInt32((discountVouEdit).ToString().Replace(",", ""));
                if(typeDiscountVouEdit == 2 && _discountCreate > 100) return SystemParam.CHECK_PERCENT;
                Coupon _voucherCus = cnn.Coupons.Find(ID);
                if(status == 1)
                {
                    var listCou = cnn.Coupons.Where(x => x.Type == 2).ToList();
                    foreach (var item in listCou)
                    {
                        item.Status = 0;
                    }
                }
                if (imgVoucherIntroduceEdit != null)
                {
                    var index = imgVoucherIntroduceEdit.IndexOf("files/");
                    var imgSlice = imgVoucherIntroduceEdit.Substring(index + 6);
                    _voucherCus.ImageUrl = imgSlice;
                }
                _voucherCus.Note = NoteVoucherIntroduceEdit;
                _voucherCus.Status = status;
                _voucherCus.Name = nameVouEdit;
                _voucherCus.TypeDiscount = typeDiscountVouEdit;
                _voucherCus.Discount = _discountCreate;
                cnn.SaveChanges();
               
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public VoucherIntroduceNewModel DetailVoucherIntroduce(int ID)
        {
            try
            {

                var query = cnn.Coupons.Find(ID);
                var voucherCus = cnn.CouponCustomers.Where(x => x.CouponID.Equals(query.ID)).ToList();
                var voucherRef = cnn.CouponRefers.Where(x => x.CouponID.Equals(query.ID)).ToList();

                VoucherIntroduceNewModel detailCustomerNewModel = new VoucherIntroduceNewModel();
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
                detailCustomerNewModel.QuantityReceived = voucherRef.Count;

                if (query.ImageUrl != null)
                {
                    detailCustomerNewModel.ImageUrl = fullUrl + query.ImageUrl;
                }
                return detailCustomerNewModel;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new VoucherIntroduceNewModel();
            }
        }

        public int DeleteVoucherIntroduce(int ID)
        {
            try
            {

                Coupon  coupon = cnn.Coupons.Find(ID);
                if(coupon.Status == 1)
                {
                    return SystemParam.VOUCHER_STATUS;
                }
                coupon.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<VoucherIntroduceNewModel> SearchVoucherIntroduce(int Page, string nameVoucher, DateTime? fromDate, DateTime? toDate, int? typeDiscountVoucherIntroduce)
        {
            try
            {
                if (toDate.HasValue)
                    toDate = toDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);
                var query = from n in cnn.Coupons
                            where n.IsActive.Equals(SystemParam.ACTIVE)
                              && n.Type.Equals(SystemParam.COUPON_TYPE_REFER)
                              && (fromDate.HasValue ? n.CreateDate >= fromDate.Value : true)
                              && (toDate.HasValue ? n.CreateDate <= toDate.Value : true)
                              && (typeDiscountVoucherIntroduce != null ? n.TypeDiscount == typeDiscountVoucherIntroduce : true)
                            orderby n.ID descending
                            select new VoucherIntroduceNewModel
                            {
                                ID = n.ID,
                                Code = n.Code,
                                Name = n.Name,
                                Discount = n.Discount,
                                TypeDiscount = n.TypeDiscount,
                                Quantity = n.Quantity,
                                Remain = n.Remain,
                                QuantityReceived = cnn.CouponRefers.Count(x=>x.CouponID == n.ID),
                                Note = n.Note,
                                Status = n.Status,
                                Type = n.Type,
                                FromDate = n.FromDate,
                                ToDate = n.ToDate,
                                IsActive = n.IsActive,
                                CreateDate = n.CreateDate,
                                IsStatus = n.Status,
                                //CouponID = cor.CouponID,
                            };
                var lstVoucherIntroduce = query.ToList();
                if (!String.IsNullOrEmpty(nameVoucher))
                    lstVoucherIntroduce = lstVoucherIntroduce.Where(u => Util.Converts(u.Code.ToLower()).Contains(Util.Converts(nameVoucher.ToLower())) || u.Name.ToLower().Contains(nameVoucher.ToLower())).ToList();

                return lstVoucherIntroduce;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<VoucherIntroduceNewModel>();
            }
        }
      
    }
}
