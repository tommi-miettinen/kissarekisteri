using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public class CatShowPhoto
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        [Required]
        public int CatShowId { get; set; }

        [JsonIgnore]
        public CatShow CatShow { get; set; }
    }
}
