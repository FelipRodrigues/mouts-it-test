using System;
using System.Collections.Generic;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public required string SaleNumber { get; set; }
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public required string CustomerName { get; set; }
    public Guid BranchId { get; set; }
    public required string BranchName { get; set; }
    public required List<CreateSaleItemRequest> Items { get; set; } = new();
}

public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
}

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber).NotEmpty().MaximumLength(50);

        RuleFor(x => x.SaleDate).NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);

        RuleFor(x => x.CustomerId).NotEmpty();

        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.BranchId).NotEmpty();

        RuleFor(x => x.BranchName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Items)
            .NotEmpty()
            .Must(items => items.Count > 0)
            .WithMessage("At least one item is required");

        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemRequestValidator());
    }
}

public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    public CreateSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(100);

        RuleFor(x => x.UnitPrice).GreaterThan(0);

        RuleFor(x => x.Quantity).GreaterThan(0);

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.UnitPrice * x.Quantity)
            .WithMessage("Discount cannot be greater than the total amount");
    }
}
