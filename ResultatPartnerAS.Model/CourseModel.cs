using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultatPartnerAS.Model
{
    public class CourseModel
    {
        public long CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseDescription { get; set; }
    }
    public class UserParticipatedCourse
    {
        public CourseModel Course { get; set; }
        public bool Participated { get; set; }
    }
}
