using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using BlogWithJWT;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogDb>(options => 
    options.UseSqlite("Data Source=JWTBlog.db"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "JWT Authentication",
            Description = "Enter JWT Bearer token **_only_**",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", // must be lower case
            BearerFormat = "JWT",
            Reference = new OpenApiReference
            {
                Id = JwtBearerDefaults.AuthenticationScheme,
                Type = ReferenceType.SecurityScheme
            }
        };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] { }}
    });
    //c.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
});
builder.Services.AddSingleton<TokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepositoryService>(new UserRepositoryService());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
});
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapPost("/login",   [AllowAnonymous] async (
    [FromBody] UserModel userModel,  TokenService tokenService, 
    IUserRepositoryService userRepositoryService, HttpResponse response) => {
    var userDto = userRepositoryService.GetUser(userModel);
    if (userDto == null)
    {
        response.StatusCode = 401;
        return;
    }
    var token = tokenService.BuildToken(
        builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"],
         builder.Configuration["Jwt:Audience"], userDto);
        await response.WriteAsJsonAsync(new { token = token });
        return;
    })
    .Produces(StatusCodes.Status200OK)
    .WithName("Login").WithTags("Accounts");

app.MapGet("/AuthorizedResource", (Func<string>)(
[Authorize] () => "Action Succeeded")
).Produces(StatusCodes.Status200OK)
.WithName("Authorized").WithTags("Accounts").RequireAuthorization();

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