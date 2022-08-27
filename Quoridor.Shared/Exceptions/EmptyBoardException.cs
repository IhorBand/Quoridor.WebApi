using System;
using System.Collections.Generic;
using System.Text;

namespace Quoridor.Shared.Exceptions
{
    public class EmptyBoardException : Exception
    {
        public EmptyBoardException()
            : base("Board is Empty. Try to create new Game.")
        {
        }

        public EmptyBoardException(string message)
            : base(message)
        {
        }

        public EmptyBoardException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
