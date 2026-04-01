using System.ComponentModel.DataAnnotations;

namespace BENHVIENDAKHOANHANDUC.Models
{
    public class Patient
    {
        public int Id { get; set; }

        // --- THÔNG TIN HÀNH CHÍNH ---
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; } // Nam/Nữ/Khác
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Occupation { get; set; } // Nghề nghiệp

        // --- ĐỊA CHỈ (Phân cấp để dễ thống kê) ---
        public string Address { get; set; }     // Số nhà, tên đường
        public string Ward { get; set; }        // Phường/Xã
        public string District { get; set; }    // Quận/Huyện
        public string Province { get; set; }    // Tỉnh/Thành phố

        // --- THÔNG TIN TIẾP NHẬN ---
        public string Reason { get; set; }      // Lý do đến khám
        public DateTime RegisteredAt { get; set; } = DateTime.Now;
        public string Status { get; set; }      // Chờ khám / Đang khám / Hoàn tất
    }
}