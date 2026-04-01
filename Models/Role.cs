namespace BENHVIENDAKHOANHANDUC.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Liên kết: Một quyền có thể được gán cho nhiều UserRole
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
