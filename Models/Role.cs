using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Kissarekisteri.Models;

[Index(nameof(Name), IsUnique = true)]
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
