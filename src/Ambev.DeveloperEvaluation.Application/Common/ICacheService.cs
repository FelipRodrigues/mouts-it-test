using System;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
    }
} 