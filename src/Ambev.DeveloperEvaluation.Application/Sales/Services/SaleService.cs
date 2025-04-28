using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheService _cacheService;

        public SaleService(ISaleRepository saleRepository, IEventPublisher eventPublisher, ICacheService cacheService)
        {
            _saleRepository = saleRepository;
            _eventPublisher = eventPublisher;
            _cacheService = cacheService;
        }

        public async Task<SaleDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            var cacheKey = $"sale:id:{id}";
            var cached = await _cacheService.GetAsync<SaleDto>(cacheKey);
            if (cached != null) return cached;
            var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
            if (sale == null) return null;
            var dto = MapToDto(sale);
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<IEnumerable<SaleDto>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            var sales = await _saleRepository.GetAllAsync(cancellationToken);
            return sales.Select(MapToDto);
        }

        public async Task<SaleDto?> GetBySaleNumberAsync(
            string saleNumber,
            CancellationToken cancellationToken = default
        )
        {
            var cacheKey = $"sale:number:{saleNumber}";
            var cached = await _cacheService.GetAsync<SaleDto>(cacheKey);
            if (cached != null) return cached;
            var sale = await _saleRepository.GetBySaleNumberAsync(saleNumber, cancellationToken);
            if (sale == null) return null;
            var dto = MapToDto(sale);
            await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
            return dto;
        }

        public async Task<SaleDto> CreateAsync(
            CreateSaleDto createSaleDto,
            CancellationToken cancellationToken = default
        )
        {
            var sale = new Sale(
                createSaleDto.SaleNumber,
                createSaleDto.SaleDate,
                createSaleDto.CustomerId,
                createSaleDto.CustomerName,
                createSaleDto.BranchId,
                createSaleDto.BranchName
            );

            foreach (var itemDto in createSaleDto.Items)
            {
                // Regras de negócio de desconto e quantidade
                if (itemDto.Quantity > 20)
                    throw new InvalidOperationException(
                        $"Cannot purchase more than 20 identical items for product {itemDto.ProductName}."
                    );

                decimal discount = 0;
                if (itemDto.Quantity >= 10 && itemDto.Quantity <= 20)
                {
                    discount = itemDto.UnitPrice * itemDto.Quantity * 0.20m;
                }
                else if (itemDto.Quantity >= 4 && itemDto.Quantity < 10)
                {
                    discount = itemDto.UnitPrice * itemDto.Quantity * 0.10m;
                }
                // Menos de 4 itens: sem desconto

                var item = new SaleItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Discount = discount,
                };
                item.CalculateTotalAmount();
                sale.Items.Add(item);
            }

            sale.TotalAmount = sale.Items.Sum(item => item.TotalAmount);
            var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

            // Invalida cache relacionado
            await _cacheService.RemoveAsync($"sale:number:{createdSale.SaleNumber}");
            await _cacheService.RemoveAsync($"sale:id:{createdSale.Id}");

            // Publica evento SaleCreated
            await _eventPublisher.PublishSaleCreatedAsync(
                new SaleCreatedEvent { SaleId = createdSale.Id }
            );

            return MapToDto(createdSale);
        }

        public async Task<SaleDto?> UpdateAsync(
            UpdateSaleDto updateSaleDto,
            CancellationToken cancellationToken = default
        )
        {
            var existingSale = await _saleRepository.GetByIdAsync(
                updateSaleDto.Id,
                cancellationToken
            );
            if (existingSale == null)
                return null;

            bool wasCancelled = existingSale.IsCancelled;
            existingSale.IsCancelled = updateSaleDto.IsCancelled;

            // Detecta itens removidos para publicar ItemCancelledEvent
            var removedItems = existingSale
                .Items.Where(ei => updateSaleDto.Items.All(ui => ui.ProductId != ei.ProductId))
                .ToList();

            if (updateSaleDto.Items.Any())
            {
                existingSale.Items.Clear();
                foreach (var itemDto in updateSaleDto.Items)
                {
                    // Regras de negócio de desconto e quantidade
                    if (itemDto.Quantity > 20)
                        throw new InvalidOperationException(
                            $"Cannot purchase more than 20 identical items for product {itemDto.ProductName}."
                        );

                    decimal discount = 0;
                    if (itemDto.Quantity >= 10 && itemDto.Quantity <= 20)
                    {
                        discount = itemDto.UnitPrice * itemDto.Quantity * 0.20m;
                    }
                    else if (itemDto.Quantity >= 4 && itemDto.Quantity < 10)
                    {
                        discount = itemDto.UnitPrice * itemDto.Quantity * 0.10m;
                    }
                    // Menos de 4 itens: sem desconto

                    var item = new SaleItem
                    {
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        Discount = discount,
                    };
                    item.CalculateTotalAmount();
                    existingSale.Items.Add(item);
                }
                existingSale.TotalAmount = existingSale.Items.Sum(item => item.TotalAmount);
            }

            var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

            // Invalida cache relacionado
            await _cacheService.RemoveAsync($"sale:number:{updatedSale.SaleNumber}");
            await _cacheService.RemoveAsync($"sale:id:{updatedSale.Id}");

            // Publica eventos de itens cancelados
            foreach (var removedItem in removedItems)
            {
                await _eventPublisher.PublishItemCancelledAsync(
                    new ItemCancelledEvent { SaleId = updatedSale.Id, ItemId = removedItem.Id }
                );
            }

            // Publica evento de cancelamento de venda
            if (!wasCancelled && updateSaleDto.IsCancelled)
            {
                await _eventPublisher.PublishSaleCancelledAsync(
                    new SaleCancelledEvent { SaleId = updatedSale.Id }
                );
            }
            else
            {
                // Publica evento de modificação de venda
                await _eventPublisher.PublishSaleModifiedAsync(
                    new SaleModifiedEvent { SaleId = updatedSale.Id }
                );
            }

            return MapToDto(updatedSale);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
            var result = await _saleRepository.DeleteAsync(id, cancellationToken);
            if (sale != null)
            {
                await _cacheService.RemoveAsync($"sale:number:{sale.SaleNumber}");
                await _cacheService.RemoveAsync($"sale:id:{id}");
            }
            return result;
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                Date = sale.SaleDate,
                CustomerName = sale.CustomerName,
                Items = sale
                    .Items.Select(item => new SaleItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        TotalAmount = item.TotalAmount,
                    })
                    .ToList(),
            };
        }
    }
}
