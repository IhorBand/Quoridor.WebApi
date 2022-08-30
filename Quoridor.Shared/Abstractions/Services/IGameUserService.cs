using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.OutputModels;

namespace Quoridor.Shared.Abstractions.Services
{
    public interface IGameUserService
    {
        public Task RemoveUserFromGameAsync(Guid userGameId);
        public Task RemoveUserFromGameAsync(Guid gameId, Guid userId);
        public Task<JoinGameOutputModel> AddPlayerToGameAsync(Guid userId, Guid gameId);
    }
}
