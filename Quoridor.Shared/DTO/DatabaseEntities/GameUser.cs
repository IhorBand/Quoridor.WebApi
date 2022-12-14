using System;
using System.Collections.Generic;
using System.Text;
using Quoridor.Shared.DTO.Enums;

namespace Quoridor.Shared.DTO.DatabaseEntities
{
    public class GameUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAbleToMove { get; set; }
        public Direction DirectionToWin { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
