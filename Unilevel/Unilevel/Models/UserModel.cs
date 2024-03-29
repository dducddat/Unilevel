﻿using Unilevel.Data;

namespace Unilevel.Models
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePass
    {
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }


    public class UserInfo
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string StatusName { get; set; }
        public string AreaName { get; set; }
        public string ReportTo { get; set; }
    }

    public class AddUser
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public bool Status { get; set; }
        public string ReportTo { get; set; }
    }

    public class EditInfoUser
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    public class UserIdAndNameAndEmail
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public class ResetPassword
    { 
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class ProfileUser
    {
        public string FullName { get; set; }

        public string CratedDate { get; set; }

        public List<CommentSummary> LastComment { get; set; }

        public List<JobSummary> TaskDone { get; set; }

        public List<JobSummary> TaskNotDone { get; set; }

        public List<CourseModel> Courses { get; set; }
    }
}
