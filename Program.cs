using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SRPM.Data;
using SRPM.Middlewares;
using SRPM.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Khởi tạo Firebase SDK
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
});

// ⚙️ Đăng ký ApplicationDbContext (EF Core)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 📦 Thêm dịch vụ cần thiết
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SRPM API",
        Version = "v1"
    });
});

// 🧩 Đăng ký các service (nếu có)
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IFundingService, FundingService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEvaluationService, EvaluationService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// 🧪 Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// ⚠️ Middleware xử lý lỗi toàn cục
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 🔐 Middleware xác thực Firebase
app.UseMiddleware<FirebaseAuthMiddleware>();

app.UseAuthorization();

// 📌 Map các controller route
app.MapControllers();

// ▶️ Chạy ứng dụng
app.Run();
