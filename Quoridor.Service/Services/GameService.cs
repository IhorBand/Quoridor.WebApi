using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.DTO.InputModels.Game;

namespace Quoridor.Service.Services
{
    public class GameService : IGameService
    {
        private readonly ILogger<GameService> logger;
        private readonly IGameRepository gameRepository;
        private readonly IGameUserRepository gameUserRepository;

        public GameService(
            ILogger<GameService> logger,
            IGameRepository gameRepository,
            IGameUserRepository gameUserRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            this.gameUserRepository = gameUserRepository ?? throw new ArgumentNullException(nameof(gameUserRepository));
        }

        public async Task<List<Game>> GetAllAvailableSessionsAsync()
        {
            return await this.gameRepository.GetAllAvailableSessionsAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///   Creates new game.
        /// </summary>
        /// <param name="createGameModel">model to create a game record.</param>
        /// <param name="userCreatorId">Id of creator.</param>
        /// <returns>A <see cref="Guid"/> returns Id of created Game.</returns>
        public async Task<Guid> CreateGameAsync(CreateGameModel createGameModel, Guid userCreatorId)
        {
            if (createGameModel.MaxPlayers == 2)
            {
                var game = new Game()
                {
                    Name = createGameModel.Name,
                    MaxPlayers = createGameModel.MaxPlayers,
                    Status = GameStatus.WaitingForPlayers,
                    UserCreatorId = userCreatorId
                };

                var gameId = await this.gameRepository.CreateGameAsync(game).ConfigureAwait(false);
                return gameId;
            }
            else
            {
                throw new Exception("maxPlayers field must be 2 ( 4 players will be available soon :) ).");
            }
        }
    }
}
