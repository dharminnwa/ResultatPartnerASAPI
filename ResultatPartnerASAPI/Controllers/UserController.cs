using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ResultatPartnerAS.Model;
using ResultatPartnerAS.IBLL;
using System.Threading.Tasks;
using ResultatPartnerASAPI.Filters;
using ResultatPartnerASAPI.BL;

namespace ResultatPartnerASAPI.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        IUser iuserDetails;

        public UserController(IUser _iuserDetails)
        {
            iuserDetails = _iuserDetails;
        }

        [Route("LoginUser")]
        [HttpPost]
        public async Task<HttpResponseMessage> LoginUser([FromBody]UserModel value)
        {
            UserModel model = iuserDetails.LoginUser(value);
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [Route("LogoutUser")]
        [HttpPost]
        [APIAuthorize(Enable = true)]
        public async Task<HttpResponseMessage> LogoutUser()
        {
            string Token = Utility.GetRequestToken(Request);
            bool Response = iuserDetails.LogoutUser(Token);
            if (Response == true)
                return Request.CreateResponse(HttpStatusCode.OK, Response);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, Response);
        }
    }
}
