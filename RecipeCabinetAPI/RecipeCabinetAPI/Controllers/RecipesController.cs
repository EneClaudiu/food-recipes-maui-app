using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeCabinetAPI.Converters;
using RecipeCabinetAPI.Data;
using RecipeCabinetAPI.Models;
using RecipeCabinetAPI.Services;
using System.Text.Json;

namespace RecipeCabinetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;

        public RecipesController(ApplicationDbContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        // POST: api/Recipes
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromForm] string recipeJson, IFormFile imageFile)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            };

            var recipe = System.Text.Json.JsonSerializer.Deserialize<Recipe>(recipeJson, jsonSerializerOptions);
            if (recipe == null)
            {
                return BadRequest("Invalid recipe data.");
            }

            if (!TryValidateModel(recipe))
            {
                return BadRequest(ModelState);
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    var fileName = $"recipes/{Guid.NewGuid()}.jpg";
                    using (var stream = imageFile.OpenReadStream())
                    {
                        recipe.ImageUrl = await _s3Service.UploadFileAsync(stream, fileName);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Failed to upload image: " + ex.Message);
                }
            }

            recipe.Id = 0;
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // PUT: api/Recipes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }

            _context.Entry(recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!RecipeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAllRecipes([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var recipes = await _context.Recipes
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(recipes);
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Recipe>>> FilterRecipes([FromQuery] string? searchQuery, [FromQuery] List<string>? ingredients,
            [FromQuery] float? rating, [FromQuery] int? cookTime, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] bool randomize = false)
        {
            var query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(r => r.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            if (ingredients != null && ingredients.Count > 0)
            {
                foreach (var ingredient in ingredients)
                {
                    query = query.Where(r => r.Ingredients.ToLower().Contains(ingredient.ToLower()));
                }
            }

            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating >= rating.Value);
            }

            if (cookTime.HasValue)
            {
                query = query.Where(r => r.CookTime <= cookTime.Value);
            }

            if (randomize)
            {
                query = query.OrderBy(r => Guid.NewGuid());
            }

            var recipes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(recipes);
        }
    }
}
