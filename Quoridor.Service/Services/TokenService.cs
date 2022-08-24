using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;

namespace Quoridor.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> logger;
        private readonly IUserRepository userRepository;
        private readonly JwtTokenConfiguration jwtTokenConfiguration;

        public TokenService(
            ILogger<TokenService> logger,
            IUserRepository userRepository,
            JwtTokenConfiguration jwtTokenConfiguration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.jwtTokenConfiguration = jwtTokenConfiguration ?? throw new ArgumentNullException(nameof(jwtTokenConfiguration));
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            var user = await this.userRepository.GetByEmailAndPasswordAsync(email, password).ConfigureAwait(false);

            if (user != null)
            {
                // create claims details based on the user information
                var claims = new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, this.jwtTokenConfiguration.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(JwtCustomClaimNames.UserId, user.Id.ToString()),
                        new Claim(JwtCustomClaimNames.DisplayName, user.DisplayName),
                        new Claim(JwtCustomClaimNames.UserName, user.UserName),
                        new Claim(JwtCustomClaimNames.Email, user.Email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtTokenConfiguration.Key));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    this.jwtTokenConfiguration.Issuer,
                    this.jwtTokenConfiguration.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(10),
                    signingCredentials: signIn);

                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var tokenResult = jwtSecurityTokenHandler.WriteToken(token);

                return tokenResult;
            }

            return string.Empty;
        }
    }
}
