using Microsoft.AspNetCore.Identity;
using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Info;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Users
{
    public class User:IdentityUser
    {
        public string LastName { get; set; } 
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public DateTime RegisterDate { get; set; }
        public string ProfilePhoto { get; set; }

        [ForeignKey("UserRole")] public int RoleId { get; set; } public UserRole UserRole { get; set; }
        [ForeignKey("UserAddress")] public int AddresId { get; set; } public UserAddress UserAddress { get; set; }
        [ForeignKey("UserPassport")] public int PassportId { get; set; } public UserPassport UserPassport { get; set; }

        [Required] public Employee Employee { get; set; }
        [Required] public Wife Wife { get; set; }
        [Required] public Husband Husband { get; set; }
    }
}
