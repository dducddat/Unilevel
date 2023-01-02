using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Role
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
