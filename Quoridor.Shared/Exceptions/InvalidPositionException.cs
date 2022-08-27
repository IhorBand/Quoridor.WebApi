using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.Exceptions
{
    public class InvalidPositionException : Exception
    {
        public InvalidPositionException()
            : base("Invalid Position. Please, try to move somewhere else. :)")
        {
        }

        public InvalidPositionException(string message)
            : base(message)
        {
        }

        public InvalidPositionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
