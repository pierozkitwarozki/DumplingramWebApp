using System;
using System.ComponentModel.DataAnnotations;

namespace Dumplingram.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Hasło musi być dłuższe niż 6 znaków.")]
        public string Password { get; set; }
        public string Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
    }
}