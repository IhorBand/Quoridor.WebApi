using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.DatabaseEntities
{
    public class GameUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GameId { get; set; }
        public DateTime CreatedDateUTC { get; set; }
    }
}
