using Ordering.Domain.Entities;

namespace Ordering.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task<IReadOnlyList<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? status = null, CancellationToken cancellationToken = default);
    }
}