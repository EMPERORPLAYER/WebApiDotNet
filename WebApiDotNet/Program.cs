using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiDotNet.Data;
using WebApiDotNet.Data.Entities;
using WebApiDotNet.Interfaces;
using WebApiDotNet.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Налаштування підключення до PostgreSQL
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2. Реєстрація Identity (це усуне помилку 500)
builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    // Налаштування спрощеного пароля для тестування
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MyDatabaseContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


var seeder = new Seeder();
await seeder.SeedAsync(app.Services);

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();