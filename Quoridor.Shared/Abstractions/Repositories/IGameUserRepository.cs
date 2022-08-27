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
        public Task<GameUser> GetAsync(Guid gameId, Guid userId);
        public Task<List<GameUser>> GetByUserIdAsync(Guid userId, bool? isActive = null);
        public Task<List<GameUser>> GetByGameIdAsync(Guid gameId, bool? isActive = null);
        public Task<int> GetCountConnectedUsersToGameAsync(Guid gameId);
        public Task ChangeUsersTurnAsync(Guid userId, Guid gameId, bool isAvailableToMove);
        public Task RemoveUserFromGameAsync(Guid userGameId);
        public Task RemoveUserFromGameAsync(Guid gameId, Guid userId);
    }
}
