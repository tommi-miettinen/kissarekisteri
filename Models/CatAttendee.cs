namespace Kissarekisteri.Models
{
    public class CatAttendee
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int CatId { get; set; }
        public Cat Cat { get; set; }
        public string AttendeeId { get; set; }
        public Attendee Attendee { get; set; }
    }
}