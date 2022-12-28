using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Distributor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        [ForeignKey("Area")]
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
    }
}
