using System.ComponentModel.DataAnnotations;

namespace RecipeCabinet.Models
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;
    }
}
