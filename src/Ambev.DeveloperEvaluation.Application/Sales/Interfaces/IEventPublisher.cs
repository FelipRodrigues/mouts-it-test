using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishSaleCreatedAsync(SaleCreatedEvent saleCreatedEvent);
        Task PublishSaleModifiedAsync(SaleModifiedEvent saleModifiedEvent);
        Task PublishSaleCancelledAsync(SaleCancelledEvent saleCancelledEvent);
        Task PublishItemCancelledAsync(ItemCancelledEvent itemCancelledEvent);
    }
} 