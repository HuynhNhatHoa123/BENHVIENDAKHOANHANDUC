using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BENHVIENDAKHOANHANDUC.Data;
using BENHVIENDAKHOANHANDUC.Models;

namespace BENHVIENDAKHOANHANDUC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public UsersController(HospitalDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách nhân viên (Hiển thị ở bảng)
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    Roles = _context.UserRoles
                                    .Where(ur => ur.UserId == u.Id)
                                    .Select(ur => ur.Role.Name)
                                    .ToList()
                }).ToListAsync();
            return Ok(users);
        }

        // 2. Lấy CHI TIẾT 1 nhân viên (Đổ dữ liệu vào Modal khi ấn Sửa)
        // ĐÂY LÀ PHẦN QUAN TRỌNG ĐỂ SỬA LỖI 405
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    // Trả về RoleIds dạng mảng [1, 2] để JS tick vào checkbox
                    RoleIds = _context.UserRoles
                                    .Where(ur => ur.UserId == u.Id)
                                    .Select(ur => ur.RoleId)
                                    .ToList()
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên!" });
            }

            return Ok(user);
        }

        // 3. Thêm nhân viên mới
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });
            }

            var newUser = new User
            {
                Username = dto.Username,
                PasswordHash = dto.PasswordHash, // Khuyên dùng: BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash)
                FullName = dto.FullName,
                Email = dto.Email
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            if (dto.RoleIds != null && dto.RoleIds.Count > 0)
            {
                foreach (var roleId in dto.RoleIds)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = roleId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Thêm nhân viên và phân quyền thành công!" });
        }

        // 4. Cập nhật nhân viên
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserCreateDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên!" });
            }

            // Cập nhật thông tin cơ bản
            user.FullName = updateDto.FullName;
            user.Email = updateDto.Email;

            // Chỉ cập nhật pass nếu người dùng có nhập mới
            if (!string.IsNullOrEmpty(updateDto.PasswordHash))
            {
                user.PasswordHash = updateDto.PasswordHash;
            }

            // Cập nhật quyền hạn: Xóa cũ - Thêm mới
            var oldRoles = _context.UserRoles.Where(ur => ur.UserId == id);
            _context.UserRoles.RemoveRange(oldRoles);

            if (updateDto.RoleIds != null && updateDto.RoleIds.Count > 0)
            {
                foreach (var roleId in updateDto.RoleIds)
                {
                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = id,
                        RoleId = roleId
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cập nhật nhân viên thành công!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Lỗi xung đột dữ liệu!" });
            }
        }

        // 5. Xóa nhân viên
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy nhân viên!" });

            // Xóa quyền trước, xóa user sau (tránh lỗi khóa ngoại)
            var userRoles = _context.UserRoles.Where(ur => ur.UserId == id);
            _context.UserRoles.RemoveRange(userRoles);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xóa nhân viên thành công!" });
        }
    }

    // DTO nhận dữ liệu từ Frontend
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<int> RoleIds { get; set; }
    }
}