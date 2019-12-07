using ResultatPartnerAS.Model;
using ResultatPartnerASAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPartnerASAPI.BL
{
    public class Utility
    {
        public static double ToDouble(object val)
        {
            try
            {
                return Convert.ToDouble(val);
            }
            catch
            {
                return 0;
            }
        }
        public static long ToLong(object val)
        {
            try
            {
                return Convert.ToInt64(val);
            }
            catch
            {
                return 0;
            }
        }
        public static int ToInt(object val)
        {
            try
            {
                return Convert.ToInt32(val);
            }
            catch
            {
                return 0;
            }
        }
        public static string Tostring(object val)
        {
            try
            {
                return Convert.ToString(val);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetRequestToken(HttpRequestMessage request)
        {
            string Token = string.Empty;
            try
            {
                var Headers = request.Headers;
                if (Headers.Contains("TOKEN"))
                {
                    // Get token from request header
                    Token = Headers.GetValues("TOKEN").First().ToString();
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return Token;
        }

        public static long GetUserIDByToken(string Token)
        {
            long UserID = 0;
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    UserToken dbUserToken = dbContext.UserTokens.Where(x => x.Token == Token).FirstOrDefault();
                    if (dbUserToken != null && dbUserToken.UserID > 0)
                    {
                        UserID = Utility.ToLong(dbUserToken.UserID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return UserID;
        }
    }
}
