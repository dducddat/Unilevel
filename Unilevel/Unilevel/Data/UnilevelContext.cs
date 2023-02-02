using Microsoft.EntityFrameworkCore;
using Unilevel.Data;

namespace Unilevel.Data
{
    public class UnilevelContext : DbContext
    {
        public UnilevelContext(DbContextOptions<UnilevelContext> options)
            : base(options)
        {
        }

        public DbSet<Unilevel.Data.RefreshToken> RefreshTokens { get; set; } = default!;
        public DbSet<Unilevel.Data.User> Users { get; set; } = default!;
        public DbSet<Unilevel.Data.Role> Roles { get; set; } = default!;
        public DbSet<Unilevel.Data.Distributor> Distributors { get; set; } = default!;
        public DbSet<Unilevel.Data.Area> Areas { get; set; } = default!;
        public DbSet<Unilevel.Data.Question> Questions { get; set; } = default!;
        public DbSet<Unilevel.Data.Survey> Surveys { get; set; } = default!;
        public DbSet<Unilevel.Data.RequestSurvey> RequestSurveys { get; set; } = default!;
        public DbSet<Unilevel.Data.ResultSurvey> ResultSurveys { get; set; } = default!;
        public DbSet<Unilevel.Data.Job> Jobs { get; set; } = default!;
        public DbSet<Unilevel.Data.Category> Categories { get; set; } = default!;
        public DbSet<Unilevel.Data.Comment> Comments { get; set; } = default!;
        public DbSet<Unilevel.Data.VisitPlan> VisitPlans { get; set; } = default!;
        public DbSet<Unilevel.Data.Menu> Menus { get; set; } = default!;
        public DbSet<Unilevel.Data.LinkRoleMenu> LinkRoleMenus { get; set; } = default!;
        public DbSet<Unilevel.Data.Rating> Ratings { get; set; } = default!;
        public DbSet<Unilevel.Data.Notification> Notifications { get; set; } = default!;
        public DbSet<Unilevel.Data.Articles> Articles { get; set; } = default!;
    }
}
