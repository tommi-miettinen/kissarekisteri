using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class UserRole
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required int RoleId { get; set; }
        [Required]
        public string RoleName { get; set; }
        public Role Role { get; set; }
    }
}
