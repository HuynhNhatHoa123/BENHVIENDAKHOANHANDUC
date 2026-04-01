using Microsoft.EntityFrameworkCore;
using BENHVIENDAKHOANHANDUC.Models;

namespace BENHVIENDAKHOANHANDUC.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Patient> Patients { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình quan hệ cho bảng trung gian UserRole
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // 2. TẠO DỮ LIỆU MẪU (SEED DATA)

            // Tạo các Quyền (Roles)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Doctor" },
                new Role { Id = 3, Name = "Staff" }
            );

            // Tạo tài khoản Admin mặc định
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "123456", // Lưu ý: Sau này sẽ nâng cấp lên mã hóa mật khẩu
                    FullName = "Quản trị viên hệ thống",
                    Email = "admin@nhanduc.com"
                }
            );

            // Gán quyền Admin cho tài khoản admin (Bảng trung gian)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = 1, UserId = 1, RoleId = 1 }
            );
        }
    }
}