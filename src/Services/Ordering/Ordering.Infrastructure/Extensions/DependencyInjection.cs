namespace Ordering.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using Ordering.Domain.Interfaces;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<OrderingDbContext>(options =>
            options.UseSqlServer(connectionString));

            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
    }
} 