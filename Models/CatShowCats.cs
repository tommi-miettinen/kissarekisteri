using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatShowCats
    {
        public int Id { get; set; }

        [Required]
        public int CatId { get; set; }

        public Cat Cat { get; set; }

        [Required]
        public int CatShowId { get; set; }

        public CatShow CatShow { get; set; }
    }
}
