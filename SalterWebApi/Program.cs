using ExpRepositoryHelper;
using ExpRepositoryHelper.IRepository;
using ExpRepositoryHelper.Repository;
using ExpServiceHelper.IService;
using ExpServiceHelper.Service;
using ForumRepositoryHelper.IRepository;
using ForumRepositoryHelper.Repository;
using ForumServiceHelper.IService;
using ForumServiceHelper.Service;
using HomeRepositoryHelper.IRepository;
using HomeRepositoryHelper.Repository;
using HomeServiceHelper.IService;
using HomeServiceHelper.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SalterEFModels.EFModels;
using SalterWebApi.Middlewares;
using Scalar.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TripRepositoryHelper.IRepository;
using TripRepositoryHelper.Repository;
using TripServiceHelper.Cloudinary;
using TripServiceHelper.IService;
using TripServiceHelper.Service;
using UserRepositoryHelper.IRepository; 
using UserRepositoryHelper.Repository;
using UserServiceHelper.IService;
using UserServiceHelper.Service;




JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);


//雲端資料庫連接字串DI
//builder.Services.AddDbContext<SalterDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContext")));

//地端資料庫連接字串DI
builder.Services.AddDbContext<SalterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContextMac")));

//本機資料庫連接字串DI
//builder.Services.AddDbContext<SalterDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContextLocal")));

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
builder.Services.AddScoped<IGenericSalterRepository<ForumBoardInteraction>, GenericSalterRepository<ForumBoardInteraction>>();
builder.Services.AddScoped<IGenericSalterRepository<ForumPost>, GenericSalterRepository<ForumPost>>();
builder.Services.AddScoped<IGenericSalterRepository<ForumPostInteraction>, GenericSalterRepository<ForumPostInteraction>>();
builder.Services.AddScoped<IGenericSalterRepository<ForumAd>, GenericSalterRepository<ForumAd>>();
builder.Services.AddScoped<IGenericSalterRepository<ForumComment>, GenericSalterRepository<ForumComment>>();

//Forum功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IBoardsService, BoardsService>();
builder.Services.AddScoped<IPostsService, PostsService>();
builder.Services.AddScoped<IAdsService, AdsService>();
builder.Services.AddScoped<IPostInteractionsService, PostInteractionsService>();
builder.Services.AddScoped<IBoardInteractionsService, BoardInteractionsService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();

//User功能：泛型資料存取層 DAL DI
builder.Services.AddScoped(typeof(IGenericUserRepository<>), typeof(GenericUserRepository<>));
builder.Services.AddScoped<IGenericUserRepository<UserUser>, GenericUserRepository<UserUser>>();


//User功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<PasswordHasher<UserUser>>();

//Experience 注入
builder.Services.AddScoped<IRCoachIndex, RCoachIndex>();
builder.Services.AddScoped<ISCoachIndex, SCoachIndex>();
builder.Services.AddScoped<ISCoachMethods, SCoachMethods>();
//交易 注入
//builder.Services.AddScoped<ISEcPay, SEcPay>();



//Home功能：泛型資料存取層 DAL DI
builder.Services.AddScoped(typeof(IGenericHomeRepository<>), typeof(GenericHomeRepository<>));
builder.Services.AddScoped<IGenericHomeRepository<HomHouse>, GenericHomeRepository<HomHouse>>();
builder.Services.AddScoped<IGenericHomeRepository<HomRoomType>, GenericHomeRepository<HomRoomType>>();
builder.Services.AddScoped<IGenericHomeRepository<HomRoomImage>, GenericHomeRepository<HomRoomImage>>();
builder.Services.AddScoped<IGenericHomeRepository<HomReview>, GenericHomeRepository<HomReview>>();
//Home功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IHomService, HomService>();
builder.Services.AddScoped<CloudinaryService>();

//Trip功能 : DAL BLL DI
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ICloudinaryTripService, CloudinaryTripService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(options =>
{
    // 定義 Bearer Token 安全性定義
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "請輸入 JWT Token (不需要手動加 'Bearer ' 前綴)"
    });
});






//解決瀏覽器預設同源政策：定義存取的來源白名單 liveserver預設5500，angular4200
builder.Services.AddCors(option =>
{
    option.AddPolicy("Allow5500",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Location");
        });
    option.AddPolicy("Allow4200",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Location");
        });
});

// 使用Middleware做全域的Exception處理
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(); //全域錯誤處理

app.UseStaticFiles(); //存取靜態圖片

app.UseRouting();

//使用開放其他來源的自定義政策
//app.UseCors("Allow5500");
app.UseCors("Allow4200");

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
               .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch) // 預設產出 Fetch 代碼！
               .WithPreferredScheme("Bearer");
    });
}



app.UseHttpsRedirection();

app.UseAuthentication(); // 認證
app.UseAuthorization();  // 授權

app.MapControllers();

app.Run();
