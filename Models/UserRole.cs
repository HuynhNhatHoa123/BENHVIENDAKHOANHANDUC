namespace BENHVIENDAKHOANHANDUC.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Gắn ngược lại để EF biết dòng này thuộc về User nào và Role nào
        public virtual User? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}
