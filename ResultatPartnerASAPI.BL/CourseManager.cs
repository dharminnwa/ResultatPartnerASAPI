using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultatPartnerAS.IBLL;
using ResultatPartnerAS.Model;
using ResultatPartnerASAPI.Data;

namespace ResultatPartnerASAPI.BL
{
    public class CourseManager : ICourse
    {
        // To Get All Published Courses
        public List<CourseModel> GetAllCourses()
        {
            List<CourseModel> CourseList = new List<CourseModel>();
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    List<Cours> dbList = dbContext.Courses.ToList();
                    Parallel.ForEach(dbList, x =>
                    {
                        CourseModel obj = new CourseModel();
                        obj.CourseID = x.CourseID;
                        obj.CourseName = x.CourseName;
                        obj.CourseDescription = x.CourseDescription;
                        CourseList.Add(obj);
                    });
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return CourseList;
        }

        // To Participate in Course
        public bool ParticipateInCourse(string Token, long CourseID)
        {
            bool isParticipated = false;
            try
            {
                long UserID = Utility.GetUserIDByToken(Token);
                if (UserID > 0 && CourseID > 0)
                {
                    using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                    {
                        CourseParticipant dbUserParticipated = new CourseParticipant();
                        dbUserParticipated.UserID = UserID;
                        dbUserParticipated.CourseID = CourseID;
                        dbUserParticipated.CreatedDate = DateTime.Now;
                        dbUserParticipated.UpdatedDate = DateTime.Now;
                        dbContext.CourseParticipants.Add(dbUserParticipated);
                        dbContext.SaveChanges();
                        isParticipated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return isParticipated;
        }

        //Check if user is participated or not
        public bool isUserParticipated(string Token, long CourseID)
        {
            bool isParticipated = false;
            try
            {
                long UserID = Utility.GetUserIDByToken(Token);
                if (UserID > 0 && CourseID > 0)
                {
                    using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                    {
                        CourseParticipant dbUserParticipated = dbContext.CourseParticipants.Where(x => x.CourseID == CourseID && x.UserID == UserID).FirstOrDefault();
                        if (dbUserParticipated != null && dbUserParticipated.CourseParticipantID > 0)
                        {
                            isParticipated = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return isParticipated;
        }

        // Add New Course
        public bool AddCourse(CourseModel _CourseModel)
        {
            bool isAdded = false;
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    Cours dbCourse = new Cours();
                    dbCourse.CourseName = _CourseModel.CourseName;
                    dbCourse.CourseDescription = _CourseModel.CourseDescription;
                    dbCourse.CreatedDate = DateTime.Now;
                    dbCourse.UpdatedDate = DateTime.Now;
                    dbContext.Courses.Add(dbCourse);
                    dbContext.SaveChanges();
                    isAdded = true;
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return isAdded;
        }

        // Update Course Detail
        public bool UpdateCourse(CourseModel _CourseModel)
        {
            bool isUpdated = false;
            try
            {
                using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                {
                    Cours dbCourse = dbContext.Courses.Where(x => x.CourseID == _CourseModel.CourseID).FirstOrDefault();
                    if (dbCourse != null && dbCourse.CourseID > 0)
                    {
                        dbCourse.CourseName = _CourseModel.CourseName;
                        dbCourse.CourseDescription = _CourseModel.CourseDescription;
                        dbCourse.UpdatedDate = DateTime.Now;
                        dbContext.SaveChanges();
                        isUpdated = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return isUpdated;
        }

        // Get Course Details By ID
        public UserParticipatedCourse GetUserParticipatedCourseByID(string Token, long CourseID)
        {
            UserParticipatedCourse _Course = new UserParticipatedCourse();
            try
            {
                long UserID = Utility.GetUserIDByToken(Token);
                if (UserID > 0 && CourseID > 0)
                {
                    using (ResultatPartnerASEntities dbContext = new ResultatPartnerASEntities())
                    {
                        Cours dbCourse = dbContext.Courses.Where(x => x.CourseID == CourseID).FirstOrDefault();
                        if (dbCourse != null && dbCourse.CourseID > 0)
                        {
                            _Course.Course = new CourseModel();
                            _Course.Course.CourseID = dbCourse.CourseID;
                            _Course.Course.CourseName = dbCourse.CourseName;
                            _Course.Course.CourseDescription = dbCourse.CourseDescription;
                        }
                        CourseParticipant dbCourseParticipant = dbContext.CourseParticipants.Where(x => x.CourseID == CourseID && x.UserID == UserID).FirstOrDefault();
                        if (dbCourseParticipant != null && dbCourseParticipant.CourseParticipantID > 0)
                            _Course.Participated = true;
                        else
                            _Course.Participated = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.writelog(ex.Message);
            }
            return _Course;
        }
    }
}
