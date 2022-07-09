using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using BlogWebAPIwithJWT.Data;
using BlogWebAPIwithJWT.Services;

#region builder

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
builder.Services.AddScoped<IPostService, PostService>();
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
#endregion

#region app
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion


app.MapPost("/login", [AllowAnonymous] async (
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

app.MapGet("/posts", async (IPostService ps) => {
        return await ps.GetPostsAsync();
        }
    ).WithTags("Blog");

app.MapGet("/posts/{pageNumber}/{pageSize}", async (int pageNumber, int pageSize, IPostService ps) =>
    {
        var pg = new PageParameters() {PageNumber = pageNumber, 
                PageSize = pageSize};
        return await ps.GetPostsAsync(pg);
    }
).WithTags("Blog");

app.MapGet("/categories", async (BlogDb db) =>  
        await db.Categories.ToListAsync()).WithTags("Blog");

app.MapGet("/posts/{id}", async (int id,  IPostService ps) =>
    await ps.GetPostAsync(id)
            is Post post
                ? Results.Ok(post)
                : Results.NotFound("record")).WithTags("Blog");

app.MapPost("/posts", [Authorize] async (EditPost post, IPostService ps, BlogDb db) =>
{
    if(post.CategoryId == 0 || post.CategoryId > 3 ||
    String.IsNullOrWhiteSpace(post.Title) ||
    String.IsNullOrWhiteSpace(post.Contents))
        return Results.BadRequest("Posts require Title, Contents, and Category.");
    var basePost = (Post) post;
    db.Posts.Add(basePost);
    await db.SaveChangesAsync();
    EditPost AddedPost = await ps.GetPostAsync(post.Id);
    return Results.Created($"/posts/{post.Id}", AddedPost);
}).WithTags("Blog").RequireAuthorization();

app.MapPut("/posts/{id}", [Authorize] async (int id, Post inputPost, BlogDb db) =>
{
    var post = await db.Posts.FindAsync(id);

    if (post is null) return Results.NotFound();

    post.Title = inputPost.Title;
    post.Contents = inputPost.Contents;
    post.CategoryId = inputPost.CategoryId;
    post.Timestamp = inputPost.Timestamp;

    await db.SaveChangesAsync();

    return Results.Created($"/posts/{post.Id}", post);
}).WithTags("Blog").RequireAuthorization();

app.MapDelete("/posts/{id}", [Authorize] async (int id, BlogDb db) =>
{
    
    if (id == -1)
    {
        db.Posts.RemoveRange(db.Posts);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    if (await db.Posts.FindAsync(id) is Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        return Results.Ok(post);
    }

    return Results.NotFound();
}).WithTags("Blog").RequireAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<BlogDb>();
    if(dataContext.Database.IsSqlite())
        dataContext.Database.Migrate();    
}

app.Run();
