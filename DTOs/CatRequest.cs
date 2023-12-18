using System;

namespace Kissarekisteri.DTOs
{
    public class CatRequest
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public string Breed { get; set; }
        public string ImageUrl { get; set; }
    }
}
