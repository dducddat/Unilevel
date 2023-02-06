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

        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<Distributor> Distributors { get; set; } = default!;
        public DbSet<Area> Areas { get; set; } = default!;
        public DbSet<Question> Questions { get; set; } = default!;
        public DbSet<Survey> Surveys { get; set; } = default!;
        public DbSet<RequestSurvey> RequestSurveys { get; set; } = default!;
        public DbSet<ResultSurvey> ResultSurveys { get; set; } = default!;
        public DbSet<Job> Jobs { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<VisitPlan> VisitPlans { get; set; } = default!;
        public DbSet<Menu> Menus { get; set; } = default!;
        public DbSet<LinkRoleMenu> LinkRoleMenus { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<Articles> Articles { get; set; } = default!;
        public DbSet<Course> Courses { get; set; } = default!;
    }
}
