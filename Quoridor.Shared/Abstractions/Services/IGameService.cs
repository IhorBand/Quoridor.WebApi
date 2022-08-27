using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.InputModels.Game;

namespace Quoridor.Shared.Abstractions.Services
{
    public interface IGameService
    {
        public Task<List<Game>> GetAllAvailableSessionsAsync();
        public Task<Guid> CreateGameAsync(CreateGameModel createGameModel, Guid userCreatorId);
    }
}
