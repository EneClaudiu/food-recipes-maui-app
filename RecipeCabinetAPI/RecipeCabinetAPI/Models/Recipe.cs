using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeCabinetAPI.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Rating { get; set; } = 0;
        public string Ingredients { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int PrepTime { get; set; }
        public int CookTime { get; set; }
        public DateOnly DateCreated { get; set; }
        public bool IsSynced { get; set; } = false;

        public RecipeSource Source { get; set; } = RecipeSource.Remote;
    }

    public enum RecipeSource
    {
        Local = 0,
        Remote = 1
    }
}
