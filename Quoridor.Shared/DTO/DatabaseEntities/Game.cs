using System;
using System.Collections.Generic;
using System.Text;
using Quoridor.Shared.DTO.Enums;

namespace Quoridor.Shared.DTO.DatabaseEntities
{
    public class Game
    {
        public Guid Id { get; set; }
        public Guid UserCreatorId { get; set; }
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
        public GameStatus Status { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
