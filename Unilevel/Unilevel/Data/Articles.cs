using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Articles
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Hypertext { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public string CreateByUserId { get; set; }

        [ForeignKey("CreateByUserId")]
        public User CreateByUser { get; set; }

        public bool Status { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
