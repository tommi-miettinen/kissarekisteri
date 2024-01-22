namespace Kissarekisteri.DTOs
{
    public class UserUpdateRequestDTO
    {
        public bool IsBreeder { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhoneNumber { get; set; }
        public string PhoneNumber { get; set; }

    }
}
