using System;

namespace Ambev.DeveloperEvaluation.Domain.Exceptions
{
    public class SaleNotFoundException : Exception
    {
        public SaleNotFoundException(Guid id)
            : base($"Sale with ID {id} was not found.")
        {
        }
    }
} 