using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Area
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? AreaCode { get; set; }
        public string? Name { get; set; }
    }
}
