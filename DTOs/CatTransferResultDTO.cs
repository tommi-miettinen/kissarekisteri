using Kissarekisteri.Models;

namespace Kissarekisteri.DTOs
{
    public class CatTransferResultDTO : CatTransfer
    {
        public UserResponseDTO Requester { get; set; }
    }
}
