using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public enum Place
    {
        First = 1,
        Second = 2,
        Third = 3,
    }

    [Index(nameof(CatId), IsUnique = true)]
    public class CatShowResult
    {
        public int Id { get; set; }
        public required int CatShowId { get; set; }
        [JsonIgnore]
        public CatShow CatShow { get; set; }


        public required int CatId { get; set; }
        public required string Breed { get; set; }
        [JsonIgnore]
        public Cat Cat { get; set; }

        public required Place Place { get; set; }
    }
}
