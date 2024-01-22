using System.ComponentModel.DataAnnotations;

namespace Kissarekisteri.DTOs
{
    public class UserCreatePayloadDTO
    {
        [Required]
        public string MailNickname { get; set; }

        [Required]
        public string GivenName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
