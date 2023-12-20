using Kissarekisteri.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kissarekisteri.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Breed { get; set; }
        public required string OwnerId { get; set; }
        [NotMapped]
        public UserResponse Owner { get; set; }
        public required string BreederId { get; set; }

        [NotMapped]
        public UserResponse Breeder { get; set; }
        public string ImageUrl { get; set; }

        public List<CatShowResult> Results { get; set; }
        public List<CatPhoto> Photos { get; set; }
    }
}