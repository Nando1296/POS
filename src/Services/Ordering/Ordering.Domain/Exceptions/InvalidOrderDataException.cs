
namespace Ordering.Domain.Exceptions
{
    public class InvalidOrderDataException : DomainException
    {
        public InvalidOrderDataException(string message) : base(message)
        {
        }
    }
}