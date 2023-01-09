using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Question
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public bool Status { get; set; }
        public string? SurveyId { get; set; }

        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set;}
    }
}
