using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.Shared.Abstractions.Services
{
    public interface IGameUserService
    {
        public Task RemoveUserFromGameAsync(Guid userGameId);
        public Task RemoveUserFromGameAsync(Guid gameId, Guid userId);
        public Task<Guid> AddPlayerToGameAsync(Guid userId, Guid gameId);
    }
}
