using Microsoft.EntityFrameworkCore;
using BENHVIENDAKHOANHANDUC.Data;

var builder = WebApplication.CreateBuilder(args);

// --- PHẦN 1: ĐĂNG KÝ CÁC DỊCH VỤ (SERVICES) ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký Database
builder.Services.AddDbContext<HospitalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký CORS (Để trình duyệt không chặn khi làm giao diện)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// --- PHẦN 2: XÂY DỰNG ỨNG DỤNG ---
// (Dòng này chỉ được xuất hiện DUY NHẤT 1 lần)
var app = builder.Build();

// --- PHẦN 3: CẤU HÌNH CÁCH CHẠY (MIDDLEWARE) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
// Kích hoạt CORS (Phải nằm TRƯỚC UseAuthorization)
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Chạy ứng dụng
app.Run();