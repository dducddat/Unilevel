using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class ResultSurvey
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }

        public bool ResultA { get; set; }
        public bool ResultB { get; set; }
        public bool ResultC { get; set; }
        public bool ResultD { get; set; }
        public string SurveyId { get; set; }

        [ForeignKey("SurveyId")]
        public Survey Survey { get; set; }
    }
}
