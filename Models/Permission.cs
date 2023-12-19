using Microsoft.EntityFrameworkCore;

namespace Kissarekisteri.Models;

[Index(nameof(Name), IsUnique = true)]
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; }
}
