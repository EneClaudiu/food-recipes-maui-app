using System.ComponentModel.DataAnnotations;

namespace RecipeCabinetAPI.Models
{
    public class User
    {
        [Key]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
