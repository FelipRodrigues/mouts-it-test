using System;

namespace Ambev.DeveloperEvaluation.Domain.Exceptions
{
    public class InvalidSaleOperationException : Exception
    {
        public InvalidSaleOperationException(string message)
            : base(message)
        {
        }
    }
} 