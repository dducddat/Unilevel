using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class CompanionList
    {
        public int Id { get; set; }

        public int VisitPlanId { get; set; }

        [ForeignKey("VisitPlanId")]
        public VisitPlan VisitPlan { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
