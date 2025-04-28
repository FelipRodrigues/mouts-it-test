using System;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.Services
{
    public class SnsSqsEventPublisher : IEventPublisher
    {
        public Task PublishSaleCreatedAsync(SaleCreatedEvent saleCreatedEvent)
        {
            Console.WriteLine($"[MOCK SNS/SQS] SaleCreatedEvent published: SaleId={saleCreatedEvent.SaleId}, CreatedAt={saleCreatedEvent.CreatedAt}");
            return Task.CompletedTask;
        }

        public Task PublishSaleModifiedAsync(SaleModifiedEvent saleModifiedEvent)
        {
            Console.WriteLine($"[MOCK SNS/SQS] SaleModifiedEvent published: SaleId={saleModifiedEvent.SaleId}, ModifiedAt={saleModifiedEvent.ModifiedAt}");
            return Task.CompletedTask;
        }

        public Task PublishSaleCancelledAsync(SaleCancelledEvent saleCancelledEvent)
        {
            Console.WriteLine($"[MOCK SNS/SQS] SaleCancelledEvent published: SaleId={saleCancelledEvent.SaleId}, CancelledAt={saleCancelledEvent.CancelledAt}");
            return Task.CompletedTask;
        }

        public Task PublishItemCancelledAsync(ItemCancelledEvent itemCancelledEvent)
        {
            Console.WriteLine($"[MOCK SNS/SQS] ItemCancelledEvent published: SaleId={itemCancelledEvent.SaleId}, ItemId={itemCancelledEvent.ItemId}, CancelledAt={itemCancelledEvent.CancelledAt}");
            return Task.CompletedTask;
        }
    }
} 