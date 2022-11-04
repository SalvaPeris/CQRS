using ApplicationCore.Features.Products.Queries;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace IntegrationTests.Features.Products
{
    public class GetProductsQueryTests : TestBase
    {
        [Test]
        public async Task Products_Obtained_With_Authenticated_User()
        {
            //Arrenge
            var (Client, UserID) = await GetClientAsAdminUserAsync();

            //Act
            var products = await Client.GetFromJsonAsync<List<GetProductsQueryResponse>>("/api/products");

            //Assert
            products.Should().NotBeNullOrEmpty();
            products.Count.Should().Be(2);
        }

        [Test]
        public async Task Products_ProducesException_WithAnonymUser()
        {
            // Arrenge
            var client = ApplicationFactory.CreateClient();

            // Act and Assert
            await FluentActions.Invoking(() =>
                    client.GetFromJsonAsync<List<GetProductsQueryResponse>>("/api/products"))
                        .Should().ThrowAsync<HttpRequestException>();
        }
    }
}
