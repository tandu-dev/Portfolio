
namespace Blog.Api.Tests
{
     public static class AssertExtensions
    {
        public static void AreEqualByJson(object expected, object actual)
        {
           
            var expectedJson = JsonSerializer.Serialize(expected);
            var actualJson = JsonSerializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
        public static void AreNotEqualByJson(object expected, object actual)
        {
            var expectedJson = JsonSerializer.Serialize(expected);
            var actualJson = JsonSerializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }
}