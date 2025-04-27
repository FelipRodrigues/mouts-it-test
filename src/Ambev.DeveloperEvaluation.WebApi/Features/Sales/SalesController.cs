using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Sales.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesController : BaseController
{
    private readonly ISaleService _saleService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="saleService">The sale service instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public SalesController(ISaleService saleService, IMapper mapper)
    {
        _saleService = saleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleDto request, CancellationToken cancellationToken)
    {
        var response = await _saleService.CreateAsync(request, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<SaleDto>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }

    /// <summary>
    /// Retrieves a sale by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _saleService.GetByIdAsync(id, cancellationToken);
        if (response == null)
            return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

        return Ok(new ApiResponseWithData<SaleDto>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = response
        });
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="request">The sale update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleDto request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new ApiResponse { Success = false, Message = "ID mismatch" });

        var response = await _saleService.UpdateAsync(request, cancellationToken);
        if (response == null)
            return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

        return Ok(new ApiResponseWithData<SaleDto>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = response
        });
    }

    /// <summary>
    /// Deletes a sale by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the sale was deleted</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var success = await _saleService.DeleteAsync(id, cancellationToken);
        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Sale deleted successfully"
        });
    }

    /// <summary>
    /// Retrieves a sale by its number
    /// </summary>
    /// <param name="saleNumber">The sale number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("number/{saleNumber}")]
    [ProducesResponseType(typeof(ApiResponseWithData<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleByNumber([FromRoute] string saleNumber, CancellationToken cancellationToken)
    {
        var response = await _saleService.GetBySaleNumberAsync(saleNumber, cancellationToken);
        if (response == null)
            return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

        return Ok(new ApiResponseWithData<SaleDto>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = response
        });
    }

    /// <summary>
    /// Retrieves all sales
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all sales</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<SaleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSales(CancellationToken cancellationToken)
    {
        var response = await _saleService.GetAllAsync(cancellationToken);

        return Ok(new ApiResponseWithData<IEnumerable<SaleDto>>
        {
            Success = true,
            Message = "Sales retrieved successfully",
            Data = response
        });
    }
} 