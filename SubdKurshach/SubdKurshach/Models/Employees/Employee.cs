using SubdKurshach.Models.Families;
using SubdKurshach.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Employees
{
    public class Employee
    {
        [Key] public int EmployeeId { get; set; }
        [ForeignKey("User")] public string UserId { get; set; } public User User { get; set; }
        [Required] public List<Marriage> Marriage { get; set; }
        [Required] public  List<Divorce> Divorce { get; set; }
    }
}
