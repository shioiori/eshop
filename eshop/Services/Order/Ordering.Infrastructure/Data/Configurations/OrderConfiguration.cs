using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Enums;
using Ordering.Domain.Modals;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Id).HasConversion(orderId => orderId.Value, dbId => OrderId.Of(dbId));
            builder.Property(x => x.OrderName).HasConversion(orderName => orderName.Value, dbName => OrderName.Of(dbName));

            builder.HasOne<Customer>()
                   .WithMany()
                   .HasForeignKey(x => x.CustomerId)
                   .IsRequired();
            
            builder.HasMany<OrderItem>()
                   .WithOne()
                   .HasForeignKey(x => x.OrderId);
            
            builder.ComplexProperty(x => x.ShippingAddress, addressBuilder =>             
            {
                addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
                addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                addressBuilder.Property(a => a.State).HasMaxLength(50);
                addressBuilder.Property(a => a.Country).HasMaxLength(50);
            });
            
            builder.ComplexProperty(x => x.BillingAddress, addressBuilder =>
            {
                addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
                addressBuilder.Property(a => a.EmailAddress).HasMaxLength(50);
                addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();
                addressBuilder.Property(a => a.State).HasMaxLength(50);
                addressBuilder.Property(a => a.Country).HasMaxLength(50);
            });
            
            builder.ComplexProperty(x => x.Payment, paymentBuilder =>
            {
                paymentBuilder.Property(a => a.CardName).HasMaxLength(50);
                paymentBuilder.Property(a => a.CardNumber).HasMaxLength(50).IsRequired();
                paymentBuilder.Property(a => a.CVV);
                paymentBuilder.Property(a => a.PaymentMethod).HasMaxLength(180);
                paymentBuilder.Property(a => a.Expiration);
            });
            
            builder.Property(x => x.Status).HasDefaultValue(OrderStatus.Draft)
                .HasConversion(x => x.ToString(), dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus));
            
            builder.Property(x => x.TotalPrice);
        }
    }
}
