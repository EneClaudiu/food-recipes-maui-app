using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RecipeCabinetAPI.Models
{
    public class RecipeRatings
    {
        [Key, Column(Order = 0)]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Key, Column(Order = 1)]
        public int RecipeId { get; set; }

        [Range(0, 5)]
        public float Rating { get; set; }
    }
}
