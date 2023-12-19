using System;
using System.Collections.Generic;

namespace Kissarekisteri.Models
{
    public class CatShow
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<Attendee> Attendees { get; set; }
        public List<CatShowPhoto> Photos { get; set; }

        public List<CatShowResult> Results { get; set; }
    }
}
