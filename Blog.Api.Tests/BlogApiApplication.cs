

class BlogApiApplication : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var root = new InMemoryDatabaseRoot();

        builder.ConfigureServices(services => {
            services.RemoveAll(typeof(DbContextOptions<BlogDb>));
            services.AddDbContext<BlogDb>(options => 
                options.UseInMemoryDatabase("Testing", root));
        });
        
        return base.CreateHost(builder);
    }
}