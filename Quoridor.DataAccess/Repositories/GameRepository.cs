using Microsoft.Extensions.Logging;
using Quoridor.Shared.Abstractions.Repositories;
using Quoridor.Shared.DTO.Configuration;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.DataAccess.Repositories
{
    public class GameRepository : BaseRepository, IGameRepository
    {
        private readonly ILogger<GameRepository> logger;

        public GameRepository(
            ConnectionStringConfiguration connectionStringConfiguration,
            ILogger<GameRepository> logger)
            : base(connectionStringConfiguration.Main)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> CreateGameAsync(Game game)
        {
            var sql = @"
                        INSERT INTO [dbo].[T_Game]
                        (
                            [Name],
                            [FK_User_Creator],
	                        [Max_Players],
	                        [Status]
                        )
                        OUTPUT INSERTED.PK_Game
                        VALUES
                        (
                            @Name,
                            @UserCreatorId,
                            @MaxPlayers,
                            @Status
                        )";
            var gameId = await this.QuerySingleAsync<Guid>(sql, game);
            return gameId;
        }

        public async Task<List<Game>> GetAllAvailableSessionsAsync()
        {
            var sql = @"
                    SELECT
                        [PK_Game] AS Id,
                        [Name],
                        [FK_User_Creator] AS UserCreatorId,
	                    [Max_Players] AS MaxPlayers,
	                    [Status],
                        [CreatedDateUTC]
                    FROM [dbo].[T_Game]
                    WHERE [Status] = @Status";
            var games = await this.QueryAsync<Game>(sql, new { Status = GameStatus.WaitingForPlayers });
            return games;
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            var sql = @"
                    SELECT
                        [PK_Game] AS Id,
                        [Name],
                        [FK_User_Creator] AS UserCreatorId,
	                    [Max_Players] AS MaxPlayers,
	                    [Status],
                        [CreatedDateUTC]
                    FROM [dbo].[T_Game]
                    WHERE [PK_Game] = @Id";
            var game = await this.QueryFirstOrDefaultAsync<Game>(sql, new { Id = id});
            return game;
        }

        public async Task UpdateStatusAsync(Guid gameId, GameStatus gameStatus)
        {
            var sql = @"
                    UPDATE [dbo].[T_Game] 
                    SET [Status] = @Status 
                    WHERE [PK_Game] = @GameId";
            await this.ExecuteAsync(sql, new { GameId = gameId, Status = gameStatus });
        }
    }
}
