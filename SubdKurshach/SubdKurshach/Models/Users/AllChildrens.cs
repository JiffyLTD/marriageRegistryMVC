using SubdKurshach.Models.Families;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Users
{
    public class AllChildrens
    {
        [Key] public int ChildrensId { get; set; }
        [ForeignKey("Child")] public int? ChildId { get; set; } public Child? Child { get; set; }
        [ForeignKey("UserPassport")] public int PassportId { get; set; } public UserPassport UserPassport { get; set; }
        [Required] public Family Family { get; set; }
    }
}   
