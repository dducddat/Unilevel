namespace Unilevel.Services
{
    public class SurveyDetail
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfPeopleDid { get; set; }
    }

    public class AddSurvey
    {
        public string Title { get; set; }
    }
}
