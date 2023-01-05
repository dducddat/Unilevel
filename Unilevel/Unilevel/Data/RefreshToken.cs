using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
    }
}
