using System.ComponentModel.DataAnnotations;

namespace RecipeCabinet.Models
{
    public class ChangePasswordFormModel
    {
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password must not contain spaces")]
        public string NewPassword { get; set; } = string.Empty;

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
