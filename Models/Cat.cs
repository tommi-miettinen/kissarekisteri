using System;

namespace Kissarekisteribackend.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Breed { get; set; }
        public string OwnerId { get; set; }
        public string BreederId { get; set; }

    }
}