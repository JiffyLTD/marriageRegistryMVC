using SubdKurshach.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Families
{
    public class Family
    {
        [Key] public int FamilyId { get; set; }
        [ForeignKey("Marriage")] public int MarriageId {get;set;} public Marriage Marriage { get; set; }
        [ForeignKey("AllChildrens")] public int? AllChildrensId { get;set;} public AllChildrens? AllChildrens { get; set; }
        [Required] public Divorce Divorce { get; set; }
    }
}
