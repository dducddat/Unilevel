namespace Unilevel.Models
{
    public class LinkRoleMenuModel
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int MenuId { get; set; }
        public string MenuPermission { get; set; }
    }

    public class AddLinkRoleMenu
    {
        public string RoleId { get; set; }
        public int MenuId { get; set; }
    }
}
