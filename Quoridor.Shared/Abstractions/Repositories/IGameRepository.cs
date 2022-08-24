using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;

namespace Quoridor.Shared.Abstractions.Repositories
{
    public interface IGameRepository
    {
        public Task<Guid> CreateGameAsync(Game game);
        public Task<Game> GetByIdAsync(Guid id);
    }
}
