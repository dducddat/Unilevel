using System.ComponentModel.DataAnnotations.Schema;
using Unilevel.Data;

namespace Unilevel.Models
{
    public class ArticlesBrief
    {
        public string Title { get; set; }

        public string Thumbnail { get; set; }

        public string Hypertext { get; set; }

        public string Description { get; set; }
    }

    public class ArticlesDetail
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Hypertext { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public string CreateByUser { get; set; }

        public string Status { get; set; }

        public string CreateDate { get; set; }
    }

    public class ArticlesModel
    {
        public int Id { get; set; }

        public string CreateByUser { get; set; }

        public string CreateDate { get; set; }

        public string Title { get; set; }

        public string Status { get; set; }
    }
}
