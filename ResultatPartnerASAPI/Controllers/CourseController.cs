using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ResultatPartnerAS.Model;
using ResultatPartnerAS.IBLL;
using ResultatPartnerASAPI.Filters;
using ResultatPartnerASAPI.BL;

namespace ResultatPartnerASAPI.Controllers
{
    [RoutePrefix("api/Course")]
    public class CourseController : ApiController
    {
        ICourse icourseDetails;

        public CourseController(ICourse _icoursedetails)
        {
            icourseDetails = _icoursedetails;
        }

        [Route("GetAllCourses")]
        [HttpGet]
        [APIAuthorize(Enable = true)]
        public async Task<HttpResponseMessage> GetAllCourses()
        {
            List<CourseModel> li = icourseDetails.GetAllCourses();
            return Request.CreateResponse(HttpStatusCode.OK, li);
        }

        [Route("GetUserParticipatedCourseByID")]
        [HttpGet]
        [APIAuthorize(Enable = true)]
        public async Task<HttpResponseMessage> GetUserParticipatedCourseByID(long id)
        {
            string Token = Utility.GetRequestToken(Request);
            UserParticipatedCourse Response = icourseDetails.GetUserParticipatedCourseByID(Token, id);
            return Request.CreateResponse(HttpStatusCode.OK, Response);
        }

        [Route("ParticipateInCourse")]
        [HttpPost]
        [APIAuthorize(Enable = true)]
        public async Task<HttpResponseMessage> ParticipateInCourse([FromBody]CourseParticipantReq value)
        {
            string Token = Utility.GetRequestToken(Request);
            bool Response = false;
            Response = icourseDetails.ParticipateInCourse(Token, value.CourseID);
            if (Response == true)
                return Request.CreateResponse(HttpStatusCode.OK, Response);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, Response);
        }

        [Route("AddCourse")]
        [HttpPost]
        [APIAuthorize(Enable = true, isAdmin = true)]
        public async Task<HttpResponseMessage> AddCourse([FromBody]CourseModel value)
        {
            bool Response = icourseDetails.AddCourse(value);
            if (Response == true)
                return Request.CreateResponse(HttpStatusCode.OK, Response);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, Response);
        }

        [Route("UpdateCourse")]
        [HttpPost]
        [APIAuthorize(Enable = true, isAdmin = true)]
        public async Task<HttpResponseMessage> UpdateCourse([FromBody]CourseModel value)
        {
            bool Response = icourseDetails.UpdateCourse(value);
            if (Response == true)
                return Request.CreateResponse(HttpStatusCode.OK, Response);
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, Response);
        }
    }
}
