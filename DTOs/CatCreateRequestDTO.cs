using System;
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.DTOs
{
    public class CatCreateRequestDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required DateTime BirthDate { get; set; }
        [Required]
        public required string Breed { get; set; }

        [Required]
        public required string Sex { get; set; }
        public string ImageUrl { get; set; }
        public int? MotherId { get; set; }
        public int? FatherId { get; set; }

        public string OwnerId { get; set; }
        public string BreederId { get; set; }
    }
}
