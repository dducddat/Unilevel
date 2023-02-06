using Unilevel.Models;

namespace Unilevel.Services
{
    public interface ICourseRepository
    {
        public Task<List<object>> ImportCourse(List<FileExcelCourse> courses, string userId);
    }
}
