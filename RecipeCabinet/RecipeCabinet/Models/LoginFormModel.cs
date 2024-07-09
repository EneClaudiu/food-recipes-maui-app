using System.ComponentModel.DataAnnotations;

namespace RecipeCabinet.Models
{
    public class LoginFormModel
    {
        [Display(Prompt = "example@mail.com", Name = "Email")]
        [EmailAddress(ErrorMessage = "Enter your email - example@mail.com")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Enter the password")]
        public string Password { get; set; } = string.Empty;
    }
}
