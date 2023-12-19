using System;

namespace Kissarekisteri.DTOs
{
    public class CatRequest
    {
        public required string Name { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Breed { get; set; }
        public string ImageUrl { get; set; }
    }
}
