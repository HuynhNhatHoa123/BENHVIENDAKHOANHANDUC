using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BENHVIENDAKHOANHANDUC.Data;
using BENHVIENDAKHOANHANDUC.Models;

namespace BENHVIENDAKHOANHANDUC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public PatientsController(HospitalDbContext context)
        {
            _context = context;
        }

        // 1. Tìm kiếm bệnh nhân theo Số điện thoại (Dùng cho nút Search ở Reception)
        [HttpGet("search")]
        public async Task<IActionResult> SearchByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return BadRequest(new { message = "Số điện thoại không được để trống" });

            var patient = await _context.Patients
                .Where(p => p.Phone == phone)
                .OrderByDescending(p => p.Id) // Ưu tiên bản ghi mới nhất
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound(new { message = "Không tìm thấy bệnh nhân" });

            return Ok(patient);
        }

        // 2. Thêm mới bệnh nhân (Dùng cho nút LƯU & ĐĂNG KÝ KHÁM)
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] Patient patient)
        {
            if (patient == null) return BadRequest();

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return Ok(patient);
        }

        // 3. Lấy danh sách tất cả bệnh nhân (Dùng cho trang quản trị nếu cần)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }
    }
}