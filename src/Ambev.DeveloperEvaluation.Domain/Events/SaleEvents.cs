using System;

namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleCreatedEvent
    {
        public Guid SaleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class SaleModifiedEvent
    {
        public Guid SaleId { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }

    public class SaleCancelledEvent
    {
        public Guid SaleId { get; set; }
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    }

    public class ItemCancelledEvent
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
        public DateTime CancelledAt { get; set; } = DateTime.UtcNow;
    }
} 