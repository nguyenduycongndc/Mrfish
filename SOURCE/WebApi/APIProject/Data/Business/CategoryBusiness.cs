using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using System.IO;
using System.Web;
using OfficeOpenXml;
//using QRCoder;
using System.Drawing;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Data.Model.APIApp;

namespace Data.Business
{
    public class CategoryBusiness
    {
        public CategoryBusiness(MrFishEntities context = null) : base()
        {
        }
        public string fullUrl = Util.getFullUrl();

        public List<ListCategoryOutputModel> loadGroupItems()
        {
            MrFishEntities cnn = new MrFishEntities();
            var query = from p in cnn.GroupItems
                        where p.ParentID == null && p.IsActive == 1
                        orderby p.Name
                        select new ListCategoryOutputModel()
                        {
                            ID = p.ID,
                            ParentId = p.ParentID,
                            Name = p.Name,
                            CreateDate = p.CreatedDate,
                            ImageUrl = fullUrl + p.ImageUrl,
                            Description = p.Description
                        };
            if (query != null && query.Count() > 0)
            {
                return query.ToList();
            }
            else
            {
                return new List<ListCategoryOutputModel>();
            }
        }
        public List<ListCategoryOutputModel> loadCategorys()
        {
            MrFishEntities cnn = new MrFishEntities();

            List<ListCategoryOutputModel> list = new List<ListCategoryOutputModel>();
            var query = (from c in cnn.GroupItems
                         orderby c.Name
                         where c.IsActive == 1
                         select new ListCategoryOutputModel()
                         {
                             ID = c.ID,
                             Name = c.Name,
                             ParentId = c.ParentID,
                             CreateDate = c.CreatedDate,
                             ImageUrl = c.ImageUrl
                             
                         }).ToList();
            if (query != null && query.Count() > 0)
            {

                foreach (var value in query)
                {
                    if (value.ParentId != null)
                    {
                        // lấy thông tin của danh mục cha 
                        var parent = query.Where(u => u.ID == value.ParentId);
                        if (parent != null && parent.Count() > 0)
                        {
                            var p = parent.SingleOrDefault();
                            value.ParentName = p.Name;
                        }
                        else
                        {
                            value.ParentName = "";
                        }
                    }
                    else
                    {
                        value.ParentName = "";
                    }

                    list.Add(value);
                }
                return list;
            }
            else
            {
                return new List<ListCategoryOutputModel>();
            }
        }

        public List<ListCategoryOutputModel> Search(int Page, string CateName, int? Status, string FromDate, string ToDate/*, int? Category*/)
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {

                DateTime? startDate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1).AddHours(-12).AddMinutes(59).AddSeconds(59);

                List<ListCategoryOutputModel> list = new List<ListCategoryOutputModel>();
                
                var query = (from c in cnn.GroupItems join u in cnn.Users on c.UserID equals u.UserID
                             orderby c.CreatedDate descending
                             where c.IsActive == SystemParam.ACTIVE
                            && (startDate.HasValue ? c.CreatedDate >= startDate.Value : true)
                             && ((!String.IsNullOrEmpty(CateName) ? c.Name.Contains(CateName) : true))
                             select new ListCategoryOutputModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 Status = c.Status,
                                 ParentId = c.ParentID,
                                 CreateDate = c.CreatedDate,
                                 ImageUrl = c.ImageUrl,
                                 UserID = u.UserID,
                                 UserName = u.UserName,
                             }).ToList();
                if (endDate.HasValue)
                {
                    // query = query.Where(u => u.CreateDate.Day >= startDate.Value.Day && u.CreateDate.Month >= startDate.Value.Month && u.CreateDate.Year >= startDate.Value.Year).ToList();
                    query = query.Where(u => u.CreateDate.Day <= endDate.Value.Day && u.CreateDate.Month <= endDate.Value.Month && u.CreateDate.Year <= endDate.Value.Year).ToList();
                }
                if (Status.HasValue)
                {
                    query = query.Where(u => u.Status.Equals(Status.Value)).ToList();
                }
                if (CateName != "" && CateName != null)
                {
                    query = query.Where(u => Util.Converts(u.Name.ToLower()).Contains(Util.Converts(CateName.ToLower()))).ToList();
                }

                //if (Category != null && Category == 0)
                //{
                //    query = query.Where(u => u.ParentId == null).ToList();
                //}
                //if (Category != null && Category == 1)
                //{
                //    // query = query.Where(u => u.ParentId != null).ToList();
                //    foreach (var value in query)
                //    {
                //        if (value.ParentId != null)
                //        {
                //            // lấy thông tin của danh mục cha 
                //            var parent = query.Where(u => u.ID == value.ParentId);
                //            if (parent != null)
                //            {
                //                var p = parent.SingleOrDefault();
                //                value.ParentName = p.Name;
                //            }
                //            else
                //            {
                //                value.ParentName = "";
                //            }
                //        }
                //        else
                //        {
                //            value.ParentName = "";
                //        }

