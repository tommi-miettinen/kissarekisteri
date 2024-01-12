namespace Kissarekisteri.Models
{
    public class CatShowCats
    {
        public int Id { get; set; }

        public int CatId { get; set; }

        public Cat Cat { get; set; }

        public int CatShowId { get; set; }

        public CatShow CatShow { get; set; }

    }
}
