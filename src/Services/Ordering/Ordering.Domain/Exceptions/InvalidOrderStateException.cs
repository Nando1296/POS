using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Domain.Exceptions
{
    public class InvalidOrderStateException : DomainException
    {
        public InvalidOrderStateException(string message) : base (message)
        {
            
        }
    }
}