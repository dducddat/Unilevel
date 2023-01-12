namespace Unilevel.Models
{
    public class SurveyInfo
    {
        public string SurveyId { get; set; }
        public string TitleSurvey { get; set; }
        public DateTime Created { get; set; }
    }

    public class SurveyDetail : SurveyInfo
    {
        public List<QuestionDetail> QuestionDetails { get; set; }
    }

    public class AddOrEditSurvey
    {
        public string Title { get; set; }
    }

    public class AddListQuestionId
    {
        public List<string> QuestionId { get; set; }
    }

    public class SendRequestSurvey
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> UserId { get; set; }
    }

    public class SurveyList
    {
        public string SurveyId { get; set; }
        public string TitleSurvey { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string NumberOfPeopleDid { get; set; }
    }

    public class UserDidOrDontDoSurvey : SurveyInfo
    {
        public List<UserInfo> Users { get; set; }
    }
}
