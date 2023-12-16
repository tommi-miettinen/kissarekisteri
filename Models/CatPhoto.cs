using System.Text.Json.Serialization;

namespace Kissarekisteribackend.Models
{
    public class CatPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int CatId { get; set; }

        [JsonIgnore]
        public Cat Cat { get; set; }
    }
}
