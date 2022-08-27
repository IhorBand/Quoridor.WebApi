using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.Exceptions;

namespace Quoridor.Service.Services
{
    public class GameUserService : IGameUserService
    {
        private readonly ILogger<GameUserService> logger;
        private readonly IGameRepository gameRepository;
        private readonly IGameUserRepository gameUserRepository;
        private readonly IGameBoardRepository gameBoardRepository;

        public GameUserService(
            ILogger<GameUserService> logger,
            IGameRepository gameRepository,
            IGameUserRepository gameUserRepository,
            IGameBoardRepository gameBoardRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            this.gameUserRepository = gameUserRepository ?? throw new ArgumentNullException(nameof(gameUserRepository));
            this.gameBoardRepository = gameBoardRepository ?? throw new ArgumentNullException(nameof(gameBoardRepository));
        }

        public async Task RemoveUserFromGameAsync(Guid userGameId)
        {
            await this.RemoveUserFromGameAsync(userGameId).ConfigureAwait(false);
        }

        public async Task RemoveUserFromGameAsync(Guid gameId, Guid userId)
        {
            await this.RemoveUserFromGameAsync(gameId, userId).ConfigureAwait(false);
        }

        /// <summary>
        ///   Add user with [userId] to game [gameId].
        ///   If game doesn't exists or already full -> exception.
        ///   If this user filled game (count of players in game = max players in game) -> change status of game to InProgress.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <param name="gameId">Id of game.</param>
        /// <returns>A <see cref="Guid"/> returns Id of T_Game_User Table.</returns>
        public async Task<Guid> AddPlayerToGameAsync(Guid userId, Guid gameId)
        {
            var game = await this.gameRepository.GetByIdAsync(gameId).ConfigureAwait(false);

            if (game != null && game.Status == GameStatus.WaitingForPlayers)
            {
                var gameIdsForUser = await this.gameUserRepository.GetByUserIdAsync(userId, true).ConfigureAwait(false);
                if (gameIdsForUser != null && gameIdsForUser.Count > 0)
                {
                    foreach (var userGame in gameIdsForUser)
                    {
                        await this.gameUserRepository.RemoveUserFromGameAsync(userGame.Id).ConfigureAwait(false);
                    }
                }

                var countConnectedUsersToGame = await this.gameUserRepository.GetCountConnectedUsersToGameAsync(gameId).ConfigureAwait(false);
                var isFirstConnection = countConnectedUsersToGame == 0;
                var gameUser = new GameUser()
                {
                    GameId = gameId,
                    UserId = userId,
                    IsActive = true,
                    IsAbleToMove = isFirstConnection ? true : false,
                    DirectionToWin = isFirstConnection ? Direction.Up : Direction.Down
                };

                var gameUserId = await this.gameUserRepository.CreateAsync(gameUser).ConfigureAwait(false);

                var userIdsInGame = await this.gameUserRepository.GetByGameIdAsync(gameId, true).ConfigureAwait(false);

                if (!isFirstConnection)
                {
                    List<GameBoard> initialPawns = new List<GameBoard>();

                    initialPawns = new List<GameBoard>()
                    {
                        new GameBoard()
                        {
                            GameBoardEntityType = GameBoardEntityType.Player,
                            GameId = gameId,
                            UserId = userIdsInGame.First(u => u.Id != userId).Id,
                            Order = 0,
                            PositionX = 9,
                            PositionY = 1
                        },
                        new GameBoard()
                        {
                            GameBoardEntityType = GameBoardEntityType.Player,
                            GameId = gameId,
                            UserId = userId,
                            Order = 0,
                            PositionX = 9,
                            PositionY = 17
                        }
                    };

                    foreach (var initialPawn in initialPawns)
                    {
                        await this.gameBoardRepository.CreateAsync(initialPawn).ConfigureAwait(false);
                    }

                    await this.gameRepository.UpdateStatusAsync(gameId, GameStatus.InProgress).ConfigureAwait(false);
                }

                return gameUserId;
            }
            else
            {
                throw new RoomFullException("Game is full.");
            }
        }
    }
}
