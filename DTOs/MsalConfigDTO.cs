namespace Kissarekisteri.DTOs
{
    public class MsalConfigDTO
    {
        public required int Id { get; set; }
        public required string AuthorityDomain { get; set; }
        public required string Authority { get; set; }
        public required string ClientId { get; set; }
        public required string Instance { get; set; }
        public required string Domain { get; set; }
        public required string RedirectUri { get; set; }
    }
}
