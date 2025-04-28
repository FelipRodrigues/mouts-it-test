using System;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        private decimal _totalAmount;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount
        {
            get => _totalAmount;
            set => _totalAmount = value;
        }
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public SaleItem()
        {
            ProductName = string.Empty;
        }

        public void CalculateTotalAmount()
        {
            TotalAmount = (Quantity * UnitPrice) - Discount;
        }
    }
}
