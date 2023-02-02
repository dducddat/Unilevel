using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class VisitPlan
    {
        public int Id { get; set; }

        public DateTime VisitDate { get; set; }

        public string Title { get; set; }

        public int Time { get; set; }

        public string DistributorId { get; set; }

        public string? CreateByUserId { get; set; }

        [ForeignKey("CreateByUserId")]
        public User? User { get; set; }

        [ForeignKey("DistributorId")]
        public Distributor Distributor { get; set; }

        public string Purpose { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Remove { get; set; }

        public string? GuestId { get; set; }

        [ForeignKey("GuestId")]
        public User? Guest { get; set; }

        public bool Status { get; set; }
    }
}
