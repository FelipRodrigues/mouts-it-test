using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;

namespace Ambev.DeveloperEvaluation.Application.Sales.Interfaces
{
    public interface ISaleService
    {
        Task<SaleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SaleDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SaleDto?> GetBySaleNumberAsync(
            string saleNumber,
            CancellationToken cancellationToken = default
        );
        Task<SaleDto> CreateAsync(
            CreateSaleDto createSaleDto,
            CancellationToken cancellationToken = default
        );
        Task<SaleDto?> UpdateAsync(
            UpdateSaleDto updateSaleDto,
            CancellationToken cancellationToken = default
        );
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
