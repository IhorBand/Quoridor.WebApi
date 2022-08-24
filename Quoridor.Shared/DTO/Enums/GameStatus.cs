using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.DTO.Enums
{
    public enum GameStatus : int
    {
        WaitingForPlayers = 0,
        InProgress = 1,
        Finished = 2
    }
}
