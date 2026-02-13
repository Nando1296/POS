using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;

namespace Ordering.Data.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Clave primaria
        builder.HasKey(oi => oi.Id); // Aquí definimos que 'oi' es el item

        // Propiedades
        builder.Property(oi => oi.ProductName)
               .HasMaxLength(100)
               .IsRequired();
               
        builder.Property(oi => oi.UnitPrice)
               .HasColumnType("decimal(18,2)");

        // Value Objects (Options)
        builder.OwnsMany(oi => oi.Options, optionBuilder =>
        {
            optionBuilder.ToTable("OrderItemOptions");
            optionBuilder.WithOwner().HasForeignKey("OrderItemId");
            
            optionBuilder.Property(o => o.Name).IsRequired();
            optionBuilder.Property(o => o.AdditionalPrice).HasColumnType("decimal(18,2)");
            
            optionBuilder.HasKey("OrderItemId", "Name");
        });

        // Configuración de acceso al campo privado (Aquí fallaba el Metadata)
        builder.Metadata.FindNavigation(nameof(OrderItem.Options))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}