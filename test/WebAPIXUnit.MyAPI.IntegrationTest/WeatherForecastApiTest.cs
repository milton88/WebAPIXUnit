using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
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
                .UseEnvironment("Development")
                .UseConfiguration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json") // Use appsettings.json
                    .Build())
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


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        public async Task GetSettings(string method)
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "teste-integration");
            // Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var request = new HttpRequestMessage(new HttpMethod(method), "/weatherforecast/settings");

            // Act
            var response = await _client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();
            using var respBody = JsonDocument.Parse(content);

            var root = respBody.RootElement;


            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("teste-integration", root.GetProperty("envVariable").GetString());
            Assert.Equal("Information-teste", root.GetProperty("appSettingsLog").GetString());
        }
    }
}
