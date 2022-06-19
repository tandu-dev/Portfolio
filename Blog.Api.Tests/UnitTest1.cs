using BlogWithJWT;
using System.Net.Http;
using System.Net.Http.Json;

namespace Blog.Api.Tests;

public class Tests
{
    private BlogApiApplication _application;
    
    [SetUp]
    public void Setup()
    {
        _application = new BlogApiApplication();
        using (var scope = _application.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            using (var blogDbContext = provider.GetRequiredService<BlogDb>())
            {
                blogDbContext.Database.EnsureCreatedAsync();

                 blogDbContext.Posts.AddAsync(new  Post { Id= 1,
                        Title= "Blog Post Title 1",
                        Contents= "<p>This is a blog post</p>",
                        Timestamp= DateTime.Now,
                        CategoryId= 1
                });
                blogDbContext.Posts.AddAsync(new  Post { Id= 2,
                        Title= "Blog Post Title 2",
                        Contents= "<p>This is a blog post</p>",
                        Timestamp= DateTime.Now,
                        CategoryId= 3
                });
                blogDbContext.SaveChangesAsync();
            }
        }

    }

    [Test]
    public async Task TestApplication()
    {
        

        var client = _application.CreateClient();
        var posts = await client.GetFromJsonAsync<List<Post>>("/posts");

        Assert.IsNotNull(posts);
        Assert.IsTrue(posts.Count == 2);
        
    }
    [Test]
    public async Task TestPosts()
    {     
        var client = _application.CreateClient();
        var blog = await client.GetFromJsonAsync<List<Post>>("/posts");

        Assert.IsNotNull(blog);
        Assert.IsTrue(blog.Count == 2);
    }
    [Test]
    public async Task TestOnePost()
    {
        var client = _application.CreateClient();
        var post = await client.GetFromJsonAsync<Post>("/posts/1");

        Assert.IsNotNull(post);
        Assert.IsTrue(post.Id == 1);
    }
    [Test]
    public async Task TestCategories()
    {
        var client = _application.CreateClient();
        var category = await client.GetFromJsonAsync<List<Category>>("/categories");

        Assert.IsNotNull(category);
        Assert.IsTrue(category.Count == 3);
    }
}