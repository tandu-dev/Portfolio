
using System.Threading.Tasks;

namespace DataSeeder {
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Please Enter the number of posts to add");
            var amt = Console.ReadLine();
            
            var PostList = new List<Post>();
            Random rnd = new Random();

            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://random-data-api.com/api/lorem_ipsum/random_lorem_ipsum?size={amt}");
            if (response.IsSuccessStatusCode)
            {
                var content = await JsonSerializer
                            .DeserializeAsync<IEnumerable<LoremIpsum>>(await
                                    response.Content.ReadAsStreamAsync());
                foreach (var li in content)
                {
                    var post = new Post()
                    {
                        Title = li.short_sentence,
                        Id = li.id,
                        Timestamp = DateTime.Now
                    };
                    foreach (var sentence in li.paragraphs)
                    {
                        post.Contents += sentence + " ";
                    }
                    post.Contents += li.very_long_sentence + " ";
                    post.Contents += li.long_sentence;
                    post.CategoryId = rnd.Next(1, 4);

                    PostList.Add(post);
                }
                
            }   
            //outside first response
            //login
            string login = "{\"userName\":\"admin\", \"password\":\"abc123\"}";
            HttpContent loginHC = new StringContent(login, System.Text.Encoding.UTF8, "application/json");
            var tokenResponse = await client.PostAsync("http://localhost:5142/login", loginHC);
            var token = await JsonSerializer.DeserializeAsync<MyJWTToken>(
                await tokenResponse.Content.ReadAsStreamAsync());
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.token}");
            foreach(var post in PostList)
            {
                string payload = JsonSerializer.Serialize<Post>(post);
                HttpContent hc = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
                response = await client.PostAsync("http://localhost:5142/posts", hc);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Posted {post.Title}, Id: {post.Id}");
                }
                else
                {
                    Console.WriteLine($"{response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            Console.WriteLine($"Posted {PostList.Count} records.");
        }
    }
}