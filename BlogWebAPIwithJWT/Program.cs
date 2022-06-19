using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using BlogWithJWT;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogDb>(options => 
    options.UseSqlite("Data Source=JWTBlog.db"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/posts", async (BlogDb db) =>
    await db.Posts.ToListAsync());

app.MapGet("/categories", async (BlogDb db) =>  
        await db.Categories.ToListAsync());

app.MapGet("/posts/{id}", async (int id, BlogDb db) =>
    await db.Posts.FindAsync(id)
        is Post post
            ? Results.Ok(post)
            : Results.NotFound("record"));

app.MapPost("/posts", async (Post post, BlogDb db) =>
{
    if(post.CategoryId == 0 || post.CategoryId > 3 ||
    String.IsNullOrWhiteSpace(post.Title) ||
    String.IsNullOrWhiteSpace(post.Contents))
        return Results.BadRequest("Posts require Title, Contents, and Category.");
    db.Posts.Add(post);
    await db.SaveChangesAsync();

    return Results.Created($"/posts/{post.Id}", post);
});

app.MapPut("/posts/{id}", async (int id, Post inputPost, BlogDb db) =>
{
    var post = await db.Posts.FindAsync(id);

    if (post is null) return Results.NotFound();

    post.Title = inputPost.Title;
    post.Contents = inputPost.Contents;
    post.CategoryId = inputPost.CategoryId;
    post.Timestamp = inputPost.Timestamp;

    await db.SaveChangesAsync();

    return Results.Created($"/posts/{post.Id}", post);
});

app.MapDelete("/posts/{id}", async (int id, BlogDb db) =>
{
    if (await db.Posts.FindAsync(id) is Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return Results.Ok(post);
    }

    return Results.NotFound();
});

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<BlogDb>();
    if(dataContext.Database.IsSqlite())
        dataContext.Database.Migrate();
}

app.Run();

namespace BlogWithJWT
{
    

    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public DateTime Timestamp {get; set;}
        public int CategoryId {get; set;}
    }
    public class Category {
        public int categoryId {get; set;}

        public string? CategoryName {get;set;}
    }

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