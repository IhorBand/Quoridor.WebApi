using System;
using System.Collections.Generic;
using System.Text;
using Quoridor.Shared.DTO.DatabaseEntities;

namespace Quoridor.Shared.DTO.InputModels.Game
{
    [Serializable]
    public class Position
    {
        public Position()
        {
            this.X = 0;
            this.Y = 0;
        }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Position(GameBoard entity)
        {
            this.X = entity.PositionX;
            this.Y = entity.PositionY;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
