using Unilevel.Data;

namespace Unilevel.Models
{
    public class VisitPlanAdd
    {
        public List<DateTime> VisitDate { get; set; }
        public string Title { get; set; }
        public int Time { get; set; }
        public string DistributorId { get; set; }
        public string Purpose { get; set; }
        public string Email { get; set; }
    }

    public class VisitPlanDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string Guest { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
        public List<JobSummary> ListJobSummary { get; set; }
    }

    public class VisitPlanSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreateDate { get; set; }
        public string VisitDate { get; set; }
        public string CreateByUser { get; set; }
        public bool Status { get; set; }
    }

    public class EditVisitPlan
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Time { get; set; }
        public DateTime VisitDate { get; set; }
        public string DistributorId { get; set; }
        public string Purpose { get; set; }
    }
}
