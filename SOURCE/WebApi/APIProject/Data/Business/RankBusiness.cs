using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class RankBusiness : GenericBusiness
    {
        public RankBusiness(MrFishEntities context = null) : base()
        {
        }

        public List<Ranking> LoadRank()
        {
            return cnn.Rankings.Where(p => p.IsActive == SystemParam.ACTIVE).ToList();
        }
        /*public int AddRank(int Level,string RankName, string Descriptions, int MinPoint, int MaxPoint)
        {
            Ranking newRank = new Ranking();
            newRank.Level = Level;
            newRank.RankName = RankName;
            newRank.Descriptions = Descriptions;
            newRank.MinPoint = MinPoint;
            newRank.MaxPoint = MaxPoint;
            newRank.IsActive = SystemParam.ACTIVE;
            newRank.CraeteDate = DateTime.Now;
            cnn.Rankings.Add(newRank);
            cnn.SaveChanges();
            return SystemParam.RETURN_TRUE;
        }*/

        public Ranking getRankByLever(int lv)
        {
            return cnn.Rankings.SingleOrDefault(x => x.Level == lv);
        }

        public Ranking ShowEditRank(int ID)
        {
            return cnn.Rankings.Find(ID);
        }


        /*public int DeleteRank(int ID)
        {
            try
            {
                var cusDel = cnn.Rankings.Find(ID);
                cusDel.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception)
            {
                return SystemParam.RETURN_FALSE;
            }
        }*/
    }
}
