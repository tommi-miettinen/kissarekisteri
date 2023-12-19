using System;
using System.Collections.Generic;

namespace Kissarekisteri.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Breed { get; set; }
        public required string OwnerId { get; set; }
        public required string BreederId { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<CatPhoto> Photos { get; set; }
    }
}