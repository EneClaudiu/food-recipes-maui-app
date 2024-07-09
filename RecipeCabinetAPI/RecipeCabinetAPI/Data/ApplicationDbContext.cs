using Microsoft.EntityFrameworkCore;
using RecipeCabinetAPI.Models;

namespace RecipeCabinetAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<RecipeRatings> RecipeRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>().Property(r => r.Source).HasConversion<int>();

            modelBuilder.Entity<RecipeRatings>()
                .HasKey(rr => new { rr.UserEmail, rr.RecipeId });
        }

    }
}
