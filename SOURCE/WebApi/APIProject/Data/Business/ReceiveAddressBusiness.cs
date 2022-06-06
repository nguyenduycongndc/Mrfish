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
    public class ReceiveAddressBusiness : GenericBusiness
    {
        public ReceiveAddressBusiness(MrFishEntities context = null) : base()
        {
        }
        public List<ReceiveAddressModel> GetListReceiveAddress(int CusID)
        {
            try
            {
                var data = cnn.ReceiveAddresses.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => new ReceiveAddressModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    DistrictID = x.DistrictID,
                    ProvinceID = x.ProvinceID,
                    WardID = x.WardID,
                    District = x.District.Name,
                    Province = x.Province.Name,
                    Ward = x.Ward.Name,
                    IsDefault = x.IsDefault
                }).ToList();
                return data;
            }catch(Exception e)
            {
                e.ToString();
                return new List<ReceiveAddressModel>();
            }
        }
        public ReceiveAddressModel GetReceiveAddressDefault(int CusID)
        {
            try
            {
                var data = cnn.ReceiveAddresses.Where(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.IsDefault.Equals(SystemParam.RECEIVE_ADDRESS_DEFAULT)).Select(x => new ReceiveAddressModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    DistrictID = x.DistrictID,
                    ProvinceID = x.ProvinceID,
                    WardID = x.WardID,
                    District = x.District.Name,
                    Province = x.Province.Name,
                    Ward = x.Ward.Name,
                    IsDefault = x.IsDefault
                }).FirstOrDefault();
                return data;
            }
            catch (Exception e)
            {
                e.ToString();
                return new ReceiveAddressModel();
            }
        }
        public ReceiveAddressModel GetReceiveAddressDetail(int ID)
        {
            try
            {
                var data = cnn.ReceiveAddresses.Where(x => x.ID.Equals(ID) && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => new ReceiveAddressModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Phone = x.Phone,
                    Address = x.Address,
                    DistrictID = x.DistrictID,
                    ProvinceID = x.ProvinceID,
                    WardID = x.WardID,
                    District = x.District.Name,
                    Province = x.Province.Name,
                    Ward = x.Ward.Name,
                    IsDefault = x.IsDefault
                }).FirstOrDefault();
                return data;
            }
            catch (Exception e)
            {
                e.ToString();
                return new ReceiveAddressModel();
            }
        }
        public int AddReceiveAddress(AddReceiveAddressModel model, int CusID)
        {
            try
            {
                var data = new ReceiveAddress
                {
                    Name = model.Name,
                    Phone = model.Phone,
                    Address = model.Address,
                    CustomerID = CusID,
                    DistrictID = model.DistrictID,
                    ProvinceID = model.ProvinceID,
                    WardID = model.WardID,
                    IsDefault = model.IsDefault,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now,
                };              
                if (model.IsDefault.Equals(SystemParam.RECEIVE_ADDRESS_DEFAULT))
                {
                    var addressDefault = cnn.ReceiveAddresses.FirstOrDefault(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.IsDefault.Equals(SystemParam.RECEIVE_ADDRESS_DEFAULT));
                    if(addressDefault != null)
                    {
                        addressDefault.IsDefault = SystemParam.RECEIVE_ADDRESS_NOT_DEFAULT;
                    }
                    cnn.SaveChanges();
                }
                cnn.ReceiveAddresses.Add(data);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int UpdateReceiveAddress(UpdateReceiveAddressModel model, int CusID)
        {
            try
            {
                var address = cnn.ReceiveAddresses.FirstOrDefault(x => x.ID.Equals(model.ID) && x.IsActive.Equals(SystemParam.ACTIVE) );
                if(address == null)
                {
                    return SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND;
                }
                address.Name = model.Name;
                address.Phone = model.Phone;
                address.Address = model.Address;
                address.DistrictID = model.DistrictID;
                address.ProvinceID = model.ProvinceID;
                address.WardID = model.WardID;
                address.IsDefault = model.IsDefault;
                if (model.IsDefault.Equals(SystemParam.RECEIVE_ADDRESS_DEFAULT))
                {
                    var addressDefault = cnn.ReceiveAddresses.FirstOrDefault(x => x.CustomerID.Equals(CusID) && x.IsActive.Equals(SystemParam.ACTIVE) && x.IsDefault.Equals(SystemParam.RECEIVE_ADDRESS_DEFAULT) && !x.ID.Equals(address.ID));
                    if (addressDefault != null)
                    {
                        addressDefault.IsDefault = SystemParam.RECEIVE_ADDRESS_NOT_DEFAULT;
                    }
                    cnn.SaveChanges();
                }
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int DeleteReceiveAddress(int ID)
        {
            try
            {
                var address = cnn.ReceiveAddresses.FirstOrDefault(x => x.ID.Equals(ID) && x.IsActive.Equals(SystemParam.ACTIVE));
                if (address == null)
                {
                    return SystemParam.ERROR_RECEIVE_ADDRESS_NOT_FOUND;
                }
                address.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
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
