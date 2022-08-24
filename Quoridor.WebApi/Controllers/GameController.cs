using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.InputModels.Game;
using Quoridor.Shared.DTO.InputModels.Token;

namespace Quoridor.WebApi.Controllers
{
    [Route("api/game")]
    [ApiController]
    public class GameController : UserControllerBase
    {
        private readonly ILogger<GameController> logger;
        private readonly IMapper mapper;
        private readonly IGameService gameService;

        public GameController(
            ILogger<GameController> logger,
            IMapper mapper,
            IGameService gameService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.gameService = gameService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateGameAsync(CreateGameModel model)
        {
            if (model != null)
            {
                try
                {
                    var result = await this.gameService.CreateGameAsync(model, this.UserId).ConfigureAwait(false);
                    return this.Ok(result);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "ERROR [CreateGameAsync]");
                    return this.BadRequest(ex.Message);
                }
            }
            else
            {
                return this.BadRequest("Please, fill input fields.");
            }
        }
    }
}
