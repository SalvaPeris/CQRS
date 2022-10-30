using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            //Se inicializa la aplicación
            if(_httpContextAccessor is null || _httpContextAccessor.HttpContext is null)
            {
                User = new CurrentUser(Guid.Empty.ToString(), string.Empty, false);
                return;
            }

            // El Http Request existe pero es un usuario no autenticado
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext!.User!.Identity!.IsAuthenticated == true)
            {
                User = new CurrentUser(Guid.Empty.ToString(), string.Empty, false);
                return;
            }

            var id = httpContext.User.Claims
                .FirstOrDefault(q => q.Type == ClaimTypes.Sid)!.Value;

            var userName = httpContext.User.Identity!.Name ?? "Desconocido";

            User = new CurrentUser(id!, userName, true);

        }

        public CurrentUser User { get; }

        public bool IsInRole(string roleName) => _httpContextAccessor.HttpContext!.User.IsInRole(roleName);

    }
}
