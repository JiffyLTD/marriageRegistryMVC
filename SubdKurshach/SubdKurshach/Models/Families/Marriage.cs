using SubdKurshach.Models.Employees;
using SubdKurshach.Models.Info;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Families
{
    public class Marriage
    {
        [Key] public int MarriageId { get; set; }
        [ForeignKey("Wife")] public int WifeId { get; set; } public Wife Wife { get; set; }
        [ForeignKey("Husband")] public int HusbandId { get; set; } public Husband Husband { get; set; }
        [ForeignKey("Employee")] public int EmployeeId { get; set; } public Employee Employee { get; set; }
        public DateTime MarriageDate { get; set; }
        [Required] public Family Family { get; set; }
    }
}
