using Kissarekisteribackend.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kissarekisteribackend.Models
{
    public class CatShow
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public virtual List<Attendee> Attendees { get; set; }

        [NotMapped]
        public List<UserResponse> AttendeeDetails { get; set; }

        public ICollection<CatShowPhoto> Photos { get; set; }
    }
}
