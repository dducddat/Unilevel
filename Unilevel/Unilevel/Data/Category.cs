using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Remove { get; set; }
    }
}
