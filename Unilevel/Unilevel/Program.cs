using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UnilevelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Unilevel")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IDistributorRepository, DistributorRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
