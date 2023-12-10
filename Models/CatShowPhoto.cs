namespace Kissarekisteribackend.Models
{
    public class CatShowPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int CatShowId { get; set; }
        public CatShow CatShow { get; set; }
    }
}
