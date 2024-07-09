using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RecipeCabinetAPI.Data;
using RecipeCabinetAPI.Models;

namespace RecipeCabinetAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeRatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        public RecipeRatingsController(ApplicationDbContext context)
        {
            _context = context;
            _connectionString = context.Database.GetConnectionString();
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateRecipe([FromBody] RecipeRatings rating)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Call the rate_recipe stored procedure that updates the rating for a recipe after inserting/updating a rating
                using (var cmd = new NpgsqlCommand("CALL rate_recipe(@userEmail, @recipeId, @rating)", conn))
                {
                    cmd.Parameters.AddWithValue("userEmail", rating.UserEmail);
                    cmd.Parameters.AddWithValue("recipeId", rating.RecipeId);
                    cmd.Parameters.AddWithValue("rating", rating.Rating);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok();
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<IEnumerable<RecipeRatings>>> GetRatingsForRecipe(int recipeId)
        {
            return await _context.RecipeRatings.Where(rr => rr.RecipeId == recipeId).ToListAsync();
        }

        [HttpGet("user/{userEmail}")]
        public async Task<ActionResult<IEnumerable<RecipeRatings>>> GetRatingsByUser(string userEmail)
        {
            return await _context.RecipeRatings.Where(rr => rr.UserEmail == userEmail).ToListAsync();
        }

        [HttpGet("{userEmail}/{recipeId}")]
        public async Task<ActionResult<RecipeRatings>> GetRating(string userEmail, int recipeId)
        {
            var rating = await _context.RecipeRatings
                .FirstOrDefaultAsync(rr => rr.UserEmail == userEmail && rr.RecipeId == recipeId);

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }
    }
}
