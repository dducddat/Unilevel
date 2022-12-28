using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public int PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Rating { get; set; }
        public bool Status { get; set; }
        public int? RoleId { get; set; }
        public int? ReportTo { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [ForeignKey("ReportTo")]
        public Role? ReportT { get; set; }

        [ForeignKey("Area")]
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
    }
}
