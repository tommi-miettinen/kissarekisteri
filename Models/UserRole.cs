namespace Kissarekisteri.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
