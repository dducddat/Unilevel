using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Unilevel.Data;
using Unilevel.Helpers;
using Unilevel.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UnilevelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Unilevel")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthorization(option => {
    option.AddPolicy("CanManageRole", policy =>
        policy.AddRequirements(new UserRoleRequirement("Role.Manage"))
    );

    option.AddPolicy("CanManageArea", policy =>
        policy.AddRequirements(new UserRoleRequirement("Area.Manage"))
    );

    option.AddPolicy("CanManageDistributor", policy =>
        policy.AddRequirements(new UserRoleRequirement("Distributor.Manage"))
    );
    option.AddPolicy("CanManageUser", policy =>
        policy.AddRequirements(new UserRoleRequirement("User.Manage"))
    );
});

builder.Services.AddScoped<IAuthorizationHandler, UserRoleHandler>();
builder.Services.AddScoped<IDistributorRepository, DistributorRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<IArticlesRepository, ArticlesRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IVisitPlanRepository, VisitPlanRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(builder.Configuration.GetSection("SecretKey").Value)),

        ClockSkew = TimeSpan.Zero
    };
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    option.OperationFilter<SecurityRequirementsOperationFilter>();
}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
