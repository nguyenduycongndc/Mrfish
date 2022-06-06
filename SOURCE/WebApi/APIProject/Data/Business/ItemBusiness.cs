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
    public class ItemBusiness : GenericBusiness
    {
        public ItemBusiness(MrFishEntities context = null) : base()
        {

        }
        public string fullUrl = Util.getFullUrl();
        public List<ListItemOutputModel> Search(int Page, DateTime? fromDate, DateTime? toDate, string itemName, string category, int? Status, int? IsHot)
        {
            try
            {
                if (category == "1")
                {
                    category = null;
                }
                //DateTime? startdate = Util.ConvertDate(fromDate);
                //DateTime? endDate = Util.ConvertDate(toDate);
                //if (endDate.HasValue)
                //    endDate = endDate.Value.AddDays(1);
                //endDate = endDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<ListItemOutputModel> lstItemOutput = new List<ListItemOutputModel>();
                var query = (from i in cnn.Items
                             join g in cnn.GroupItems on i.GroupItemID equals g.ID
                             where i.IsActive.Equals(SystemParam.ACTIVE)
                               && (fromDate.HasValue ? i.CreateDate >= fromDate.Value : true)
                               && (toDate.HasValue ? i.CreateDate <= toDate.Value : true)
                             orderby i.ID descending
                             select new ListItemOutputModel
                             {
                                 ID = i.ID,
                                 Name = g.Name,
                                 Code = i.Code,
                                 ImageUrl = fullUrl + i.ImageUrl,
                                 Description = i.Description,
                                 Price = i.Price,
                                 CreateDate = i.CreateDate,
                             }).ToList();

                return query;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListItemOutputModel>();
            }
        }

        //public int CreateItem(int value1, int? special, string code, string name, int value2, string price, int value3, string imageUrl, string note)
        //{
        //    throw new NotImplementedException();
        //}

        //public int CreateItem(int value, int? special, string code, string name, string price, string imageUrl, string note)
        //{
        //    throw new NotImplementedException();
        //}

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
        public IPagedList<ListItemOutputModel> GetListItem(int page, int limit, string search, int? categoryID, int? isHot, int? orderPrice, int? isFlashSale)
        {
            try
            {
                var listItem = (from i in cnn.Items
                                join g in cnn.GroupItems on i.GroupItemID equals g.ID
                                where i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE)
                               && (!String.IsNullOrEmpty(search) ? i.Name.Contains(search) : true)
                               && (categoryID.HasValue ? g.ID.Equals(categoryID.Value) : true) && (isHot.HasValue ? i.IsHot.Equals(isHot.Value) : true)
                              && (isFlashSale.HasValue ? (isFlashSale.Value.Equals(SystemParam.ITEM_FLASH_SALE) ? (i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE)
                              && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Count() > 0) : true) : true)
                               && g.IsActive.Equals(SystemParam.ACTIVE) && g.Status.Equals(SystemParam.ACTIVE)
                                orderby i.ID descending
                                select new ListItemOutputModel
                                {
                                    ID = i.ID,
                                    Name = i.Name,
                                    Code = i.Code,
                                    ImageUrl = i.ImageUrl,
                                    Description = i.Description,
                                    CategoryID = g.ID,
                                    CategoryName = g.Name,
                                    Status = i.Status,
                                    Price = i.Price,
                                    IsHot = i.IsHot,
                                    PriceSale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                                    SalePercent = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Count() > 0 ?
                                  ((int)(i.Price - i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault()) * 100 / (int)i.Price) : 0,
                                    CreateDate = i.CreateDate,
                                }).AsEnumerable().Select(x => new ListItemOutputModel
                                {
                                    ID = x.ID,
                                    ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault(),
                                    Price = x.Price,
                                    Name = x.Name,
                                    CategoryID = x.CategoryID,
                                    CategoryName = x.CategoryName,
                                    Status = x.Status,
                                    Code = x.Code,
                                    IsHot = x.IsHot,
                                    Description = x.Description,
                                    PriceSale = x.PriceSale,
                                    SalePercent = x.SalePercent,
                                    CreateDate = x.CreateDate,
                                }).AsQueryable();
                if (orderPrice.HasValue)
                {
                    if (orderPrice.Value.Equals(SystemParam.ORDER_PRICE_LOW_TO_HIGH))
                    {
                        return listItem.OrderBy(x => x.PriceSale > 0 ? x.PriceSale : x.Price).ToPagedList(page, limit);
                    }
                    else
                    {
                        return listItem.OrderByDescending(x => x.PriceSale > 0 ? x.PriceSale : x.Price).ToPagedList(page, limit);
                    }
                }
                return listItem.ToPagedList(page, limit);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListItemOutputModel>().ToPagedList(1, 1);
            }
        }
        public ItemDetailModel GetItemDetail(int ID,int CusID)
        {
            try
            {
                var data = (from i in cnn.Items
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            where i.ID.Equals(ID) && i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE)
                             && g.IsActive.Equals(SystemParam.ACTIVE) && g.Status.Equals(SystemParam.ACTIVE)
                            select new
                            {
                                ID = i.ID,
                                ImageUrl = i.ImageUrl,
                                Price = i.Price,
                                Name = i.Name,
                                Code = i.Code,
                                Description = i.Description,
                                IsHot = i.IsHot,
                                PriceSale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                                QuantitySale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Remain).FirstOrDefault()
                                - cnn.Carts.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.ItemID.Equals(i.ID)).Select(x => x.Quantity).FirstOrDefault(),
                                SalePercent = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Count() > 0 ?
                                  ((int)(i.Price - i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault()) * 100 / (int)i.Price) : 0,
                            }).AsEnumerable().Select(x => new ItemDetailModel
                            {
                                ID = x.ID,
                                ImageUrl = x.ImageUrl.Split(',').Select(i => fullUrl + i).ToList(),
                                Price = x.Price,
                                Name = x.Name,
                                Code = x.Code,
                                IsHot = x.IsHot,
                                Description = x.Description,
                                PriceSale = x.PriceSale,
                                SalePercent = x.SalePercent,
                                QuantitySale = x.QuantitySale > 0 ? x.QuantitySale : 0 
                            }).FirstOrDefault();
                return data;
            }
            catch (Exception)
            {
                return new ItemDetailModel();
            }

        }
        public List<ItemHomeModel> GetListItemRelative(int ID)
        {
            try
            {
                var item = cnn.Items.Where(x => x.ID.Equals(ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                var data = (from i in cnn.Items
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            where i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE) && i.GroupItemID.Equals(item.GroupItemID)
                            && g.IsActive.Equals(SystemParam.ACTIVE) && g.Status.Equals(SystemParam.ACTIVE) && !i.ID.Equals(ID)
                            select new ItemHomeModel
                            {
                                ID = i.ID,
                                ImageUrl = i.ImageUrl,
                                Price = i.Price,
                                Name = i.Name,
                                IsHot = i.IsHot,
                                PriceSale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                                SalePercent = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Count() > 0 ?
                                  ((int)(i.Price - i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault()) * 100 / (int)i.Price) : 0,
                            }).AsEnumerable().Select(x => new ItemHomeModel
                            {
                                ID = x.ID,
                                ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault(),
                                Price = x.Price,
                                Name = x.Name,
                                PriceSale = x.PriceSale,
                                SalePercent = x.SalePercent,
                                IsHot = x.IsHot
                            }).OrderByDescending(x => x.PriceSale != 0 ? 1 : 0).ThenByDescending(x => x.ID).Take(SystemParam.QTY_CONTENT_HOME_SCREEN).ToList();
                return data;
            }
            catch (Exception)
            {
                return new List<ItemHomeModel>();
            }

        }
        public List<ItemHomeModel> GetListItemHot()
        {
            try
            {
                var data = (from i in cnn.Items
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            where i.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE) && i.IsHot.Equals(SystemParam.ITEM_HOT)
                            && g.IsActive.Equals(SystemParam.ACTIVE) && g.Status.Equals(SystemParam.ACTIVE)
                            select new ItemHomeModel
                            {
                                ID = i.ID,
                                ImageUrl = i.ImageUrl,
                                Price = i.Price,
                                Name = i.Name,
                                IsHot = i.IsHot,
                                PriceSale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                                SalePercent = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Count() > 0 ?
                                  ((int)(i.Price - i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault()) * 100 / (int)i.Price) : 0,
                            }).AsEnumerable().Select(x => new ItemHomeModel
                            {
                                ID = x.ID,
                                ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault(),
                                Price = x.Price,
                                Name = x.Name,
                                PriceSale = x.PriceSale,
                                SalePercent = x.SalePercent,
                                IsHot = x.IsHot
                            }).OrderByDescending(x => x.PriceSale != 0 ? 1 : 0).ThenByDescending(x => x.ID).ToList();
                return data;
            }
            catch (Exception)
            {
                return new List<ItemHomeModel>();
            }

        }
        public List<ItemHomeModel> GetListItemFlashSale()
        {
            try
            {
                var data = (from i in cnn.Items
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            join its in cnn.ItemsSales on i.ID equals its.ItemID
                            where i.IsActive.Equals(SystemParam.ACTIVE) && its.IsActive.Equals(SystemParam.ACTIVE) && i.Status.Equals(SystemParam.ACTIVE) && its.Remain > 0 && its.FromDate < DateTime.Now && its.ToDate > DateTime.Now
                            && g.IsActive.Equals(SystemParam.ACTIVE) && g.Status.Equals(SystemParam.ACTIVE)
                            orderby its.ID descending
                            select new ItemHomeModel
                            {
                                ID = i.ID,
                                Name = i.Name,
                                ImageUrl = i.ImageUrl,
                                Price = i.Price,
                                PriceSale = its.Price,
                                SalePercent = (i.Price - its.Price) * 100 / i.Price,
                                IsHot = i.IsHot
                            }).AsEnumerable().Select(x => new ItemHomeModel
                            {
                                ID = x.ID,
                                ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault(),
                                Price = x.Price,
                                Name = x.Name,
                                PriceSale = x.PriceSale,
                                SalePercent = x.SalePercent,
                                IsHot = x.IsHot
                            }).ToList();
                return data;
            }
            catch (Exception)
            {
                return new List<ItemHomeModel>();
            }

        }

        public Boolean CheckExistingItem(string itemCode)
        {
            try
            {
                var item = cnn.Items.Where(u => u.Code.Equals(itemCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (item != null && item.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public int CreateItem(int CategoryID, int? Special, string Code, string Name, string Price, string ImageUrl,/* string Tech,*/  string Note, int IsHot)
        {
            try
            {
                if (cnn.Items.Count(x => x.Code == Code && x.IsActive == SystemParam.ACTIVE) > 0)
                    return SystemParam.EXISTING;
                Item item = new Item();
                item.GroupItemID = CategoryID;
                item.Code = Code;
                item.Name = Name;
                item.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                if (ImageUrl != null)
                {
                    var img = ImageUrl.Split(',').ToList();
                    var imgSlice = "";
                    for (int i = 0; i < img.Count; i++)
                    {
                        var index = img[i].IndexOf("files/");
                        imgSlice += (img[i].Substring(index + 6) + ",");
                    }

                    item.ImageUrl = imgSlice.TrimEnd(',');
                }
                //item.ImageUrl = ImageUrl;
                item.Description = Note;
                item.CreateDate = DateTime.Now;
                item.IsActive = SystemParam.ACTIVE;
                item.IsHot = IsHot;
                item.Status = SystemParam.ACTIVE;
                //item.Technical = Tech;
                cnn.Items.Add(item);
                cnn.SaveChanges();


                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public CreateItemInputModel LoadItem(int ID)
        {
            try
            {
                var obj = cnn.Items.Find(ID);
                CreateItemInputModel Item = new CreateItemInputModel();
                Item.ID = obj.ID;
                Item.Code = obj.Code;
                Item.Name = obj.Name;
                Item.CategoryID = obj.GroupItemID;
                Item.Status = obj.Status;
                Item.IsHot = obj.IsHot;
                Item.Price = obj.Price.ToString();
                //Item.ImageUrl = fullUrl + obj.ImageUrl;
                if (obj.ImageUrl != null)
                {
                    var img = obj.ImageUrl.Split(',').ToList();
                    var imgSlice = "";
                    for (int i = 0; i < img.Count; i++)
                    {
                        var index = img[i].IndexOf("files/");
                        imgSlice += fullUrl + (img[i] + ",");
                    }
                    Item.ImageUrl = imgSlice.TrimEnd(',');
                }
                Item.Note = obj.Description;
                Item.Technical = obj.Technical;
                return Item;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new CreateItemInputModel();
            }
        }


        //Lưu lại cập nhật sản phẩm
        public int SaveEditItem(int ID,/* int? Special,*/ string Code, string Name, int? Status, int? IsHot, int? CategoryID, string ImageUrl, string Note,/*string Tech,*/ string Price)
        {
            try
            {
                Item item = cnn.Items.Find(ID);
                GroupItem gri = cnn.GroupItems.Find(CategoryID);
                if (gri.Status == 0 && CategoryID != item.GroupItemID)
                {
                    return SystemParam.EDIT_ITEM;
                }
                else
                {
                    item.Code = Code;
                    item.Name = Name.Trim();
                    item.GroupItemID = CategoryID.Value;
                    item.Status = Status.Value;
                    item.IsHot = IsHot.Value;
                    item.Price = Convert.ToInt32((Price).ToString().Replace(",", ""));
                    if (ImageUrl != null)
                    {
                        var img = ImageUrl.Split(',').ToList();
                        var imgSlice = "";
                        for (int i = 0; i < img.Count; i++)
                        {
                            var index = img[i].IndexOf("files/");
                            imgSlice += (img[i].Substring(index + 6) + ",");
                        }
                        item.ImageUrl = imgSlice.TrimEnd(',');
                    }
                    item.Description = Note.Trim();
                    //item.Technical = Tech.Trim();
                    cnn.SaveChanges();
                    return SystemParam.RETURN_TRUE;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public string countItem()
        {
            var listItem = cnn.Items.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            int activeItem = listItem.Where(u => u.Status.Equals(SystemParam.STATUS_ACTIVE)).Count();
            int countItem = listItem.Count();
            return "" + activeItem + " / " + countItem;
        }
        public int DeleteItem(int ID)
        {
            try
            {

                Item item = cnn.Items.Find(ID);
                var itemSale = cnn.ItemsSales.Where(x => x.IsActive == SystemParam.ACTIVE && x.ItemID == ID).FirstOrDefault();
                if (itemSale != null)
                {
                    ItemsSale itm = cnn.ItemsSales.Find(itemSale.ID);
                    itm.IsActive = SystemParam.NO_ACTIVE_DELETE;
                }
                item.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public List<ListItemWebModel> SearchWeb(int Page, DateTime? fromDate, DateTime? toDate, string itemName, string category, int? Status, int? IsHot)
        {
            try
            {
                if (category == "1")
                {
                    category = null;
                }
                if (toDate.HasValue)
                    toDate = toDate.Value.AddDays(1);
                    //toDate = toDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<ListItemWebModel> lstItemOutput = new List<ListItemWebModel>();
                var query = from i in cnn.Items
                            join g in cnn.GroupItems on i.GroupItemID equals g.ID
                            where i.IsActive.Equals(SystemParam.ACTIVE)
                              && (fromDate.HasValue ? i.CreateDate >= fromDate.Value : true)
                              && (toDate.HasValue ? i.CreateDate <= toDate.Value : true)
                              && (toDate.HasValue ? i.CreateDate <= toDate.Value : true)
                              && (Status != null ? i.Status == Status : true)
                              && (IsHot != null ? i.IsHot == IsHot : true)
                            orderby i.ID descending
                            select new ListItemWebModel
                            {
                                ID = i.ID,
                                Name = g.Name,
                                ItemCode = i.Code,
                                ItemName = i.Name,
                                ImgUrl = fullUrl + i.ImageUrl,
                                Technical = i.Technical,
                                Description = i.Description,
                                Category = i.GroupItem != null ? i.GroupItem.Name : "Chưa được cập nhật",
                                ItemStatus = i.Status,
                                Price = i.Price,
                                CreateDate = i.CreateDate,
                                IsActive = i.IsActive,
                                Status = i.Status,
                                IsHot = i.IsHot,
                            };
                //if(category != null)
                //{
                //    lstItem = lstItem.Where(u => u.Category.Equals(category.Value));
                //}

                lstItemOutput = query.ToList();

                if (!String.IsNullOrEmpty(itemName))
                    lstItemOutput = lstItemOutput.Where(u => Util.Converts(u.ItemName.ToLower()).Contains(Util.Converts(itemName.ToLower())) || u.ItemCode.ToLower().Contains(itemName.ToLower())).ToList();
                if (category != null)
                {
                    lstItemOutput = lstItemOutput.Where(u => u.Category.Contains(category)).ToList();
                }

                return lstItemOutput;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListItemWebModel>();
            }
        }

    }
}
