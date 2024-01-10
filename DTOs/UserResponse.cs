using Kissarekisteri.Models;
using Microsoft.Graph.Models;
using System.Collections.Generic;

namespace Kissarekisteri.DTOs
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string AvatarUrl { get; set; }

        public string Email { get; set; }

        public bool IsBreeder { get; set; }

        public UserRole UserRole { get; set; }

        public List<ObjectIdentity> Identities { get; set; }
    }
}
