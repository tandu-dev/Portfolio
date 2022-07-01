using Microsoft.EntityFrameworkCore;

namespace BlogWebAPIwithJWT.Data{

    public class BlogDb : DbContext
    {
        public BlogDb(DbContextOptions<BlogDb> options)
            : base(options) { }
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Category> Categories => Set<Category>();
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);
        
            modelbuilder.Entity<Category>().HasData(
                        new Category{categoryId = 1, CategoryName = "General"},
                        new Category {categoryId = 2, CategoryName = "Technology"},
                        new Category {categoryId = 3, CategoryName = "Random"}
            );
            
        }
    }
}