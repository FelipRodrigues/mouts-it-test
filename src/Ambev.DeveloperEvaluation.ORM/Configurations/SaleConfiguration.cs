using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SaleNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.SaleDate)
                .IsRequired();

            builder.Property(s => s.CustomerId)
                .IsRequired();

            builder.Property(s => s.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.BranchId)
                .IsRequired();

            builder.Property(s => s.BranchName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(s => s.IsCancelled)
                .IsRequired();

            builder.HasMany(s => s.Items)
                .WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");

            builder.HasKey(si => si.Id);

            builder.Property(si => si.ProductId)
                .IsRequired();

            builder.Property(si => si.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(si => si.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(si => si.Quantity)
                .IsRequired();

            builder.Property(si => si.Discount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(si => si.TotalAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(si => si.SaleId)
                .IsRequired();
        }
    }
} 