                //        list.Add(value);
                //    }

                //    return query.Where(u => u.ParentId != null).ToList();
                //}
                if (query != null && query.Count() > 0)
                {
                    foreach (var value in query)
                    {
                        if (value.ParentId != null)
                        {
                            // lấy thông tin của danh mục cha 
                            var parent = query.Where(u => u.ID == value.ParentId);
                            if (parent != null && parent.Count() > 0)
                            {
                                var p = parent.SingleOrDefault();
                                value.ParentName = p.Name;
                            }
                            else
                            {
                                value.ParentName = "";
                            }
                        }
                        else
                        {
                            value.ParentName = "";
                        }

                        list.Add(value);
                    }

                    return list;
                }

                else
                {
                    return new List<ListCategoryOutputModel>();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ListCategoryOutputModel>();
            }

        }
        public List<CategoryHomeModel> GetListCategory()
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {
                var query = (from p in cnn.GroupItems
                            where p.ParentID == null && p.IsActive.Equals(SystemParam.ACTIVE) && p.Status.Equals(SystemParam.ACTIVE)
                            orderby p.ID descending
                            select new CategoryHomeModel()
                            {
                                ID = p.ID,
                                Name = p.Name,
                                ImageUrl = fullUrl + p.ImageUrl,
                            }).ToList();
                return query;
            }catch(Exception e)
            {
                return new List<CategoryHomeModel>();
            }                    
        }

        public int createCategory( string Name, string Description, string ImageUrl, int Id)
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {
                GroupItem obj = new GroupItem();
                
                // obj.ParentID = ParentID;
                obj.Name = Name;
                obj.Status = 1;
                obj.ImageUrl = ImageUrl;
                obj.CreatedDate = DateTime.Now;
                obj.IsActive = SystemParam.ACTIVE;
                obj.Type = 1;
                obj.UserID = Id;
                obj.OrderIndex = 1;
                obj.Description = Description;
                cnn.GroupItems.Add(obj);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public ListCategoryOutputModel ModalEditCategory(int ID)
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {
                ListCategoryOutputModel categoryDetail = new ListCategoryOutputModel();
                var query = (from c in cnn.GroupItems
                             where c.IsActive == SystemParam.ACTIVE
                             && c.ID == ID
                             select new ListCategoryOutputModel
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 Status = c.Status,
                                 ParentId = c.ParentID,
                                 ImageUrl = fullUrl + c.ImageUrl,
                                 Description = c.Description
                             }).FirstOrDefault();
                if (query != null && query.ID > 0)
                {
                    return categoryDetail = query;
                }
                else
                {
                    return categoryDetail;
                }
                
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListCategoryOutputModel();
            }
        }
        public int EditCategory(ListCategoryOutputModel data)
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {
                var obj = cnn.GroupItems.Find(data.ID);
                if (data.ParentId == 0)
                {
                    obj.ParentID = null;
                }
                else
                {
                    obj.ParentID = data.ParentId;
                }
                obj.Name = data.Name;
                obj.Status = data.Status.GetValueOrDefault();
                obj.ImageUrl = data.ImageUrl;
                obj.Description = data.Description;

                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }

        public int DeleteCategory(int ID)
        {
            MrFishEntities cnn = new MrFishEntities();
            try
            {
                var checkitem = cnn.Items.Where(x => x.IsActive == SystemParam.ACTIVE && x.GroupItemID == ID).ToList();
                if (checkitem.Count() > 0) return 4;
                GroupItem category = cnn.GroupItems.Find(ID);
                List<int> item = (from i in cnn.Items
                                  where i.IsActive.Equals(SystemParam.ACTIVE)
                                  select i.GroupItemID).ToList();
                List<int> check = (from gi in cnn.GroupItems
                                   where gi.IsActive.Equals(SystemParam.ACTIVE) && (gi.ParentID.HasValue ? gi.ParentID.Value.Equals(ID) : false)
                                   select gi.ParentID.Value).ToList();

                if (check.Count > 0)
                {
                    return 3;
                }
                if (item.Contains(category.ID))
                {
                    return 4;
                }
                else
                {
                    if (category != null && category.IsActive.Equals(SystemParam.ACTIVE))
                    {
                        category.IsActive = SystemParam.ACTIVE_FALSE;
                        cnn.SaveChanges();
                        return SystemParam.RETURN_TRUE;
                    }
                    return SystemParam.RETURN_FALSE;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
    }
}
