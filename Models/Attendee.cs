namespace Kissarekisteribackend.Models
{
    public class Attendee
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public CatShow CatShow { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
