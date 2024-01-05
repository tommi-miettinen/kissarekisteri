using Kissarekisteri.Models;

namespace Kissarekisteri.DTOs
{
    public class CatTransferResultDTO : CatTransfer
    {
        public UserResponse Requester { get; set; }
    }
}
