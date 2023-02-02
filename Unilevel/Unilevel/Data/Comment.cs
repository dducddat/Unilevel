using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public Job Job { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string Content { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
