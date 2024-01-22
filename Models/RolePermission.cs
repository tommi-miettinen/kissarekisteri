using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    [Index(nameof(RoleName), nameof(PermissionName), IsUnique = true)]
    public class RolePermission
    {
        public int Id { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        public int RoleId { get; set; }

        [Required]
        public string PermissionName { get; set; }

        [Required]
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
