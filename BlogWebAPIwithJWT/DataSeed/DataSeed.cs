using BlogWithJWT;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;

namespace BlogWithJWT.DataSeed{

    public static class DataSeed {
        public static async Task<List<Post>> AddPosts(int numberToAdd)
        {
            var url = $"https://random-data-api.com/api/lorem_ipsum/random_lorem_ipsum?size={numberToAdd}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                Console.WriteLine(responseStream.ToString());
                var result = await JsonSerializer.DeserializeAsync<LoremIpsum>(responseStream);

                return new List<Post>();
            }
            else
            {
                return new List<Post>();
            }
        }
    }
}