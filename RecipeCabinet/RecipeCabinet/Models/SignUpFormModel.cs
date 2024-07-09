using System.ComponentModel.DataAnnotations;

namespace RecipeCabinet.Models
{
    public class SignUpFormModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Enter a username")]
        public string Username { get; set; } = string.Empty;

        [Display(Prompt = "example@mail.com", Name = "Email")]
        [EmailAddress(ErrorMessage = "Enter your email - example@mail.com")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password must not contain spaces")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
