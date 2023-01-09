namespace Unilevel.Data
{
    public class Survey
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeopleDid { get; set; }
    }
}
