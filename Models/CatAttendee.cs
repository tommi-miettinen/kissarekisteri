namespace Kissarekisteri.Models
{
    public class CatAttendee
    {
        public int Id { get; set; }
        public required int EventId { get; set; }
        public required int CatId { get; set; }
        public Cat Cat { get; set; }
        public required int AttendeeId { get; set; }
        public Attendee Attendee { get; set; }
    }
}