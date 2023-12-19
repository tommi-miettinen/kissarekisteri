﻿using Kissarekisteri.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kissarekisteri.Models
{
    public class Attendee
    {
        public int Id { get; set; }
        public required int EventId { get; set; }
        public CatShow CatShow { get; set; }
        public required string UserId { get; set; }

        [NotMapped]
        public UserResponse User { get; set; }
        public List<CatAttendee> CatAttendees { get; set; }
    }
}
