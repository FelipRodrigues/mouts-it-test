using System;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSalesByDateRange;

public class GetSalesByDateRangeRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetSalesByDateRangeRequestValidator : AbstractValidator<GetSalesByDateRangeRequest>
{
    public GetSalesByDateRangeRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("Start date must be less than or equal to end date");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("End date cannot be in the future");
    }
} 