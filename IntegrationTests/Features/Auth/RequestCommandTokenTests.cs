using System;
using ApplicationCore.Features.Auth.Command;
using FluentAssertions;
using NUnit.Framework;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace IntegrationTests.Features.Auth
{
    public class RequestTokenCommandTests : TestBase
    {

        [Test]
        public async Task User_CanLogin()
        {
            var client = ApplicationFactory.CreateClient();

            var result = await client.PostAsJsonAsync("api/auth", new TokenCommand
            {
                UserName = "test_user",
                Password = "Passw0rd.1234"
            });


            FluentActions.Invoking(() => result.EnsureSuccessStatusCode())
                .Should().NotThrow();

            var response = JsonSerializer.Deserialize<TokenCommandResponse>(result.Content.ReadAsStream(), new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            response.Should().NotBeNull();
            response?.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task User_CannotLogin()
        {
            var client = ApplicationFactory.CreateClient();

            var result = await client.PostAsJsonAsync("api/auth", new TokenCommand
            {
                UserName = "test_user",
                Password = "123456"
            });

            FluentActions.Invoking(() => result.EnsureSuccessStatusCode())
                .Should().Throw<HttpRequestException>();
            result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
