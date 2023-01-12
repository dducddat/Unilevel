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
}
