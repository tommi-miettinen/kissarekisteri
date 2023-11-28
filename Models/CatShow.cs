using System;
using System.Collections.Generic;

namespace Kissarekisteribackend.Models
{
    public class CatShow
    {
        public int Id { get; set; } // Unique identifier for the event

        public string Name { get; set; } // Name or title of the event

        public string Description { get; set; } // Description of the event

        public string Location { get; set; } // Venue or location of the event

        public DateTime StartDate { get; set; } // Date and time when the event starts

        public DateTime EndDate { get; set; }

        public virtual List<Attendee> Attendees { get; set; }
    }
}
