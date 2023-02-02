using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Job
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string? CreateByUserId { get; set; }

        [ForeignKey("CreateByUserId")]
        public User? CreateByUser { get; set; }

        public int? VisitPlanId { get; set; }

        [ForeignKey("VisitPlanId")]
        public VisitPlan? VisitPlan { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Status { get; set; }

        public bool Remove { get; set; }
    }
}
