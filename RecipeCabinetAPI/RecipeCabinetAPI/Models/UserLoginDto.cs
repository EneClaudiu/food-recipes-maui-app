using System.ComponentModel.DataAnnotations;

namespace RecipeCabinetAPI.Models
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } =  null!;
    }
}
