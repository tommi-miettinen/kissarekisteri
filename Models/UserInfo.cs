
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    [Index(nameof(UserId), IsUnique = true)]
    public class UserInfo
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public bool IsBreeder { get; set; }
        public string AvatarUrl { get; set; }

    }
}
