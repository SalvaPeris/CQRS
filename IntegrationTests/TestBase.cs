using ApplicationCore.Features.Auth.Command;
using ApplicationCore.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        /// Al terminar cada prueba, se resetea la BBDD.
        /// </summary>
        /// <returns></returns>
        [TearDown]
        public async Task Down()
        {
            await ResetState();
        }

        /// <summary>
        /// Crea un HttpClient con JWT válido con usuario ADMIN
        /// </summary>
        /// <returns></returns>
        public Task<(HttpClient Client, string UserId)> GetClientAsAdminUserAsync() => CreateTestUser("user@admin.com", "Passw0rd", new string[] { "Admin" });

        /// <summary>
        /// Crea un HttpClient cpn JWT válido con usuario DEFAULT
        /// </summary>
        /// <returns></returns>
        public Task<(HttpClient Client, string UserId)> GetClientAsDefaultUserAsync() => CreateTestUser("user@default.com", "Passw0rd", Array.Empty<string>());

        /// <summary>
        /// Libera recursos al terminar todas las pruebas
        /// </summary>
        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            ApplicationFactory.Dispose();
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            ApplicationFactory = new ApiWebApplicationFactory();
            EnsureDatabase();
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

        /// <summary>
        /// Shortcut para agregar Entidades
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task<TEntity> AddAsync <TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = ApplicationFactory.Services.CreateScope();

            var context = scope.ServiceProvider.GetService<MyAppDbContext>();

            context.Add(entity);

            await context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Shortcut para buscar Entidades según PK
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        protected async Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            using var scope = ApplicationFactory.Services.CreateScope();

            var context = scope.ServiceProvider.GetService<MyAppDbContext>();

            return await context.FindAsync<TEntity>(keyValues);
        }

        /// <summary>
        /// Shortcut para buscar Entidades según un criterio
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected async Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            using var scope = ApplicationFactory.Services.CreateScope();

            var context = scope.ServiceProvider.GetService<MyAppDbContext>();

            return await context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Se asegura de crear la BBDD
        /// </summary>
        private void EnsureDatabase()
        {
            using var scope = ApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<MyAppDbContext>();

            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Shortcut para autenticar un usuario para pruebas
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Limpiamos la BBDD
        /// </summary>
        /// <returns></returns>
        private async Task ResetState()
        {
            using var scope = ApplicationFactory.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<MyAppDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            await MyAppDbContextSeed.SeedProductsAsync(context);
        }
    }
}
