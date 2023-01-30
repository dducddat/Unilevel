using System.ComponentModel.DataAnnotations.Schema;

namespace Unilevel.Data
{
    public class LinkRoleMenu
    {
        public int Id { get; set; }

        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }
    }
}
