namespace Ordering.Domain.Exceptions
{
    public class OrderNotFoundException : DomainException
    {
        public OrderNotFoundException(Guid id) 
            : base($"The order with ID {id}  was not found.")
        {
            
        }
    }
}