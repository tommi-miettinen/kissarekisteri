
namespace Kissarekisteri.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public bool IsBreeder { get; set; }
        public string AvatarUrl { get; set; }

    }
}
