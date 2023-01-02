using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class Area
    {
        [Key]
        public string AreaCode { get; set; }
        public string Name { get; set; }
    }
}
