using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public enum Place
    {
        First = 1,
        Second = 2,
        Third = 3,
    }

    [Index(nameof(CatId), nameof(Breed), nameof(CatShowId), IsUnique = true)]
    public class CatShowResult
    {
        public int Id { get; set; }

        [Required]
        public required int CatShowId { get; set; }
        public CatShow CatShow { get; set; }

        [Required]
        public required int CatId { get; set; }

        [Required]
        public required string Breed { get; set; }
        [JsonIgnore]
        public Cat Cat { get; set; }

        [Required]
        public required Place Place { get; set; }
    }
}
