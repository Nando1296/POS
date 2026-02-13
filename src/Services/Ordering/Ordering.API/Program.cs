using Ordering.Application;
using Ordering.Data;
using Ordering.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor
builder.Services.AddControllers();

// Swagger / OpenAPI (Para probar la API visualmente)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capas de Arquitectura Limpia
builder.Services.AddApplicationServices(); // Registra MediatR
builder.Services.AddInfrastructureServices(builder.Configuration); // Registra EF Core y Repositorios

var app = builder.Build();

// 2. Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();