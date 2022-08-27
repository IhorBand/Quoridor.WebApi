using System;
using System.Collections.Generic;
using System.Text;
using Quoridor.Shared.DTO.Enums;

namespace Quoridor.Shared.DTO.DatabaseEntities
{
    public class GameBoard
    {
        public Guid Id { get; set; }
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        public GameBoardEntityType GameBoardEntityType { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public int Order { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
