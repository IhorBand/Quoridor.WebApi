using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.Exceptions
{
    public class RoomFullException : Exception
    {
        public RoomFullException()
            : base("Game is full.")
        {
        }

        public RoomFullException(string message)
            : base(message)
        {
        }

        public RoomFullException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
