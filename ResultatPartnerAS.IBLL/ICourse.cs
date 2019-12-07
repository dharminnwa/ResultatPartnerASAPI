using ResultatPartnerAS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPartnerAS.IBLL
{
    public interface ICourse
    {
        List<CourseModel> GetAllCourses();
        bool ParticipateInCourse(string Token, long CourseID);
        bool isUserParticipated(string Token, long CourseID);
        bool AddCourse(CourseModel _CourseModel);
        bool UpdateCourse(CourseModel _CourseModel);
        UserParticipatedCourse GetUserParticipatedCourseByID(string Token, long CourseID);
    }
}
