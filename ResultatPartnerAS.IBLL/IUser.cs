using ResultatPartnerAS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPartnerAS.IBLL
{
    public interface IUser
    {
        UserModel LoginUser(UserModel model);
        bool LogoutUser(string Token);
    }
}
