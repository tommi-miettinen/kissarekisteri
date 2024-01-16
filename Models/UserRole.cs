using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    [Index(nameof(UserId), IsUnique = true)]
    public class UserRole
    {
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [Required]
        public required int RoleId { get; set; }
        [Required]
        public required string RoleName { get; set; }
        public Role Role { get; set; }
    }
}
