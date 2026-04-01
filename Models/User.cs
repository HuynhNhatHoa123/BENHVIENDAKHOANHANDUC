namespace BENHVIENDAKHOANHANDUC.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Liên kết: Một user có thể nằm trong nhiều dòng của bảng UserRole
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
