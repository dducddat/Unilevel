using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
