using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.DTO.InputModels.Game;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.DataAccess.Repositories
{
    public class GameBoardRepository : BaseRepository, IGameBoardRepository
    {
        private readonly ILogger<GameBoardRepository> logger;

        public GameBoardRepository(
            ConnectionStringConfiguration connectionStringConfiguration,
            ILogger<GameBoardRepository> logger)
            : base(connectionStringConfiguration.Main)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> CreateAsync(GameBoard gameBoard)
        {
            var sql = @"
                        INSERT INTO [dbo].[T_Game_Board]
                        (
	                        [FK_Game],
                            [FK_User],
                            [Position_X],
                            [Position_Y],
                            [Game_Board_Entity_Type],
                            [Order]
                        )
                        OUTPUT INSERTED.PK_Game_Board
                        VALUES
                        (
                            @GameId,
                            @UserId,
                            @PositionX,
                            @PositionY,
                            @GameBoardEntityType,
                            @Order
                        )";
            var gameBoardId = await this.QuerySingleAsync<Guid>(sql, gameBoard);
            return gameBoardId;
        }

        public async Task<List<GameBoard>> GetLastMoveAsync(Guid gameId)
        {
            var sql = @"
                        SELECT
                            [PK_Game_Board] AS Id,
	                        [FK_Game] AS GameId,
                            [FK_User] AS UserId,
	                        [Position_X] AS PositionX,
                            [Position_Y] AS PositionY,
                            [Game_Board_Entity_Type] AS GameBoardEntityType,
                            [Order],
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_Board]
                        WHERE 
                            [FK_Game] = @GameId
                            AND [Order] = (
                                SELECT TOP 1 [Order]
                                FROM [dbo].[T_Game_Board]
                                WHERE [FK_Game] = @GameId
                                ORDER BY [Order] DESC
                            )";
            var gameBoard = await this.QueryAsync<GameBoard>(sql, new { GameId = gameId }).ConfigureAwait(false);
            return gameBoard;
        }

        public async Task<GameBoard> GetLastPawnMoveAsync(Guid gameId, Guid userId)
        {
            var sql = @"
                        SELECT TOP 1
                            [PK_Game_Board] AS Id,
	                        [FK_Game] AS GameId,
                            [FK_User] AS UserId,
	                        [Position_X] AS PositionX,
                            [Position_Y] AS PositionY,
                            [Game_Board_Entity_Type] AS GameBoardEntityType,
                            [Order],
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_Board]
                        WHERE 
                            [FK_Game] = @GameId AND [FK_User] = @UserId AND [Game_Board_Entity_Type] = @GameBoardEntityType
                        ORDER BY [Order] DESC";
            var gameBoard = await this.QueryFirstOrDefaultAsync<GameBoard>(sql, new { GameId = gameId, UserId = userId, GameBoardEntityType = GameBoardEntityType.Player }).ConfigureAwait(false);
            return gameBoard;
        }

        public async Task<List<GameBoard>> GetWallsAsync(Guid gameId)
        {
            var sql = @"
                        SELECT
                            [PK_Game_Board] AS Id,
	                        [FK_Game] AS GameId,
                            [FK_User] AS UserId,
	                        [Position_X] AS PositionX,
                            [Position_Y] AS PositionY,
                            [Game_Board_Entity_Type] AS GameBoardEntityType,
                            [Order],
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_Board]
                        WHERE [FK_Game] = @GameId AND [Game_Board_Entity_Type] = @GameBoardEntityType";
            var walls = await this.QueryAsync<GameBoard>(sql, new { GameId = gameId, GameBoardEntityType = GameBoardEntityType.Wall }).ConfigureAwait(false);
            return walls;
        }

        public async Task<bool> IsWallExistsOnPositionAsync(Guid gameId, Position position)
        {
            var sql = @"
                        SELECT CASE WHEN 
	                        EXISTS (
		                        SELECT TOP 1 PK_Game_board
                                FROM   dbo.T_Game_Board
                                WHERE  FK_Game = @GameId AND Position_X = @PositionX AND Position_Y = @PositionY AND [Game_Board_Entity_Type] = @GameBoardEntityType
	                        ) 
	                        THEN 1
                            ELSE 0
	                        END";
            var isExists = await this.QuerySingleAsync<bool>(sql, new { GameId = gameId, PositionX = position.X, PositionY = position.Y, GameBoardEntityType = GameBoardEntityType.Wall }).ConfigureAwait(false);
            return isExists;
        }

        public async Task<bool> IsEnemyExistsOnPositionAsync(Guid gameId, Guid playerId, Position position)
        {
            var sql = @"
                        SELECT CASE WHEN 
	                        LTRIM(@PositionX)+'_'+LTRIM(@PositionY) = (
		                        SELECT TOP 1 LTRIM([Position_X])+'_'+LTRIM([Position_Y])
                                FROM   dbo.T_Game_Board
                                WHERE  FK_Game = @GameId AND [FK_User] <> @UserId
                                ORDER BY [Order] DESC
	                        ) 
	                        THEN 1
                            ELSE 0
	                        END";
            var isExists = await this.QuerySingleAsync<bool>(sql, new { GameId = gameId, UserId = playerId, PositionX = position.X, PositionY = position.Y }).ConfigureAwait(false);
            return isExists;
        }
    }
}
