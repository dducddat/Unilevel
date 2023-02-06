using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class CourseRepository : ICourseRepository
    {
        private readonly UnilevelContext _context;

        public CourseRepository(UnilevelContext context)
        {
            _context = context;
        }

        public async Task<List<object>> ImportCourse(List<FileExcelCourse> courses, string userId)
        {
            var listCourse = courses;
            List<object> result = new List<object>();
            foreach (var course in listCourse)
            {
                try
                {
                    var user = await _context.Users.Where(u => u.Email == course.Email && u.FullName == course.FullName).SingleOrDefaultAsync();
                    if (user is null)
                    {
                        result.Add(new
                        {
                            Status = "False",
                            Course = course,
                            Message = "This line could not be added, this user was not found"
                        });

                        continue;
                    }

                    string startDate = course.StartDate;
                    DateTime parsedStartDate = DateTime.ParseExact(startDate, "d/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    string endDate = course.EndDate;
                    DateTime parsedEndDate = DateTime.ParseExact(endDate, "d/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                    _context.Add(new Course
                    {
                        Title = course.Title,
                        UserId = user.Id,
                        CreateDate = DateTime.Now,
                        CodeOfCourse = course.CodeOfCourse,
                        StartDate = parsedStartDate,
                        EndDate = parsedEndDate,
                    });
                    await _context.SaveChangesAsync();

                    _context.Add(new Notification
                    {
                        CreateByUserId = userId,
                        CreateDate = DateTime.Now,
                        RecipientUserId = user.Id,
                        View = false,
                        Title = "You have just been invited to a new course",
                        Description = $"You have just been invited to a new course: TITLE: {course.Title}, CODE OF COURSE: {course.CodeOfCourse}, START DATE: {parsedStartDate.ToString("dd-MM-yyyy")}, END DATE: {parsedEndDate.ToString("dd-MM-yyyy")}"
                    });
                }
                catch
                { 
                    result.Add(new
                    {
                        Status = "False",
                        Course = course,
                        Message = "This line could not be added, invalid input"
                    });

                    continue;
                }
            }

            return result;
        }
    }
}
