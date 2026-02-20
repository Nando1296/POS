namespace Ordering.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities; 
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _context;

    public OrderRepository(OrderingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Options)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Options)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.Status.ToString() == status);
        }

        return await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
