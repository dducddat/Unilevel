namespace Unilevel.Models
{
    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AddUserDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? RoleId { get; set; }
        public bool Status { get; set; }
        public string? AreaCode { get; set; }
        public string ReportTo { get; set; }
    }

}
