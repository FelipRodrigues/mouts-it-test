using System;
using System.Collections.Generic;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public List<SaleItem> Items { get; set; } = new();

        public Sale()
        {
            SaleNumber = string.Empty;
            CustomerName = string.Empty;
            BranchName = string.Empty;
            Items = new List<SaleItem>();
        }

        public Sale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName)
        {
            if (string.IsNullOrWhiteSpace(saleNumber))
                throw new ArgumentException("Sale number is required.", nameof(saleNumber));
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            if (string.IsNullOrWhiteSpace(branchName))
                throw new ArgumentException("Branch name is required.", nameof(branchName));

            SaleNumber = saleNumber;
            SaleDate = saleDate;
            CustomerId = customerId;
            CustomerName = customerName;
            BranchId = branchId;
            BranchName = branchName;
            Items = new List<SaleItem>();
        }
    }
} 