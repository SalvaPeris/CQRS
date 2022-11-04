using ApplicationCore.Features.Auth.Command;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class TestBase
    {
        protected ApiWebApplicationFactory ApplicationFactory;

        /// <summary>
        /// Crea usuario de prueba
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<(HttpClient Client, string UserId)> CreateTestUser(string username, string password, string[] roles)
        {
            var scope = ApplicationFactory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var newUser = new IdentityUser(username);

            await userManager.CreateAsync(newUser, password);

            foreach (var role in roles)
            {
                await userManager.AddToRoleAsync(newUser, role);
            }

            var accesToken = await GetAccessToken(username, password);

            var client = ApplicationFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesToken);

            return (client, newUser.Id);
        }

        /// <summary>
        /// Shortcut para ejecutar IRequest con el Mediador
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            using var scope = ApplicationFactory.Services.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

            return await mediator.Send(request);
        }

        public async Task<string> GetAccessToken(string username, string password)
        {
            using var scope = ApplicationFactory.Services.CreateScope();

            var result = await SendAsync( new TokenCommand
            {
                UserName = username,
                Password = password
            });

            return result.AccessToken;
        }


    }
}
