using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class CustomerDetailOutputModel
    {
        public int ID { get; set; }
        public double Point { get; set; }
        public int TypeLogin { get; set; }
        public string Phone { set; get; }
        public string CustomerName { set; get; }
        public DateTime? DOB { set; get; }
        public string DOBStr { get; set; }
        public int Sex { set; get; }
        public string Email { set; get; }
        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public string WardName { get; set; }
        public string Address { set; get; }
        public int? ProvinceID { set; get; }
        public int? DistrictID { set; get; }
        public int? WardID { get; set; }
        public string UrlAvatar { get; set; }
        //public int Remain { get; set; }
        public int Role { get; set; }
    }

    public class ListCustomerOutPutModel
    {
        public int limit { get; set; }
        public int totalCount { get; set; }
        public int page { get; set; }
        public int totalPage { get; set; }
        public int sumCustomer { get; set; }
        public IPagedList<CustomerDetailOutputModel> listCustomer { get; set; }
    }

    public class GetUser
    {
        public UserInforOutputModel UserInfo { get; set; }

    }
}