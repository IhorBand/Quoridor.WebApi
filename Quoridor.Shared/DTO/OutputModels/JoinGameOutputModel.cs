using System;
using System.Collections.Generic;
using System.Text;
using Quoridor.Shared.DTO.InputModels.Game;

namespace Quoridor.Shared.DTO.OutputModels
{
    public class JoinGameOutputModel
    {
        public Guid UserGameId { get; set; }
        public Position PlayerPosition { get; set; }
        public Position EnemyPosition { get; set; }
        public bool IsFull { get; set; }
        public bool IsAbleToMove { get; set; }
        public DTO.Enums.Direction DirectionToWin { get; set; }
    }
}
