using SubdKurshach.Models.Employees;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Families
{
    public class Divorce
    {
        [Key] public int DivorceId { get; set; }
        [ForeignKey("Family")] public int FamilyId { get; set; } public Family Family { get; set; }
        [ForeignKey("Employee")] public int EmployeeId { get; set; } public Employee Employee { get; set; }
        public DateTime DivorceDate { get; set; }
    }
}
