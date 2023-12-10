namespace Kissarekisteribackend.Models
{
    public class CatPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int CatId { get; set; }
        public Cat Cat { get; set; }
    }
}
