using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models;

[Index(nameof(Name), IsUnique = true)]
public class Role
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
