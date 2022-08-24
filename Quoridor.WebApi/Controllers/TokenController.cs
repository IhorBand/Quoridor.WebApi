using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.InputModels.Token;

namespace Quoridor.WebApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> logger;
        private readonly IMapper mapper;
        private readonly ITokenService tokenService;

        public TokenController(
            ILogger<TokenController> logger,
            IMapper mapper,
            ITokenService tokenService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> GetTokenAsync(AuthorizeUserModel model)
        {
            if (model != null && model.Email != null && model.Password != null)
            {
                var result = await this.tokenService.GetTokenAsync(model.Email, model.Password).ConfigureAwait(false);
                if (string.IsNullOrEmpty(result))
                {
                    return this.BadRequest("Invalid Credentials.");
                }

                return this.Ok(result);
            }
            else
            {
                return this.BadRequest("Please, fill email and password fields.");
            }
        }
    }
}
