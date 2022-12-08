using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.Models.Users
{
    public class UserPassport
    {
        [Key]public int PassportId { get; set; }
        public int PassportSeries { get; set; }
        public int PassportNum { get; set; }
        public DateTime PassportDate { get; set; }
        public string PassportIssuedByWhom { get; set; }
        public DateTime Birthday { get; set; }
        [Required] public List<AllChildrens> AllChildrens { get; set; }
        [Required] public List<User> User { get; set; }
    }
}
