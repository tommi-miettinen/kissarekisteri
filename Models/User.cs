namespace Kissarekisteribackend.Models
{
    public class User
    {
        public string Id { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; } = "default";
        public bool IsBreeder { get; set; } = false;
    }
}
