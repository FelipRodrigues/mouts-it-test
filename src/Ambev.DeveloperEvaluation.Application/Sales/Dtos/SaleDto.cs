using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Application.Sales.Dtos
{
    public class SaleDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public List<SaleItemDto> Items { get; set; } = new();
    }

    public class SaleItemDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CreateSaleDto
    {
        public CreateSaleDto()
        {
            Items = new List<CreateSaleItemDto>();
        }

        public required string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }
        public Guid BranchId { get; set; }
        public required string BranchName { get; set; }
        public List<CreateSaleItemDto> Items { get; set; }
    }

    public class CreateSaleItemDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
    }

    public class UpdateSaleDto
    {
        public UpdateSaleDto()
        {
            Items = new List<CreateSaleItemDto>();
        }

        public Guid Id { get; set; }
        public bool IsCancelled { get; set; }
        public List<CreateSaleItemDto> Items { get; set; }
    }
}
