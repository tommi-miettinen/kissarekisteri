using System;

namespace Kissarekisteribackend.Models
{
    public class User
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; } = "default";
        public Boolean IsBreeder { get; set; } = false;
    }
}
