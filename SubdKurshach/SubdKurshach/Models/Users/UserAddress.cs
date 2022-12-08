using System.ComponentModel.DataAnnotations;

namespace SubdKurshach.Models.Users
{
    public class UserAddress
    {
        [Key] public int AddressId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNum { get; set; }
        public string? AppartNum { get; set; }

        [Required]public User User { get; set; }
    }
}
