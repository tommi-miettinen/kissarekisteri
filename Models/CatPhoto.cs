using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public class CatPhoto
    {
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public int CatId { get; set; }

        [JsonIgnore]
        public Cat Cat { get; set; }
    }
}
