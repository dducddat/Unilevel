namespace Unilevel.Models
{
    public class CommentSummary
    {
        public string AvatarUser { get; set; }

        public string FullNameUser { get; set; }

        public string Content { get; set; }
    }

    public class SendComment
    {
        public int JobId { get; set; }
        public string Content { get; set; }
    }
}
