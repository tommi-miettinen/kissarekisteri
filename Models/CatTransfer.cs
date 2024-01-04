
namespace Kissarekisteri.Models
{
    public class CatTransfer
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public Cat Cat { get; set; }
        public string RequesterId { get; set; }
        public string ConfirmerId { get; set; }
        public bool Confirmed { get; set; } = false;
    }
}
