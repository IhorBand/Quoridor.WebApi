using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.Enums;

namespace Quoridor.Shared.Abstractions.Repositories
{
    public interface IGameRepository
    {
        public Task<List<Game>> GetAllAvailableSessionsAsync();
        public Task<Guid> CreateGameAsync(Game game);
        public Task<Game> GetByIdAsync(Guid id);
        public Task UpdateStatusAsync(Guid gameId, GameStatus gameStatus);
    }
}
