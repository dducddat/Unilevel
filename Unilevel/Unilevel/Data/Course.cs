using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreateDate { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User user { get; set; }

        public string CodeOfCourse { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
