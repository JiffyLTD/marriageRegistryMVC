using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.Models.Users
{
    public class UpdatedUserPassport
    {
        [Key] public int Id { get; set; }
        public int PassportId { get; set; }
        public int PassportSeries { get; set; }
        public int PassportNum { get; set; }
        public DateTime PassportDate { get; set; }
        public string PassportIssuedByWhom { get; set; }
        public DateTime Birthday { get; set; }
    }
}
