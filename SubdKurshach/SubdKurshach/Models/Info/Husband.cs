﻿using SubdKurshach.Models.Families;
using SubdKurshach.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdKurshach.Models.Info
{
    public class Husband
    {
        [Key] public int HusbandId { get; set; }
        [ForeignKey("User")] public string UserId { get; set; } public User User { get; set; }
        [Required] public Marriage Marriage { get; set; }
    }
}
