using ForumRepositoryHelper.IRepository;
using ForumRepositoryHelper.Repository;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;

using SalterEFModels.EFModels;
using Scalar.AspNetCore;

using ForumServiceHelper.IService;
using ForumServiceHelper.Service;

var builder = WebApplication.CreateBuilder(args);


//雲端資料庫連接字串DI
//builder.Services.AddDbContext<SalterDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContext")));

//地端資料庫連接字串DI
builder.Services.AddDbContext<SalterDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SalterDbContextMac")));

//Forum功能：泛型資料存取層 DAL DI
builder.Services.AddScoped(typeof(IGenericSalterRepository<>), typeof(GenericSalterRepository<>));
builder.Services.AddScoped<IGenericSalterRepository<ForumBoardCategory>, GenericSalterRepository<ForumBoardCategory>>();

//Forum功能：商業邏輯層 BLL DI
builder.Services.AddScoped<IBoardsService, BoardsService>();

//Home功能：泛型資料存取層 DAL DI


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

app.UseAuthorization();

app.MapControllers();

app.Run();
