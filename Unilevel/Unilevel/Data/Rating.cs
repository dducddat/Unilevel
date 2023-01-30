using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Rating
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        public int NumberOfStar { get; set; }
    }
}
