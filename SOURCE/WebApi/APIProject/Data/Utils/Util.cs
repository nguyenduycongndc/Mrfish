using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Web;

namespace Data.Utils
{
    public class Util
    {

        public static object Users { get; private set; }

        public static int? checkTokenApp(string tocken)
        {
            MrFishEntities cnn = new MrFishEntities();
            Customer customer = cnn.Customers.Where(u => u.Token.Equals(tocken) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (customer != null)
            {
                return customer.ID;
            }
            else
                return null;

        }
        public static string getFullUrl()
        {
            if (HttpContext.Current == null)
                return "";
            string url = "http://" + HttpContext.Current.Request.Url.Authority + "/Uploads/files/";
            return url;
        }



        public static double minPointAccount()
        {
            MrFishEntities cnn = new MrFishEntities();
            return cnn.Configs.Where(u => u.Key.Equals(SystemParam.MIN_POINT)).FirstOrDefault().Value;
        }

        public static double maxPointPerDay()
        {
            MrFishEntities cnn = new MrFishEntities();
            return cnn.Configs.Where(u => u.Key.Equals(SystemParam.MAX_POINT_PER_DAY)).FirstOrDefault().Value;
        }

        // check quận/huyện có nằm trong tỉnh/thành?
        public static bool checkDistrictInProvince(int provinceID, int districtID, int wardID)
        {
            MrFishEntities cnn = new MrFishEntities();
            District districts = cnn.Districts.Find(districtID);
            Ward ward = cnn.Wards.Find(wardID);
            if (districts == null || ward == null) return false;
            if (districts.ProvinceCode != provinceID || ward.District_id != districtID )
                return false;
            return true;
        }

        public static bool validPhone(string phone)
        {
            return Regex.Match(phone, @"^0[1-9]{1}[0-9]{8}$").Success;
        }

        public static bool validNumber(string number)
        {
            // \d bắt buộc là số, dấu + bắt buộc xuất hiện 1 lần
            return Regex.Match(number, @"^[\d]+$").Success;
        }

        //public static int? checkTokenWeb(string tocken)
        //{
        //    User users = Connect.Users.Where(u => u.Token.Equals(tocken) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
        //    if (users != null)
        //    {
        //        return users.UserID;
        //    }
        //    else
        //        return null;
        //}

        public static string CreateMD5(string input)
        {
            //bam du lieu
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        // Create token by jwt
        public static string GenerateJwt(Customer customer)
        {
            string key = SystemParam.JWT_KEY; 
            var issuer = SystemParam.JWT_ISSUER; 
            var exp = SystemParam.JWT_EXPIES; 

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Sub, customer.ID.ToString()));
            permClaims.Add(new Claim(ClaimTypes.Name, customer.Phone));
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim(ClaimTypes.NameIdentifier, customer.ID.ToString()));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddMinutes(exp), // Thời gian hết hạn token theo phút
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt_token;
        }

