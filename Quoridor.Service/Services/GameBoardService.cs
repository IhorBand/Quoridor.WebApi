using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.Abstractions.Services;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.DTO.InputModels.Game;
using Quoridor.Shared.Exceptions;

namespace Quoridor.Service.Services
{
    public class GameBoardService : IGameBoardService
    {
        private readonly ILogger<GameBoardService> logger;
        private readonly IGameBoardRepository gameBoardRepository;
        private readonly IGameUserRepository gameUserRepository;

        public GameBoardService(
            ILogger<GameBoardService> logger,
            IGameBoardRepository gameBoardRepository,
            IGameUserRepository gameUserRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gameBoardRepository = gameBoardRepository ?? throw new ArgumentNullException(nameof(gameBoardRepository));
            this.gameUserRepository = gameUserRepository ?? throw new ArgumentNullException(nameof(gameUserRepository));
        }

        public async Task<bool> MakeMoveAsync(Guid gameId, Guid userId, Direction direction)
        {
            var gameUserRecord = await this.gameUserRepository.GetAsync(gameId, userId).ConfigureAwait(false);
            if (gameUserRecord == null)
            {
                throw new EmptyBoardException();
            }

            if (gameUserRecord.Id == Guid.Empty)
            {
                throw new EmptyBoardException();
            }

            if (gameUserRecord.IsAbleToMove == false)
            {
                throw new AnotherPlayerMoveException();
            }

            var lastMove = await this.gameBoardRepository.GetLastPawnMoveAsync(gameId, userId).ConfigureAwait(false);

            if (lastMove == null || lastMove.Id == Guid.Empty)
            {
                throw new EmptyBoardException();
            }

            Position newPosition = new Position(lastMove);
            Position possibleWallPosition = new Position(lastMove);

            if (direction == Direction.Up)
            {
                possibleWallPosition.Y += 1;
                newPosition.Y += 2;
            }
            else if (direction == Direction.Down)
            {
                possibleWallPosition.Y -= 1;
                newPosition.Y -= 2;
            }
            else if (direction == Direction.Right)
            {
                possibleWallPosition.X += 1;
                newPosition.X += 2;
            }
            else if (direction == Direction.Left)
            {
                possibleWallPosition.X -= 1;
                newPosition.X -= 2;
            }

            if ((gameUserRecord.DirectionToWin == Direction.Up && newPosition.Y > 17)
                || (gameUserRecord.DirectionToWin == Direction.Down && newPosition.Y < 1))
            {
                await this.gameBoardRepository.CreateAsync(new GameBoard()
                {
                    GameBoardEntityType = GameBoardEntityType.Player,
                    GameId = gameId,
                    Order = lastMove.Order + 2,
                    PositionX = newPosition.X,
                    PositionY = newPosition.Y,
                    UserId = userId
                }).ConfigureAwait(false);

                var gameUsers = await this.gameUserRepository.GetByGameIdAsync(gameId).ConfigureAwait(false);

                foreach (var gameUser in gameUsers)
                {
                    await this.gameUserRepository.RemoveUserFromGameAsync(gameUser.Id).ConfigureAwait(false);
                }

                return true;
            }

            if (newPosition.Y > 17 || newPosition.Y < 1 || newPosition.X > 17 || newPosition.X < 1)
            {
                throw new InvalidPositionException();
            }

            var isWallExists = await this.gameBoardRepository.IsWallExistsOnPositionAsync(gameId, possibleWallPosition).ConfigureAwait(false);
            if (isWallExists)
            {
                throw new InvalidPositionException();
            }

            var isEnemyOnNewPosition = await this.gameBoardRepository.IsEnemyExistsOnPositionAsync(gameId, userId, newPosition).ConfigureAwait(false);
            if (isEnemyOnNewPosition)
            {
                throw new InvalidPositionException();
            }

            await this.gameBoardRepository.CreateAsync(new GameBoard()
            {
                GameBoardEntityType = GameBoardEntityType.Player,
                GameId = gameId,
                Order = lastMove.Order + 2,
                PositionX = newPosition.X,
                PositionY = newPosition.Y,
                UserId = userId
            }).ConfigureAwait(false);

            await this.gameUserRepository.ChangeUsersTurnAsync(userId, gameId, false).ConfigureAwait(false);

            return false;
        }

