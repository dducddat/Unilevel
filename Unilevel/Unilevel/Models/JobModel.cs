namespace Unilevel.Models
{
    public class JobSummary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreateByUser { get; set; }
        public string CreateDate { get; set; }
        public string Status { get; set; }
    }

    public class AddJob : EditJob
    {
        public int VisitPlanId { get; set; }
    }

    public class EditJob
    {
        public int? Id { get; set; }

        public string Title { get; set; }

        public int CategoryId { get; set; }

        public string UserEmail { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class JobDetails
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CreateByUser { get; set; }

        public string CreateDate { get; set; }

        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }

        public string Category { get; set; }

        public string User { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string Rate { get; set; }

        public List<CommentSummary> Comments { get; set; }
    }
}
