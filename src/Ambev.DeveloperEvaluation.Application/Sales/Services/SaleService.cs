using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using System.Threading;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Application.Sales.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;

        public SaleService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
            return sale != null ? MapToDto(sale) : null;
        }

        public async Task<IEnumerable<SaleDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sales = await _saleRepository.GetAllAsync(cancellationToken);
            return sales.Select(MapToDto);
        }

        public async Task<SaleDto?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetBySaleNumberAsync(saleNumber, cancellationToken);
            return sale != null ? MapToDto(sale) : null;
        }

        public async Task<SaleDto> CreateAsync(CreateSaleDto createSaleDto, CancellationToken cancellationToken = default)
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
                var item = new SaleItem
                {
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Discount = itemDto.Discount
                };
                item.CalculateTotalAmount();
                sale.Items.Add(item);
            }

            sale.TotalAmount = sale.Items.Sum(item => item.TotalAmount);
            var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
            return MapToDto(createdSale);
        }

        public async Task<SaleDto?> UpdateAsync(UpdateSaleDto updateSaleDto, CancellationToken cancellationToken = default)
        {
            var existingSale = await _saleRepository.GetByIdAsync(updateSaleDto.Id, cancellationToken);
            if (existingSale == null)
                return null;

            existingSale.IsCancelled = updateSaleDto.IsCancelled;

            if (updateSaleDto.Items.Any())
            {
                existingSale.Items.Clear();
                foreach (var itemDto in updateSaleDto.Items)
                {
                    var item = new SaleItem
                    {
                        ProductId = itemDto.ProductId,
                        ProductName = itemDto.ProductName,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        Discount = itemDto.Discount
                    };
                    item.CalculateTotalAmount();
                    existingSale.Items.Add(item);
                }
                existingSale.TotalAmount = existingSale.Items.Sum(item => item.TotalAmount);
            }

            var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);
            return MapToDto(updatedSale);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _saleRepository.DeleteAsync(id, cancellationToken);
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                Date = sale.SaleDate,
                CustomerName = sale.CustomerName,
                Items = sale.Items.Select(item => new SaleItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalAmount = item.TotalAmount
                }).ToList()
            };
        }
    }
} 