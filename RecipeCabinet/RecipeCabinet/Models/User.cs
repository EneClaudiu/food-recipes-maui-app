using SQLite;
using System.ComponentModel.DataAnnotations;

namespace RecipeCabinet.Models
{
    public class User
    {
        [PrimaryKey]
        [EmailAddress(ErrorMessage = "Invalid email.")]
        public string Email { get; set; } = string.Empty;

        public string? Username { get; set; } = null;

        public string Password { get; set; } = string.Empty;
    }
}
