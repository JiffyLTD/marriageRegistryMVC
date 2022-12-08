using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.Models.Users
{
    public class Child
    {
        [Key]public int ChildId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public int BirthCertificate { get; set; }
        [Required] public List<AllChildrens> AllChildrens { get; set; }
    }
}
