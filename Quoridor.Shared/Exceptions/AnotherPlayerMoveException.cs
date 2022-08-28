using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.Exceptions
{
    public class AnotherPlayerMoveException : Exception
    {
        public AnotherPlayerMoveException()
            : base("Not your turn.")
        {
        }

        public AnotherPlayerMoveException(string message)
            : base(message)
        {
        }

        public AnotherPlayerMoveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
