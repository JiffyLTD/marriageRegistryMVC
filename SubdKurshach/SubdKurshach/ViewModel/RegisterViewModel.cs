using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.ViewModel
{
    public class RegisterViewModel
    {
        //для пользователя
        [Required]public string LastName { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string Patronymic { get; set; }
        [Required] public string ProfilePhoto { get; set; }
        [Required] public string PhoneNum { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        //для адреса
        [Required] public string Country { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Street { get; set; }
        [Required] public string HouseNum { get; set; }
        [Required] public string? AppartNum { get; set; }
        //для паспорта
        [Required] public int PassportSeries { get; set; }
        [Required] public int PassportNum { get; set; }
        [Required] public DateTime PassportDate { get; set; }
        [Required] public string PassportIssuedByWhom { get; set; }
        [Required] public DateTime Birthday { get; set; }
        //для жены-мужа
        [Required] public string Gender { get; set; }

        public bool isUser { get; set; }
    }
}
