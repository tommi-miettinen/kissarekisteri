using Kissarekisteri.Models;
using System.Collections.Generic;

namespace Kissarekisteri.DTOs
{
    public class AttendeeDTO
    {
        public int Id { get; set; }
        public required int EventId { get; set; }
        public CatShow CatShow { get; set; }
        public required string UserId { get; set; }

        public UserResponse User { get; set; }
        public List<CatAttendee> CatAttendees { get; set; }
    }

}

