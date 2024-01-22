using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public class CatPhoto
    {
        public int Id { get; set; }

        [Required]
        public required string Url { get; set; }

        [Required]
        public required int CatId { get; set; }

        [JsonIgnore]
        public Cat Cat { get; set; }
    }
}
