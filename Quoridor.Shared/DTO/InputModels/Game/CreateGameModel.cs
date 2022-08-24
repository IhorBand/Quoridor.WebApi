using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.InputModels.Game
{
    public class CreateGameModel
    {
        public string Name { get; set; }
        public int MaxPlayers { get; set; }
    }
}
