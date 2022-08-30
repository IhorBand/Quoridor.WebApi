using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.DatabaseEntities;
using Quoridor.Shared.DTO.InputModels.Game;

namespace Quoridor.Shared.Abstractions.Repositories
{
    public interface IGameBoardRepository
    {
        public Task<Guid> CreateAsync(GameBoard gameBoard);
        public Task<List<GameBoard>> GetLastMoveAsync(Guid gameId);
        public Task<GameBoard> GetLastPawnMoveAsync(Guid gameId, Guid userId);
        public Task<List<GameBoard>> GetWallsAsync(Guid gameId);
        public Task<bool> IsWallExistsOnPositionAsync(Guid gameId, Position position);
        public Task<bool> IsEnemyExistsOnPositionAsync(Guid gameId, Guid playerId, Position position);
    }
}
