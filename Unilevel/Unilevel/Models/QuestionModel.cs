namespace Unilevel.Models
{
    public class AddOrEditQuestion
    {
        public string Title { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
    }

    public class ViewQuestion
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }

    public class QuestionDetail : AddOrEditQuestion
    {
        public string Id { get; set;}
    }
}
