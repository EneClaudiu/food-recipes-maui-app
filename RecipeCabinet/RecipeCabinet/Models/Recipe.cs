using SQLite;

namespace RecipeCabinet.Models
{
    [Table("Recipes")]
    public class Recipe
    {
        [PrimaryKey, AutoIncrement]
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
        public DateTime DateCreated { get; set; } = DateTime.Now.Date;
        public bool IsSynced { get; set; } = false;

        public RecipeSource Source { get; set; } = RecipeSource.Local;
    }

    public enum RecipeSource
    {
        Local = 0,
        Remote = 1
    }
}
