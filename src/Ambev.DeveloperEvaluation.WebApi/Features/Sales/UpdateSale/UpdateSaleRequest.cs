using System;
using System.Collections.Generic;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    public Guid Id { get; set; }
    public bool IsCancelled { get; set; }
    public List<UpdateSaleItemRequest> Items { get; set; } = new();
}

public class UpdateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
}

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        When(x => x.Items.Count > 0, () =>
        {
            RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemRequestValidator());
        });
    }
}

public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.UnitPrice * x.Quantity)
            .WithMessage("Discount cannot be greater than the total amount");
    }
} 