using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class RequestSurvey
    {
        public string Id { get; set; }

        [ForeignKey("Survey")]
        public string SurveyId { get; set; }
        public Survey Survey { get;}

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Status { get; set; }
    }
}
