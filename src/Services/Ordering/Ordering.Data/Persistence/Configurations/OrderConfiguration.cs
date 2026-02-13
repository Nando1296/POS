using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;  
using Ordering.Domain.Entities;
using Ordering.Domain.Enums;

namespace Ordering.Data.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TableNumber)
                .IsRequired();
            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (OrderStatus)Enum.Parse(typeof(OrderStatus), s)
                );

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Order.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
}