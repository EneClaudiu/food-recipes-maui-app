using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeCabinetAPI.Controllers;
using RecipeCabinetAPI.Data;
using RecipeCabinetAPI.Models;
using RecipeCabinetAPI.Services;
namespace RecipeCabinet.Tests
{
    public class RecipesControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<S3Service> _mockS3Service;
        private readonly RecipesController _controller;

        public RecipesControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "RecipeCabinetTest")
                .Options;
            _context = new ApplicationDbContext(options);

            var mockS3Client = new Mock<IAmazonS3>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.SetupGet(x => x["AWS:BucketName"]).Returns("test-bucket");

            _mockS3Service = new Mock<S3Service>(mockS3Client.Object, mockConfiguration.Object);
            _controller = new RecipesController(_context, _mockS3Service.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public async Task GetRecipe_ReturnsNotFound_ForInvalidId()
        {
            var result = await _controller.GetRecipe(-1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetRecipe_ReturnsRecipe_ForValidId()
        {
            var recipe = new Recipe { UserEmail = "testUser", Name = "Test Recipe" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            var result = await _controller.GetRecipe(recipe.Id);

            Assert.IsType<ActionResult<Recipe>>(result);
            var returnedRecipe = result.Value;
            Assert.Equal(recipe.Name, returnedRecipe.Name);
        }

        [Fact]
        public async Task PutRecipe_ReturnsBadRequest_ForMismatchedId()
        {
            var recipe = new Recipe { Id = 1, UserEmail = "testUser", Name = "Test Recipe" };

            var result = await _controller.PutRecipe(2, recipe);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task PutRecipe_ReturnsNoContent_ForValidUpdate()
        {
            var recipe = new Recipe { UserEmail = "testUser", Name = "Test Recipe" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            recipe.Name = "Updated Recipe";
            var result = await _controller.PutRecipe(recipe.Id, recipe);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRecipe_ReturnsNotFound_ForInvalidId()
        {
            var result = await _controller.DeleteRecipe(-1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteRecipe_ReturnsNoContent_ForValidId()
        {
            var recipe = new Recipe { UserEmail = "testUser", Name = "Test Recipe" };
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteRecipe(recipe.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetAllRecipes_ReturnsPagedResults()
        {
            for (int i = 0; i < 15; i++)
            {
                _context.Recipes.Add(new Recipe { UserEmail = "testUser", Name = $"Test Recipe {i}" });
            }
            await _context.SaveChangesAsync();

            var result = await _controller.GetAllRecipes(2, 5);

            Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
            var recipes = (result.Result as OkObjectResult).Value as List<Recipe>;
            Assert.Equal(5, recipes.Count);
        }

        [Fact]
        public async Task FilterRecipes_ReturnsFilteredResults()
        {
            var recipe1 = new Recipe { UserEmail = "testUser", Name = "Chicken Soup", Ingredients = "Chicken, Water" };
            var recipe2 = new Recipe { UserEmail = "testUser", Name = "Vegetable Soup", Ingredients = "Carrot, Water" };
            _context.Recipes.AddRange(recipe1, recipe2);
            await _context.SaveChangesAsync();

            var result = await _controller.FilterRecipes("Soup", new List<string> { "Chicken" }, null, null, 1, 10);

            Assert.IsType<ActionResult<IEnumerable<Recipe>>>(result);
            var recipes = (result.Result as OkObjectResult).Value as List<Recipe>;
            Assert.Single(recipes);
            Assert.Equal("Chicken Soup", recipes[0].Name);
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
