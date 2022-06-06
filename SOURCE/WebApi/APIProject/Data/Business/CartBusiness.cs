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
    public class CartBusiness :GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        public CartBusiness(MrFishEntities context = null) : base()
        {
        }
        public bool CheckCartChange(int CusID)
        {
            var listCart = cnn.Carts.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            var check = false;
            foreach(var cart in listCart)
            {
                var item = cnn.Items.FirstOrDefault(x => x.ID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE) && x.GroupItem.IsActive.Equals(SystemParam.ACTIVE) && x.GroupItem.Status.Equals(SystemParam.ACTIVE));
                if (item == null)
                {
                    cart.IsActive = SystemParam.ACTIVE_FALSE;
                    check = true;
                    cnn.SaveChanges();
                    continue;
                }
                var priceSale = cnn.ItemsSales.Where(x => x.ItemID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault();
                var quantitySale = cnn.ItemsSales.Where(x => x.ItemID.Equals(cart.ItemID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Remain).FirstOrDefault();
                if (cart.Price != item.Price || cart.PriceSale != priceSale || (cart.QuantitySale != quantitySale && cart.Quantity > quantitySale))
                {
                    check = true;
                }
                cart.Price = item.Price;
                cart.PriceSale = priceSale;
                cart.QuantitySale = quantitySale;
                cnn.SaveChanges();
            }
            return check;
        }
        public int GetCartCount(int CusID)
        {
            try
            {
                return cnn.Carts.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.CustomerID.Equals(CusID) && x.Item.IsActive.Equals(SystemParam.ACTIVE) 
                                            && x.Item.Status.Equals(SystemParam.ACTIVE) && x.Item.GroupItem.IsActive.Equals(SystemParam.ACTIVE) && x.Item.GroupItem.Status.Equals(SystemParam.ACTIVE)).Sum(x => x.Quantity);           
            }catch(Exception ex)
            {
                ex.ToString();
                return 0;
            }
        }
        public List<CartModel> GetListCart(int CusID)
        {
            try
            {

                var model = (from c in cnn.Carts
                             join i in cnn.Items on c.ItemID equals i.ID
                             where c.CustomerID.Equals(CusID) && c.IsActive.Equals(SystemParam.ACTIVE)
                             select new CartModel
                             {
                                 ID = c.ID,
                                 ImageUrl = i.ImageUrl,
                                 ProductID = i.ID,
                                 Price = i.Price,
                                 PriceSale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                                 Quantity = c.Quantity,
                                 QuantitySale = i.ItemsSales.Where(x => x.ItemID.Equals(i.ID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Remain).FirstOrDefault(),
                                 ProductName = i.Name,
                             }).AsEnumerable().Select(x => new CartModel
                             {
                                 ID = x.ID,
                                 ProductID = x.ProductID,
                                 ImageUrl = fullUrl + x.ImageUrl.Split(',').FirstOrDefault(),
                                 Price = x.Price,
                                 ProductName = x.ProductName,
                                 PriceSale = x.PriceSale,
                                 Quantity = x.Quantity,
                                 QuantitySale = x.QuantitySale
                             }).ToList();
                return model;
            }catch(Exception e)
            {
                e.ToString();
                return new List<CartModel>();
            }
        }
        public int AddCart(AddCartModel cart,int CusID)
        {
            try
            {
                var item = cnn.Items.FirstOrDefault(x => x.ID.Equals(cart.ProductID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.Status.Equals(SystemParam.ACTIVE) && x.GroupItem.IsActive.Equals(SystemParam.ACTIVE) && x.GroupItem.Status.Equals(SystemParam.ACTIVE));
                if(item == null)
                {
                    return SystemParam.ERROR_ITEM_NOT_FOUND;
                }
                var listCart = cnn.Carts.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => x.ItemID).ToList();
                // TH Sản phẩm không tồn tại trong giỏ hàng
                if (!listCart.Contains(item.ID))
                {
                    Cart cartNew = new Cart
                    {
                        CustomerID = CusID,
                        ItemID = item.ID,
                        Quantity = cart.Quantity,
                        Price = item.Price,
                        PriceSale = item.ItemsSales.Where(x =>  x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Price).FirstOrDefault(),
                        QuantitySale = item.ItemsSales.Where(x =>  x.IsActive.Equals(SystemParam.ACTIVE) && x.Remain > 0 && x.FromDate < DateTime.Now && x.ToDate > DateTime.Now).Select(x => x.Remain).FirstOrDefault(),
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now
                    };
                    cnn.Carts.Add(cartNew);
                    cnn.SaveChanges();
                }
                // TH Sản phẩm đã tồn tại trong giỏ hàng
                else
                {
                    var oldCart = cnn.Carts.FirstOrDefault(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.ItemID.Equals(item.ID));
                    oldCart.Quantity += cart.Quantity;
                    cnn.SaveChanges();
                }
                return SystemParam.SUCCESS;
            }catch(Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int UpdateCart(UpdateCartModel cart)
        {
            try
            {
                var oldCart = cnn.Carts.FirstOrDefault(x => x.ID.Equals(cart.CartID) && x.IsActive.Equals(SystemParam.ACTIVE));
                if(oldCart == null)
                {
                    return SystemParam.ERROR_CART_UPDATED;
                }
                oldCart.Quantity = cart.Quantity;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int DeleteCart(DeleteCartModel cart)
        {
            try
            {
                if(cart.CartID != null)
                {
                    foreach(var item in cart.CartID)
                    {
                        var cartDelete = cnn.Carts.FirstOrDefault(x => x.ID.Equals(item));
                        cartDelete.IsActive = SystemParam.ACTIVE_FALSE;
                        cnn.SaveChanges();
                    }
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
    }
}
