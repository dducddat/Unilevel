using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? Rating { get; set; }
        public bool Status { get; set; }
        public string? RoleId { get; set; }
        public string? ReportTo { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [ForeignKey("ReportTo")]
        public Role ReportT { get; set; }

        [ForeignKey("Area")]
        public string? AreaCode { get; set; }
        public Area Area { get; set; }
    }
}