        public async Task BuildWallAsync(Guid gameId, Guid userId, Position positionStart, Position positionEnd)
        {
            if (positionStart.X % 2 == 0 && positionStart.Y % 2 == 0)
            {
                // diagonal squares(position). not supported.
                throw new InvalidPositionException();
            }

            if (positionEnd.X % 2 == 0 && positionEnd.Y % 2 == 0)
            {
                // diagonal squares(position). not supported.
                throw new InvalidPositionException();
            }

            if (positionStart.X != positionEnd.X && positionStart.Y != positionEnd.Y)
            {
                // wall is not straight
                throw new InvalidPositionException();
            }

            if (positionStart.X != positionEnd.X && Math.Abs(positionStart.X - positionEnd.X) > 2)
            {
                // too large distance for wall
                throw new InvalidPositionException();
            }

            if (positionStart.Y != positionEnd.Y && Math.Abs(positionStart.Y - positionEnd.Y) > 2)
            {
                // too large distance for wall
                throw new InvalidPositionException();
            }

            var gameUserRecords = await this.gameUserRepository.GetByGameIdAsync(gameId).ConfigureAwait(false);
            var enemyGameUserRecord = gameUserRecords.First(g => g.UserId != userId);
            var playerGameUserRecord = gameUserRecords.First(g => g.UserId == userId);

            if (playerGameUserRecord == null)
            {
                throw new EmptyBoardException();
            }

            if (playerGameUserRecord.Id == Guid.Empty)
            {
                throw new EmptyBoardException();
            }

            if (playerGameUserRecord.IsAbleToMove == false)
            {
                throw new AnotherPlayerMoveException();
            }

            var playerPawn = await this.gameBoardRepository.GetLastPawnMoveAsync(gameId, userId).ConfigureAwait(false);
            var enemyPawn = await this.gameBoardRepository.GetLastPawnMoveAsync(gameId, enemyGameUserRecord.UserId).ConfigureAwait(false);

            var walls = await this.gameBoardRepository.GetWallsAsync(gameId).ConfigureAwait(false);
            if (walls.Count > 2)
            {
                if (this.CheckForDeadEnd(new Position(playerPawn), new Position(enemyPawn), walls, playerGameUserRecord.DirectionToWin, new List<Position>()))
                {
                    throw new InvalidPositionException();
                }

                if (this.CheckForDeadEnd(new Position(enemyPawn), new Position(playerPawn), walls, enemyGameUserRecord.DirectionToWin, new List<Position>()))
                {
                    throw new InvalidPositionException();
                }
            }

            await this.gameBoardRepository.CreateAsync(
                new GameBoard()
                {
                    GameBoardEntityType = GameBoardEntityType.Wall,
                    GameId = gameId,
                    UserId = userId,
                    Order = enemyPawn.Order + 1,
                    PositionX = positionStart.X,
                    PositionY = positionStart.Y
                }).ConfigureAwait(false);

            await this.gameBoardRepository.CreateAsync(
                new GameBoard()
                {
                    GameBoardEntityType = GameBoardEntityType.Wall,
                    GameId = gameId,
                    UserId = userId,
                    Order = enemyPawn.Order + 1,
                    PositionX = positionEnd.X,
                    PositionY = positionEnd.Y
                }).ConfigureAwait(false);

            await this.gameUserRepository.ChangeUsersTurnAsync(userId, gameId, false).ConfigureAwait(false);
        }

        public async Task<GameBoard> GetLastPawnMoveAsync(Guid gameId, Guid userId)
        {
            return await this.gameBoardRepository.GetLastPawnMoveAsync(gameId, userId).ConfigureAwait(false);
        }

        private bool CheckForDeadEnd(Position currentPosition, Position enemyPawn, List<GameBoard> walls, Direction direction, List<Position> alreadyChecked)
        {
            if (alreadyChecked.Exists(p => p.X == currentPosition.X && p.Y == currentPosition.Y))
            {
                return true;
            }

            alreadyChecked.Add(currentPosition);

            if (currentPosition.X % 2 == 0 && currentPosition.Y % 2 == 0)
            {
                return true;
            }

            if (currentPosition.X == enemyPawn.X && currentPosition.Y == enemyPawn.X)
            {
                return true;
            }

            foreach (var wall in walls)
            {
                if (currentPosition.X == wall.PositionX && currentPosition.Y == wall.PositionY)
                {
                    return true;
                }
            }

            if ((direction == Direction.Up && currentPosition.Y + 1 >= 17)
                || (direction == Direction.Down && currentPosition.Y - 1 <= 1))
            {
                return false;
            }

            if (direction == Direction.Down && currentPosition.Y + 1 < 17)
            {
                var up = this.CheckForDeadEnd(new Position(currentPosition.X, currentPosition.Y + 1), enemyPawn, walls, direction, alreadyChecked);
                if (up == false)
                {
                    return false;
                }
            }

            if (direction == Direction.Up && currentPosition.Y - 1 > 1)
            {
                var down = this.CheckForDeadEnd(new Position(currentPosition.X, currentPosition.Y - 1), enemyPawn, walls, direction, alreadyChecked);
                if (down == false)
                {
                    return false;
                }
            }

            if (currentPosition.X + 1 <= 17)
            {
                var right = this.CheckForDeadEnd(new Position(currentPosition.X + 1, currentPosition.Y), enemyPawn, walls, direction, alreadyChecked);
                if (right == false)
                {
                    return false;
                }
            }

            if (currentPosition.X - 1 >= 1)
            {
                var left = this.CheckForDeadEnd(new Position(currentPosition.X - 1, currentPosition.Y), enemyPawn, walls, direction, alreadyChecked);
                if (left == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
