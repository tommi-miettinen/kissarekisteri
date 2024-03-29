﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models;

[Index(nameof(Name), IsUnique = true)]
public class Permission
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }
}
