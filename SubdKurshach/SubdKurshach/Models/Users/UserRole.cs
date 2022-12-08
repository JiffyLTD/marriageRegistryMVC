using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.Models.Users
{
    public class UserRole
    {
        [Key]public int RoleId { get; set; }
        public string RoleName { get; set; }
        [Required] public List<User> Users { get; set; }
    }
}
