using Data.DB;
using Data.Model.APIWeb;
using Data.Model;
using Data.Model.APIApp;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ConfigBusiness : GenericBusiness
    {
        public string fullUrl = Util.getFullUrl();
        NotifyBusiness notiBus = new NotifyBusiness();
        PushNotifyBusiness pushNotiBus = new PushNotifyBusiness();
        public ConfigBusiness(MrFishEntities context = null) : base()
        {
        }
        public DetailConfigModel BindingConfig()
        {
            var listConfig = cnn.Configs.ToList();
            var percentage = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE));
            //var desContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO));
            //var contactI = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO));
            if (percentage != null/* && desContactInfo != null && contactI != null*/)
            {
                var data = new DetailConfigModel
                {
                    KeyPercentage = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE)).Key,
                    ValuePercentage = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE)).Value,
                    TextPercentage = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE)).Text,
                    //KeyDescriptionContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO)).Key,
                    //ValueDescriptionContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO)).Value,
                    //TextDescriptionContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO)).Text,
                    //KeyContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO)).Key,
                    //ValueContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO)).Value,
                    //TextContactInfo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO)).Text,
                };
                return data;
            }
            else
            {
                return null;
            }
        }
        public List<RankNewModel> ListRankNew()
        {
            var listConfig = cnn.Configs.ToList();
            var query = from rank in cnn.Rankings
                        where rank.IsActive.Equals(SystemParam.ACTIVE)
                        orderby rank.ID descending
                        select new RankNewModel
                        {
                            ID = rank.ID,
                            RankName = rank.RankName,
                            DiscountPercent = rank.DiscountPercent,
                            Descriptions = rank.Descriptions,
                            CreateDate = rank.CreateDate,
                            IsActive = rank.IsActive,
                            MinPoint = rank.MinPoint,
                            MaxPoint = rank.MaxPoint,
                            Level = rank.Level,
                        };
            var data = query.ToList();
            return data;
        }
        public ListContactInfoNewModel listContactInfoTable()
        {
            var listConfig = cnn.Configs.ToList();
            var desContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO));
            var contactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO));
            var desContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS));
            var contactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS));
            var desContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE));
            var contactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE));
            var data = new ListContactInfoNewModel
            {
                KeyDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Key,
                ValueDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Value,
                TextDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Text,

                KeyDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Key,
                ValueDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Value,
                TextDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Text,

                KeyDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Key,
                ValueDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Value,
                TextDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Text,

                KeyContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Key,
                ValueContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Value,
                TextContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Text,

                KeyContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Key,
                ValueContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Value,
                TextContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Text,

                KeyContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Key,
                ValueContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Value,
                TextContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Text,
            };


            return data;
        }
        public int saveEditConfig(double valuePoint/*, string descriptionPoint, string contactInfo, string descriptionContact*/)
        {
            try
            {
                if(valuePoint > 100) return SystemParam.CHECK_PERCENT;
                var list = cnn.Configs.ToList();
                var percentage = list.FirstOrDefault(x => x.Key.Equals(SystemParam.PERCENTAGE));
                //var desContactInfo = list.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO));
                //var contactI = list.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO));

                if (percentage != null/* && desContactInfo != null && contactI != null*/)
                {
                    percentage.Value = valuePoint;
                    //percentage.Text = descriptionPoint;
                    //desContactInfo.Text = descriptionContact;
                    //contactI.Text = contactInfo;
                    cnn.SaveChanges();
                    return SystemParam.RETURN_TRUE;
                }
                else
                    return SystemParam.RETURN_FALSE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int saveEditRank(int ID, string MaxPoint, string DiscountPercent, string Descriptions)
        {
            try
            {
                Ranking rank = cnn.Rankings.Find(ID);
                var _maxPoint = Convert.ToInt32((MaxPoint).ToString().Replace(",", ""));
                var _discountPercent = Convert.ToDouble((DiscountPercent).ToString().Replace("%", ""));
                if(_discountPercent > 100) return SystemParam.CHECK_PERCENT;
                var listRank = cnn.Rankings.ToList();

                var rankMember = listRank.FirstOrDefault(x => x.ID.Equals(1));
                var rankSilver = listRank.FirstOrDefault(x => x.ID.Equals(3));
                var rankGold = listRank.FirstOrDefault(x => x.ID.Equals(4));
                var rankDiamond = listRank.FirstOrDefault(x => x.ID.Equals(5));


                if (ID == 1)
                {
                    if (_maxPoint >= rankSilver.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint >= rankGold.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint >= rankDiamond.MaxPoint) return SystemParam.POINT_CONFIG;

                    if (_discountPercent >= rankSilver.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent >= rankGold.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent >= rankDiamond.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                }
                else if (ID == 3)
                {
                    if (_maxPoint <= rankMember.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint >= rankGold.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint >= rankDiamond.MaxPoint) return SystemParam.POINT_CONFIG;

                    if (_discountPercent <= rankMember.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent >= rankGold.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent >= rankDiamond.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                }
                else if (ID == 4)
                {
                    if (_maxPoint <= rankMember.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint <= rankSilver.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint >= rankDiamond.MaxPoint) return SystemParam.POINT_CONFIG;

                    if (_discountPercent <= rankMember.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent <= rankSilver.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent >= rankDiamond.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                }
                else if (ID == 5)
                {
                    if (_maxPoint <= rankMember.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint <= rankSilver.MaxPoint) return SystemParam.POINT_CONFIG;
                    if (_maxPoint <= rankGold.MaxPoint) return SystemParam.POINT_CONFIG;

                    if (_discountPercent <= rankMember.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent <= rankSilver.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                    if (_discountPercent <= rankGold.DiscountPercent) return SystemParam.PERCENT_CONFIG;
                }
                rank.MaxPoint = _maxPoint;
                rank.DiscountPercent = _discountPercent;
                rank.Descriptions = Descriptions;
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int saveEditContactInfo(int key, string TextContactInfoZalo, string TextDescriptionContactInfoZalo, string TextContactInfoMess, string TextDescriptionContactInfoMess, string TextContactInfoHotline, string TextDescriptionContactInfoHotline)
        {
            try
            {
                var listConfig = cnn.Configs.ToList();
                var descriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO));
                var contactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO));

                var descriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS));
                var contactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS));

                var descriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE));
                var contactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE));
                if (key.Equals(SystemParam.ID_ZALO))
                {
                    contactInfoZalo.Text = TextContactInfoZalo;
                    descriptionContactInfoZalo.Text = TextDescriptionContactInfoZalo;
                }
                else if (key.Equals(SystemParam.ID_MESS))
                {
                    descriptionContactInfoMess.Text = TextDescriptionContactInfoMess;
                    contactInfoMess.Text = TextContactInfoMess;
                }
                else if (key.Equals(SystemParam.ID_HOTLINE))
                {
                    descriptionContactInfoHotline.Text = TextDescriptionContactInfoHotline;
                    contactInfoHotline.Text = TextContactInfoHotline;
                }

                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public RankEditNewModel ModalEditRank(int ID)
        {
            try
            {
                var obj = cnn.Rankings.Find(ID);
                RankEditNewModel detailRank = new RankEditNewModel();
                detailRank.ID = obj.ID;
                detailRank.RankName = obj.RankName;
                detailRank.DiscountPercent = obj.DiscountPercent;
                detailRank.MaxPoint = obj.MaxPoint.ToString();
                detailRank.MinPoint = obj.MinPoint.ToString();
                detailRank.Descriptions = obj.Descriptions;
                return detailRank;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new RankEditNewModel();
            }
        }
        public ListContactInfoNewModel loadContactInfo(int keycheck)
        {
            try
            {
                var listConfig = cnn.Configs.ToList();
                ListContactInfoNewModel detailContactInfo = new ListContactInfoNewModel();
                if (keycheck.Equals(SystemParam.ID_ZALO))
                {
                    detailContactInfo.KeyPub = SystemParam.STR_ZALO;
                    detailContactInfo.KeyID = SystemParam.ID_ZALO;
                    detailContactInfo.KeyDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Key;
                    detailContactInfo.ValueDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Value;
                    detailContactInfo.TextDescriptionContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_ZALO)).Text;

                    detailContactInfo.KeyContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Key;
                    detailContactInfo.ValueContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Value;
                    detailContactInfo.TextContactInfoZalo = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_ZALO)).Text;
                }
                else if (keycheck.Equals(SystemParam.ID_MESS))
                {
                    detailContactInfo.KeyPub = SystemParam.STR_MESS;
                    detailContactInfo.KeyID = SystemParam.ID_MESS;
                    detailContactInfo.KeyDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Key;
                    detailContactInfo.ValueDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Value;
                    detailContactInfo.TextDescriptionContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_MESS)).Text;

                    detailContactInfo.KeyContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Key;
                    detailContactInfo.ValueContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Value;
                    detailContactInfo.TextContactInfoMess = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_MESS)).Text;
                }
                else if (keycheck.Equals(SystemParam.ID_HOTLINE))
                {
                    detailContactInfo.KeyPub = SystemParam.STR_HOTLINE;
                    detailContactInfo.KeyID = SystemParam.ID_HOTLINE;
                    detailContactInfo.KeyDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Key;
                    detailContactInfo.ValueDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Value;
                    detailContactInfo.TextDescriptionContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.DESCRIPTIONCONTACTINFO_HOTLINE)).Text;

                    detailContactInfo.KeyContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Key;
                    detailContactInfo.ValueContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Value;
                    detailContactInfo.TextContactInfoHotline = listConfig.FirstOrDefault(x => x.Key.Equals(SystemParam.CONTACTINFO_HOTLINE)).Text;
                }
                return detailContactInfo;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListContactInfoNewModel();
            }
        }
    }
}
