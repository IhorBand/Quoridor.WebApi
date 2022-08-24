using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;

namespace Quoridor.Shared.Abstractions.Repositories
{
    public interface IGameUserRepository
    {
        public Task<Guid> CreateAsync(GameUser gameUser);
        public Task<List<GameUser>> GetByUserIdAsync(Guid userId);
        public Task<List<GameUser>> GetByGameIdAsync(Guid gameId);
    }
}
