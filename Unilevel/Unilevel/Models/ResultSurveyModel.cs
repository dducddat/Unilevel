using Unilevel.Data;

namespace Unilevel.Models
{
    public class ResultSurveyModel
    {
        public string SurveyId { get; set; }
        public List<ResultQuestion> Results { get; set; }
    }

    public class ResultQuestion
    {
        public string QuestionId { get; set; }
        public bool ResultA { get; set; }
        public bool ResultB { get; set; }
        public bool ResultC { get; set; }
        public bool ResultD { get; set; }
    }

    public class UserSurveyResultsInfor
    {
        public string SurveyTitle { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string QuestionTitle { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public bool ResultA { get; set; }
        public bool ResultB { get; set; }
        public bool ResultC { get; set; }
        public bool ResultD { get; set; }
    }
}
