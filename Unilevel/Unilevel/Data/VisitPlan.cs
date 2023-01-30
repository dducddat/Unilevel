using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class VisitPlan
    {
        public int Id { get; set; }

        public DateTime VisitDate { get; set; }

        public string Time { get; set; }

        public string DistributorId { get; set; }

        [ForeignKey("DistributorId")]
        public Distributor Distributor { get; set; }

        public string Purpose { get; set; }

        public DateTime CreateDate { get; set; }

        public bool Remove { get; set; }
    }
}
