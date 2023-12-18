namespace Kissarekisteri.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public string PermissionName { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}