        public static string CheckNullString(string input)
        {
            string output = "";
            try
            {
                output = input.ToString();
            }
            catch
            {

            }
            return output;
        }
        private static readonly string[] VietNamChar = new string[]
{
        "aAeEoOuUiIdDyY",
        "áàạảãâấầậẩẫăắằặẳẵ",
        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
        "éèẹẻẽêếềệểễ",
        "ÉÈẸẺẼÊẾỀỆỂỄ",
        "óòọỏõôốồộổỗơớờợởỡ",
        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
        "úùụủũưứừựửữ",
        "ÚÙỤỦŨƯỨỪỰỬỮ",
        "íìịỉĩ",
        "ÍÌỊỈĨ",
        "đ",
        "Đ",
        "ýỳỵỷỹ",
        "ÝỲỴỶỸ"
};
        public static string Converts(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }
        public static string ConvertCurrency(long Price)
        {
            return Price.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("is"));
        }
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString().ToUpper();
        }
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public static string ConvertPhone(string phonenumber)
        {
            if (phonenumber.Contains("+84"))
            {
                int length = phonenumber.Length - 3;
                phonenumber = "0" + phonenumber.Substring(3, length);
            }
            return phonenumber;
        }

        /// <summary>
        /// Code SeriCard
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Code(string text)
        {
            char[] charArr = text.ToCharArray();
            byte[] input = Encoding.ASCII.GetBytes(charArr);
            List<string> lsoutput = new List<string>();
            foreach (byte by in input)
            {
                int value = (int)((by * SystemParam.KeyA) % SystemParam.KeyB + SystemParam.KeyC);
                string valuestr = Encoding.ASCII.GetString(new byte[] { (byte)value });
                lsoutput.Add(valuestr);
            }
            int balance1 = (int)((48 * SystemParam.KeyA) / SystemParam.KeyB);
            int balance2 = (int)((57 * SystemParam.KeyA) / SystemParam.KeyB);
            lsoutput.Add("!" + balance1.ToString());
            lsoutput.Add("!" + balance2.ToString());
            string output = DateTime.Now.ToString("HHyyyyssddmmMM!");
            foreach (string o in lsoutput)
            {
                output += o;
            }
            return output;
        }
        /// <summary>
        /// EnCode Card   
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EnCode(string text)
        {
            string[] lsString = text.Split('!');
            string inputstr = lsString[lsString.Length - 3];
            int balance1 = int.Parse(lsString[lsString.Length - 2]);
            int balance2 = int.Parse(lsString[lsString.Length - 1]);
            char[] charArr = inputstr.ToCharArray();
            byte[] input = Encoding.ASCII.GetBytes(charArr);
            string lsoutput = "";
            foreach (byte c in input)
            {
                float value = (c - SystemParam.KeyC + SystemParam.KeyB * balance1) / SystemParam.KeyA;
                if (value < 48 || value > 57 || value != (int)value)
                    value = (c - SystemParam.KeyC + SystemParam.KeyB * balance2) / SystemParam.KeyA;
                string output = Encoding.ASCII.GetString(new byte[] { (byte)value });
                lsoutput += output;
            }
            return lsoutput;
        }


        //Convert Datetime 
        public static Nullable<DateTime> ConvertDate(string date, string datepaser = SystemParam.CONVERT_DATETIME)
        {
            try
            {
                if (!String.IsNullOrEmpty(date))
                    return DateTime.ParseExact(date, datepaser, null);
                return null;
            }
            catch
            {
                return null;
            }

        }
        //Convert Datetime 
        public static Nullable<DateTime> ConvertDate1(string date, string datepaser = SystemParam.CONVERT_DATETIME1)
        {
            try
            {
                if (!String.IsNullOrEmpty(date))
                    return DateTime.ParseExact(date, datepaser, null);
                return null;
            }
            catch
            {
                return null;
            }

        }

        //get name TYPE_ADD 
        public static string GetNameType(int ID)
        {
            string result = "";
            switch (ID)
            {
                case 1: result = "Tích điểm/Bảo hành"; break;
                case 2: result = "Tặng điểm"; break;
                case 3: result = "Được tặng điểm"; break;
                case 4: result = "Đổi quà"; break;
                case 5: result = "Hệ thống cộng điểm"; break;
                case 6: result = "Đổi thẻ"; break;
                case 7: result = "Hủy yêu cầu đổi quà"; break;
            }
            return result;
        }

        public static string GetNameStatusWarranty(int Status)
        {
            string result = "";
            switch (Status)
            {
                case 1: result = "Đã tích điểm"; break;
                case 2: result = "Chưa tích điểm"; break;
            }
            return result;
        }
        public static Bitmap CropImage(Image source, int x, int y, int width, int height)
        {
            Rectangle crop = new Rectangle(x, y, width, height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public static string GenPass(string pass)
        {
            return BCrypt.Net.BCrypt.HashPassword(pass, 10);
        }

        public static bool CheckPass(string pass, string userPass)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(pass, userPass);
            }
            catch
            {
                return false;
            }
        }
    }

}
