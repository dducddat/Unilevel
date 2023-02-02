using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Notification
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public string? CreateByUserId { get; set; }

        [ForeignKey("CreateByUserId")]
        public User? CreateByUser { get; set; }
   
        public string? RecipientUserId { get; set; }

        [ForeignKey("RecipientUserId")]
        public User? RecipientUser { get; set; }

        public bool View { get; set; }
    }
}
