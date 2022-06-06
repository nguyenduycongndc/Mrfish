using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class MemberPointHistoryBusiness : GenericBusiness
    {
        public MemberPointHistoryBusiness(MrFishEntities context = null) : base()
        {
        }
        public IPagedList<MemberPointHistoryModel> GetListMemberPointHistory(int page,int limit,string fromDate,string toDate , int CusID)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = (from mph in cnn.MembersPointHistories
                            where mph.IsActive.Equals(SystemParam.ACTIVE) && mph.CustomerID.Value.Equals(CusID)
                            && (fd.HasValue ? mph.CreateDate >= fd.Value : true)
                            && (td.HasValue ? mph.CreateDate <= td.Value : true)
                            select new MemberPointHistoryModel
                            {
                                ID = mph.ID,
                                CustomerID = mph.CustomerID,
                                Point = mph.Point,
                                OrderCode = mph.AddPointCode,
                                Balance = mph.Balance.HasValue ? mph.Balance.Value : 0,
                                CreateDate = mph.CreateDate,
                                OrderID = mph.OrderID
                            }).OrderByDescending(x => x.ID).ToPagedList(page, limit);
                return data;
            }
            catch(Exception ex)
            {
                ex.ToString();
                return new List<MemberPointHistoryModel>().ToPagedList(1, 1);
            }
           
        }
    }
}
