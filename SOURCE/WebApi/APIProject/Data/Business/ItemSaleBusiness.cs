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
    public class ItemSaleBusiness : GenericBusiness
    {
        public ItemSaleBusiness(MrFishEntities context = null) : base()
        {

        }
        public string fullUrl = Util.getFullUrl();

        public int CreateItemSale(int CategoryID, int itemId, int quantity, string Price, DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                if(fromDateTime > toDateTime) return SystemParam.EXIST_TIME_INPUT;
                var data = cnn.ItemsSales.Where(x => x.ItemID == itemId && x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if(fromDateTime == data[i].FromDate || toDateTime == data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(fromDateTime <= data[i].FromDate && toDateTime >= data[i].FromDate && toDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(fromDateTime <= data[i].FromDate && toDateTime >= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(fromDateTime <= data[i].FromDate && toDateTime >= data[i].FromDate && toDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(fromDateTime >= data[i].FromDate && fromDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(fromDateTime <= data[i].ToDate && toDateTime >= data[i].ToDate) return SystemParam.EXIST_TIME;

                    if(toDateTime == data[i].FromDate || fromDateTime == data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(toDateTime <= data[i].FromDate && fromDateTime >= data[i].FromDate && fromDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(toDateTime <= data[i].FromDate && fromDateTime >= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(toDateTime <= data[i].FromDate && fromDateTime >= data[i].FromDate && fromDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(toDateTime >= data[i].FromDate && toDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    if(toDateTime <= data[i].ToDate && fromDateTime >= data[i].ToDate) return SystemParam.EXIST_TIME;

                    //if(toDateTime == data[i].FromDate || fromDateTime == data[i].ToDate) return SystemParam.EXIST_TIME;
                    //if(toDateTime >= data[i].FromDate && toDateTime <= data[i].ToDate) return SystemParam.EXIST_TIME;
                    //if(toDateTime <= data[i].FromDate && toDateTime >= data[i].ToDate) return SystemParam.EXIST_TIME;
                }
                var dtItem = cnn.Items.Where(x => x.ID == itemId && x.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                var _price = (int)dtItem.Price;
                var _priceIn = Convert.ToInt32((Price).ToString().Replace(",", ""));
                if (_priceIn > _price)
                {
                    return SystemParam.EXIST_COMPARE;
                }
                if (fromDateTime > toDateTime) return SystemParam.EXIST_TIME;
                if (fromDateTime == toDateTime) return SystemParam.EXIST_TIME;
                
                ItemsSale itemSale = new ItemsSale();
                itemSale.ItemID = itemId;
                itemSale.Quantity = itemSale.Remain = quantity;
                itemSale.QuantitySold = 0;
                itemSale.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                itemSale.FromDate = fromDateTime;
                itemSale.ToDate = toDateTime;
                itemSale.CreateDate = DateTime.Now;
                itemSale.IsActive = SystemParam.ACTIVE;
                cnn.ItemsSales.Add(itemSale);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public ItemSaleEditModel LoadItemSale(int ID)
        {
            try
            {
                var obj = cnn.ItemsSales.Find(ID);
                var lstI = (from i in cnn.Items
                            where i.ID == obj.ItemID && i.IsActive == 1
                            select new { i.ID, i.Name, i.GroupItemID }).FirstOrDefault();
                ItemSaleEditModel ItemSale = new ItemSaleEditModel();
                ItemSale.ID = ID;
                ItemSale.ItemID = lstI.ID;
                ItemSale.CategoryID = lstI.GroupItemID;
                ItemSale.Price = obj.Price.ToString();
                ItemSale.Quantity = obj.Quantity;
                ItemSale.QuantitySold = obj.QuantitySold;
                ItemSale.Remain = obj.Remain;
                ItemSale.FromDate = obj.FromDate.ToString("yyyy-MM-dd");
                ItemSale.ToDate = obj.ToDate.ToString("yyyy-MM-dd");
                ItemSale.FromTime = obj.FromDate.ToString("HH:mm");
                ItemSale.ToTime = obj.ToDate.ToString("HH:mm");

                return ItemSale;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ItemSaleEditModel();
            }
        }


        //Lưu lại cập nhật sản phẩm
        public int SaveEditItemSale(int ID, int CategoryID, int itemId, int quantity, string Price, DateTime fromDateTime, DateTime toDateTime)
        {
            try
            {
                var data = cnn.ItemsSales.Where(x => x.ID != ID && x.ItemID == itemId && x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if (fromDateTime == data[i].FromDate && toDateTime == data[i].ToDate)
                    {
                        return SystemParam.EXIST_TIME;
                    }
                }
                var dtItem = cnn.Items.Where(x => x.ID == itemId && x.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                var _price = (int)dtItem.Price;
                var _priceIn = Convert.ToInt32((Price).ToString().Replace(",", ""));
                if (_priceIn > _price)
                {
                    return SystemParam.EXIST_COMPARE;
                }
                ItemsSale item = cnn.ItemsSales.Find(ID);
                if(quantity <= item.QuantitySold)
                {
                    return SystemParam.EXIST_QUAMTITY;
                }
                if(fromDateTime > toDateTime) return SystemParam.EXIST_TIME;
                if (fromDateTime == toDateTime) return SystemParam.EXIST_TIME;

                item.ItemID = itemId;
                item.Quantity = quantity;
                item.Remain = quantity - item.QuantitySold;
                item.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                item.FromDate = fromDateTime;
                item.ToDate = toDateTime;
                item.CreateDate = DateTime.Now;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteItemSale(int ID)
        {
            try
            {
                ItemsSale itemSale = cnn.ItemsSales.Find(ID);
                itemSale.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<ListItemSaleWebModel> SearchItemSaleWeb(int Page, DateTime? fromDateCreate, DateTime? toDateCreate, string itemSaleName, string category)
        {
            try
            {
                if (category == "1")
                {
                    category = null;
                }
                if (toDateCreate.HasValue)
                    toDateCreate = toDateCreate.Value.AddDays(1);
                    //toDateCreate = toDateCreate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<ListItemSaleWebModel> lstItemOutput = new List<ListItemSaleWebModel>();
                var query = from s in cnn.ItemsSales
                            join i in cnn.Items on s.ItemID equals i.ID
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            where s.IsActive.Equals(SystemParam.ACTIVE)
                              && s.IsActive.Equals(SystemParam.ACTIVE)
                              && g.IsActive.Equals(SystemParam.ACTIVE)
                              && (fromDateCreate.HasValue ? s.FromDate >= fromDateCreate.Value : true)
                              && (toDateCreate.HasValue ? s.ToDate <= toDateCreate.Value : true)
                              && (toDateCreate.HasValue ? s.ToDate <= toDateCreate.Value : true)
                              && (s.Quantity != 0)
                            orderby s.ID descending
                            select new ListItemSaleWebModel
                            {
                                ID = s.ID,
                                Name = g.Name,
                                ItemCode = i.Code,
                                ItemName = i.Name,
                                ImgUrl = fullUrl + i.ImageUrl,
                                //Technical = i.Technical,
                                Description = i.Description,
                                Category = i.GroupItem != null ? i.GroupItem.Name : "Chưa được cập nhật",
                                ItemStatus = i.Status,
                                Price = s.Price,
                                CreateDateSale = s.CreateDate,
                                IsActive = i.IsActive,
                                FromDate = s.FromDate,
                                ToDate = s.ToDate,
                                Quantity = s.Quantity,
                            };
                lstItemOutput = query.ToList();

                if (!String.IsNullOrEmpty(itemSaleName))
                    lstItemOutput = lstItemOutput.Where(u => Util.Converts(u.ItemName.ToLower()).Contains(Util.Converts(itemSaleName.ToLower())) || u.ItemCode.ToLower().Contains(itemSaleName.ToLower())).ToList();
                if (category != null)
                {
                    lstItemOutput = lstItemOutput.Where(u => u.Category.Contains(category)).ToList();
                }

                return lstItemOutput;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListItemSaleWebModel>();
            }
        }
        public List<CategoryModel> getListCategory()
        {
            try
            {
                List<CategoryModel> listCategory = new List<CategoryModel>();
                var query = from c in cnn.GroupItems
                            where c.IsActive == SystemParam.ACTIVE
                            && c.Status.Equals(1)
                            select new CategoryModel
                            {
                                CategoryID = c.ID,
                                Name = c.Name,
                                ParentID = c.ParentID
                            };

                if (query != null && query.Count() > 0)
                {
                    listCategory = query.ToList();
                    return listCategory;
                }
                else
                    return listCategory;
            }
            catch (Exception)
            {
                return new List<CategoryModel>();
            }

        }
        public List<CategoryModel> getListCategoryEdit()
        {
            try
            {
                List<CategoryModel> listCategory = new List<CategoryModel>();
                var query = from c in cnn.GroupItems
                            where c.IsActive == SystemParam.ACTIVE
                            select new CategoryModel
                            {
                                CategoryID = c.ID,
                                Name = c.Name,
                                ParentID = c.ParentID
                            };

                if (query != null && query.Count() > 0)
                {
                    listCategory = query.ToList();
                    return listCategory;
                }
                else
                    return listCategory;
            }
            catch (Exception)
            {
                return new List<CategoryModel>();
            }

        }
    }
}
