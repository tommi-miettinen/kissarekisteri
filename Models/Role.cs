using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models;

[Index(nameof(Name), IsUnique = true)]
public class Role
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }

    [JsonIgnore]
    public List<UserRole> UserRoles { get; set; }
}
