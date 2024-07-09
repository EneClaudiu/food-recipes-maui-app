namespace RecipeCabinet.Models
{
    public class RecipeRatings
    {
        public string UserEmail { get; set; }
        public int RecipeId { get; set; }
        public float Rating { get; set; }
    }
}
