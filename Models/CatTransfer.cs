
using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.Models
{
    public class CatTransfer
    {
        public int Id { get; set; }
        [Required]
        public int CatId { get; set; }
        public Cat Cat { get; set; }
        [Required]
        public string RequesterId { get; set; }
        [Required]
        public string ConfirmerId { get; set; }
        public bool Confirmed { get; set; } = false;
    }
}
