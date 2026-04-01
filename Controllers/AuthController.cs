using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BENHVIENDAKHOANHANDUC.Data;
using BENHVIENDAKHOANHANDUC.Models;

namespace BENHVIENDAKHOANHANDUC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public AuthController(HospitalDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Kiểm tra đầu vào cơ bản
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Vui lòng nhập đầy đủ tài khoản và mật khẩu!" });
            }

            // 2. Tìm user trong DB kèm theo các quyền (Roles) của họ
            // Sử dụng .Include và .ThenInclude để lấy dữ liệu từ bảng liên kết UserRoles sang bảng Role
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password);

            // 3. Nếu không tìm thấy hoặc sai mật khẩu
            if (user == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
            }

            // 4. Lấy danh sách các quyền thực tế mà User này ĐÃ ĐƯỢC CẤP
            // Trả về cả Id và Name để Frontend xử lý logic chọn quyền dễ dàng hơn
            var rolesList = user.UserRoles.Select(ur => new
            {
                id = ur.Role.Id,
                name = ur.Role.Name
            }).ToList();

            // 5. Kiểm tra nếu User chưa được cấp bất kỳ quyền nào
            if (!rolesList.Any())
            {
                return Forbidden(new { message = "Tài khoản của bạn hiện chưa được cấp quyền truy cập hệ thống!" });
            }

            // 6. Trả về thông tin thành công cho Frontend
            return Ok(new
            {
                userId = user.Id,
                username = user.Username,
                fullName = user.FullName,
                roles = rolesList, // Danh sách quyền để người dùng chọn
                message = "Xác thực thành công!"
            });
        }

        // Helper method cho trường hợp không có quyền (403)
        private ObjectResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }
    }
}