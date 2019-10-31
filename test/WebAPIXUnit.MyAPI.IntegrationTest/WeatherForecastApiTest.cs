using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace WebAPIXUnit.MyAPI.IntegrationTest
{

    // Ref: https://www.infoq.com/br/articles/testing-aspnet-core-web-api/

    public class WeatherForecastApiTest
    {
        private readonly HttpClient _client;

        public WeatherForecastApiTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Developement")
                .UseStartup<Startup>());
            
            _client = server.CreateClient();
        }

        [Theory]
        [InlineData("GET")]
        public async Task Get(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/weatherforecast");

            // Act
            var response = await _client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();
            var respBody = JsonDocument.Parse(content);


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
