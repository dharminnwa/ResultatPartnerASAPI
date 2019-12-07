using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultatPartnerAS.IBLL;
using System.Configuration;
using ResultatPartnerAS.Model;
using ResultatPartnerASAPI.Data;

namespace ResultatPartnerASAPI.BL
{
    public class UserManager : IUser
    {
        public UserModel LoginUser(UserModel model)
        {
            UserModel user = new UserModel();
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    User dbUser = dbContext.Users.Where(x => x.UserName == model.UserName && x.Password == model.Password).FirstOrDefault();
                    if (dbUser != null && dbUser.UserID > 0)
                    {
                        user.UserName = dbUser.UserName;
                        user.UserRole = dbUser.UserRole.Role;

                        // Generate Token and save in db with Expiry value
                        double TokenExpiry = Utility.ToDouble(ConfigurationManager.AppSettings["TokenExpiry"].ToString());
                        if (TokenExpiry == 0)
                            TokenExpiry = 1;
                        UserToken dbToken = new UserToken();
                        dbToken.UserID = dbUser.UserID;
                        dbToken.Token = Guid.NewGuid().ToString();
                        dbToken.TokenCreatedDate = DateTime.Now;
                        dbToken.TokenExpiryDate = DateTime.Now.AddHours(TokenExpiry);
                        dbToken.CreatedDate = DateTime.Now;
                        dbToken.UpdatedDate = DateTime.Now;
                        dbContext.UserTokens.Add(dbToken);
                        dbContext.SaveChanges();
                        user.Token = dbToken.Token;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return user;
        }

        public bool LogoutUser(string Token)
        {
            bool isLoggedOut = false;
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    UserToken dbUserToken = dbContext.UserTokens.Where(x => x.Token == Token).FirstOrDefault();
                    if (dbUserToken != null && dbUserToken.UserTokenID > 0)
                    {
                        dbContext.UserTokens.Remove(dbUserToken);
                        dbContext.SaveChanges();
                        isLoggedOut = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return isLoggedOut;
        }
    }
}
