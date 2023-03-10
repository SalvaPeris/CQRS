using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ApplicationCore.Common.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore.Features.Auth.Command
{
    public class TokenCommand : IRequest<TokenCommandResponse>
    {
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class TokenCommandHandler : IRequestHandler<TokenCommand, TokenCommandResponse>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public TokenCommandHandler(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<TokenCommandResponse> Handle(TokenCommand request, CancellationToken cancellation)
        {

            List<IdentityUser> users = _userManager.Users.ToList();

            foreach (var user1 in users)
            {
                Console.WriteLine("Usuario : " + user1);
            }

            //Verificamos credenciales
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new ForbiddenAccessException();

            var roles = await _userManager.GetRolesAsync(user);

            //Generamos token según los claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(720),
                    signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new TokenCommandResponse
            {
                AccessToken = jwt
            };
        }
    }

    public class TokenCommandResponse
    {
        public string AccessToken { get; set; } = default!;
    }
}
