using ForumRepositoryHelper.IRepository;
using ForumRepositoryHelper.Repository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalterEFModels.EFModels;
using Scalar.AspNetCore;
using System.Text;
using UserRepositoryHelper.IRepository; 
using UserRepositoryHelper.Repository;
using UserServiceHelper.IService;
using UserServiceHelper.Service;

var builder = WebApplication.CreateBuilder(args);


//雲端資料庫連接字串DI
//builder.Services.AddDbContext<SalterDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContext")));

//地端資料庫連接字串DI
builder.Services.AddDbContext<SalterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContextMac")));

// JWT 驗證器註冊開始 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
// --- JWT

//Forum功能：泛型資料存取層 DAL DI
builder.Services.AddScoped(typeof(IGenericSalterRepository<>), typeof(GenericSalterRepository<>));
builder.Services.AddScoped<IGenericSalterRepository<ForumBoardCategory>, GenericSalterRepository<ForumBoardCategory>>();

//Forum功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IBoardsService, BoardsService>();


//User功能：泛型資料存取層 DAL DI
builder.Services.AddScoped(typeof(IGenericUserRepository<>), typeof(GenericUserRepository<>));
builder.Services.AddScoped<IGenericUserRepository<UserUser>, GenericUserRepository<UserUser>>();


//User功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<PasswordHasher<UserUser>>();


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
    // 配置 Scalar 介面
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");

        options.WithTitle("Salter API")
               .WithTheme(ScalarTheme.Laserwave) // 設定主題
               .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch); // 預設產出 Fetch 代碼！
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 認證
app.UseAuthorization();  // 授權

app.UseAuthorization();

app.MapControllers();

app.Run();
