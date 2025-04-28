using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAll();
        Task<Sale?> GetById(Guid id);
        Task Add(Sale sale);
        Task Update(Sale sale);
        Task<IEnumerable<Sale>> GetByDateRange(DateTime startDate, DateTime endDate);
        Task<Sale?> GetBySaleNumberAsync(string saleNumber);
        Task DeleteAsync(Guid id);
    }
}
