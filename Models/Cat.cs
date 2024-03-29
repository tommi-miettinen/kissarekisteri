﻿using Kissarekisteri.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kissarekisteri.Models
{
    public class Cat : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required DateTime BirthDate { get; set; }

        [Required]
        public required string Breed { get; set; }

        [Required]
        public required string Sex { get; set; }

        [Required]
        public required string OwnerId { get; set; }

        [NotMapped]
        public UserResponseDTO Owner { get; set; }

        [Required]
        public required string BreederId { get; set; }

        [NotMapped]
        public UserResponseDTO Breeder { get; set; }
        public string ImageUrl { get; set; }
        public List<CatRelation> Parents { get; set; }
        public List<CatRelation> Kittens { get; set; }
        public List<CatShowResult> Results { get; set; }
        public List<CatPhoto> Photos { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowedSexValues = new HashSet<string> { "Male", "Female" };

            if (!allowedSexValues.Contains(Sex))
            {
                yield return new ValidationResult(
                    "Sex must be either 'Male' or 'Female'.",
                    new[] { nameof(Sex) }
                );
            }
        }
    }
}
