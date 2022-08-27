using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Quoridor.Shared.DTO.Enums;
using Quoridor.Shared.DTO.InputModels.Game;

namespace Quoridor.Shared.Abstractions.Services
{
    public interface IGameBoardService
    {
        public Task<bool> MakeMoveAsync(Guid gameId, Guid userId, Direction direction);
        public Task BuildWallAsync(Guid gameId, Guid userId, Position positionStart, Position positionEnd);
    }
}
