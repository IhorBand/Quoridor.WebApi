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
	                        [FK_Game]
                        )
                        OUTPUT INSERTED.PK_Game_User
                        VALUES
                        (
                            @UserId,
                            @GameId
                        )";
            var gameUserId = await this.QuerySingleAsync<Guid>(sql, gameUser);
            return gameUserId;
        }

        public async Task<List<GameUser>> GetByGameIdAsync(Guid gameId)
        {
            var sql = @"
                        SEELCT
                            [PK_Game_User] AS Id,
                            [FK_User] AS UserId,
	                        [FK_Game] AS GameId,
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_Game] = @GameId";
            var gameUser = await this.QueryAsync<GameUser>(sql, new { GameId = gameId});
            return gameUser;
        }

        public async Task<List<GameUser>> GetByUserIdAsync(Guid userId)
        {
            var sql = @"
                        SEELCT
                            [PK_Game_User] AS Id,
                            [FK_User] AS UserId,
	                        [FK_Game] AS GameId,
	                        [CreatedDateUTC]
                        FROM [dbo].[T_Game_User]
                        WHERE [FK_User] = @UserId";
            var gameUser = await this.QueryAsync<GameUser>(sql, new { UserId = userId });
            return gameUser;
        }
    }
}
