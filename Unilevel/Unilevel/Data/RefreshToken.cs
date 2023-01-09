using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public string JwtId { get; set; }
        public string RefToken { get; set; }
        public DateTime Expires { get; set; }
    }
}
