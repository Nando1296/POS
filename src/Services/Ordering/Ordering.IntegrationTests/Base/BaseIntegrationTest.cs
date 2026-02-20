using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ordering.Infrastructure.Persistence;
using Xunit;

namespace Ordering.IntegrationTests.Base;

public abstract class BaseIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient HttpClient;

    protected BaseIntegrationTest(WebApplicationFactory<Program> factory)
    {
        HttpClient = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // 1. Removemos las opciones genéricas
                services.RemoveAll(typeof(DbContextOptions<OrderingDbContext>));
                services.RemoveAll(typeof(OrderingDbContext));

                // 2. LA MAGIA: Creamos un mini-contenedor de dependencias 100% puro y aislado
                // que solo conoce el motor de InMemory Database.
                var internalServiceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // 3. Registramos la base de datos
                string dbName = $"TestDb_{Guid.NewGuid()}";
                services.AddDbContext<OrderingDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                    // Le ponemos el escudo: Forzamos a EF a usar el mini-contenedor, 
                    // ignorando por completo los fantasmas de SQL Server.
                    options.UseInternalServiceProvider(internalServiceProvider); 
                });

                // 4. Inicializamos las tablas
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
                
                db.Database.EnsureCreated();
            });
        }).CreateClient();
    }
}