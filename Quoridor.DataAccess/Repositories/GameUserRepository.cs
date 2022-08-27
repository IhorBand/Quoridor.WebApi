using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.DataAccess.Repositories
{
    public class GameUserRepository : BaseRepository, IGameUserRepository
    {
        private readonly ILogger<GameUserRepository> logger;

        public GameUserRepository(
            ConnectionStringConfiguration connectionStringConfiguration,
            ILogger<GameUserRepository> logger)
            : base(connectionStringConfiguration.Main)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Guid> CreateAsync(GameUser gameUser)
        {
            var sql = @"
                        INSERT INTO [dbo].[T_Game_User]
                        (
                            [FK_User],
	                        [FK_Game],
                            [Is_Active],
                            [Is_Able_To_Move],
                            [Direction_To_Win]
                        )
                        OUTPUT INSERTED.PK_Game_User
                        VALUES
                        (
                            @UserId,
                            @GameId,
                            @IsActive,
                            @IsAbleToMove,
                            @DirectionToWin
                        )";
            var gameUserId = await this.QuerySingleAsync<Guid>(sql, gameUser);
            return gameUserId;
        }

        public async Task<GameUser> GetAsync(Guid gameId, Guid userId)
        {
            var sql = @"
                        SELECT
                            [PK_Game_User] AS Id,
                            [FK_User] AS UserId,
	                        [FK_Game] AS GameId,
	                        [Is_Active] AS IsActive,
                            [Is_Able_To_Move] AS IsAbleToMove,
                            [Direction_To_Win] AS DirectionToWin,
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_Game] = @GameId AND [FK_User] = @UserId ";

            var gameUser = await this.QueryFirstOrDefaultAsync<GameUser>(sql, new { GameId = gameId, UserId = userId });
            return gameUser;
        }

        public async Task<List<GameUser>> GetByGameIdAsync(Guid gameId, bool? isActive = null)
        {
            var sql = @"
                        SELECT
                            [PK_Game_User] AS Id,
                            [FK_User] AS UserId,
	                        [FK_Game] AS GameId,
	                        [Is_Active] AS IsActive,
                            [Is_Able_To_Move] AS IsAbleToMove,
                            [Direction_To_Win] AS DirectionToWin,
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_Game] = @GameId ";

            if(isActive.HasValue)
            {
                sql += " AND [Is_Active] = @IsActive ";
            }

            sql += " ORDER BY [CreatedDateUTC] DESC ";

            var gameUser = await this.QueryAsync<GameUser>(sql, new { GameId = gameId, IsActive = isActive });
            return gameUser;
        }

        public async Task<List<GameUser>> GetByUserIdAsync(Guid userId, bool? isActive = null)
        {
            var sql = @"
                        SELECT
                            [PK_Game_User] AS Id,
                            [FK_User] AS UserId,
	                        [FK_Game] AS GameId,
	                        [Is_Active] AS IsActive,
                            [Is_Able_To_Move] AS IsAbleToMove,
                            [Direction_To_Win] AS DirectionToWin,
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_User] = @UserId ";

            if (isActive.HasValue)
            {
                sql += " AND [Is_Active] = @IsActive ";
            }

            sql += " ORDER BY [CreatedDateUTC] DESC ";

            var gameUser = await this.QueryAsync<GameUser>(sql, new { UserId = userId, IsActive = isActive });
            return gameUser;
        }

        public async Task<int> GetCountConnectedUsersToGameAsync(Guid gameId)
        {
            var sql = @"
                        SELECT Count(*)
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_Game] = @GameId";

            var gameUser = await this.QuerySingleAsync<int>(sql, new { GameId = gameId });
            return gameUser;
        }

        public async Task ChangeUsersTurnAsync(Guid userId, Guid gameId, bool isAvailableToMove)
        {
            var sql = @"
                        UPDATE [dbo].[T_Game_User]
                        SET [Is_Able_To_Move] = @IsAvailableToMove
                        WHERE [FK_Game] = @GameId AND [FK_User] = @UserId";
            await this.ExecuteAsync(sql, new { GameId = gameId, UserId = userId, IsAvailableToMove = isAvailableToMove });
            
            sql = @"
                        UPDATE [dbo].[T_Game_User]
                        SET [Is_Able_To_Move] = @IsAvailableToMove
                        WHERE [FK_Game] = @GameId AND [FK_User] <> @UserId";
            await this.ExecuteAsync(sql, new { GameId = gameId, UserId = userId, IsAvailableToMove = !isAvailableToMove });
        }

        public async Task RemoveUserFromGameAsync(Guid userGameId)
        {
            var sql = @"
                        UPDATE [dbo].[T_Game_User]
                        SET [Is_Active] = 0
                        WHERE [PK_Game_User] = @UserGameId";
            await this.ExecuteAsync(sql, new { UserGameId = userGameId });
        }

        public async Task RemoveUserFromGameAsync(Guid gameId, Guid userId)
        {
            var sql = @"
                        UPDATE [dbo].[T_Game_User]
                        SET [Is_Active] = 0
                        WHERE [FK_Game] = @GameId AND [FK_User] = @UserId";
            await this.ExecuteAsync(sql, new { GameId = gameId, UserId = userId });
        }
    }
}
