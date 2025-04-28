using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ambev.DeveloperEvaluation.Application.Sales.Dtos;
using Ambev.DeveloperEvaluation.Application.Sales.Interfaces;
using Ambev.DeveloperEvaluation.Application.Sales.Services;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class SaleServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ICacheService _cacheService;
    private readonly SaleService _service;

    public SaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _cacheService = Substitute.For<ICacheService>();
        _service = new SaleService(_saleRepository, _eventPublisher, _cacheService);
    }

    [Fact(DisplayName = "Given valid sale When creating Then returns SaleDto")]
    public async Task CreateAsync_ValidSale_ReturnsSaleDto()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            SaleNumber = "S123",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto",
                    Quantity = 2,
                    UnitPrice = 10,
                    Discount = 0,
                },
            },
        };
        var sale = new Sale(
            createDto.SaleNumber,
            createDto.SaleDate,
            createDto.CustomerId,
            createDto.CustomerName,
            createDto.BranchId,
            createDto.BranchName
        );
        sale.Items.Add(
            new SaleItem
            {
                ProductId = createDto.Items[0].ProductId,
                ProductName = "Produto",
                Quantity = 2,
                UnitPrice = 10,
                Discount = 0,
                TotalAmount = 20,
            }
        );
        sale.TotalAmount = 20;
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be(createDto.CustomerName);
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductName.Should().Be("Produto");
    }

    [Fact(DisplayName = "Given sale id When getting by id Then returns SaleDto")]
    public async Task GetByIdAsync_ExistingId_ReturnsSaleDto()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale(
            "S123",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Cliente",
            Guid.NewGuid(),
            "Filial"
        )
        {
            Id = saleId,
        };
        _cacheService.GetAsync<SaleDto>($"sale:id:{saleId}").Returns((SaleDto?)null); // Simula cache miss
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _service.GetByIdAsync(saleId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        await _cacheService.Received(1).GetAsync<SaleDto>($"sale:id:{saleId}");
        await _cacheService.Received(1).SetAsync($"sale:id:{saleId}", Arg.Any<SaleDto>(), Arg.Any<TimeSpan?>());
    }

    [Fact(DisplayName = "Given sales When getting all Then returns all SaleDto")]
    public async Task GetAllAsync_ReturnsAllSales()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new Sale("S1", DateTime.UtcNow, Guid.NewGuid(), "C1", Guid.NewGuid(), "F1"),
            new Sale("S2", DateTime.UtcNow, Guid.NewGuid(), "C2", Guid.NewGuid(), "F2"),
        };
        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);

        // Act
        var result = (await _service.GetAllAsync(CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].CustomerName.Should().Be("C1");
        result[1].CustomerName.Should().Be("C2");
    }

    [Fact(DisplayName = "Given update dto When updating sale Then returns updated SaleDto")]
    public async Task UpdateAsync_ValidUpdate_ReturnsUpdatedSaleDto()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var sale = new Sale("S1", DateTime.UtcNow, Guid.NewGuid(), "C1", Guid.NewGuid(), "F1")
        {
            Id = saleId,
        };
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        var updateDto = new UpdateSaleDto
        {
            Id = saleId,
            IsCancelled = true,
            Items = new List<CreateSaleItemDto>(),
        };

        // Act
        var result = await _service.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
    }

    [Fact(DisplayName = "Given sale id When deleting sale Then returns true")]
    public async Task DeleteAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        _saleRepository.DeleteAsync(saleId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _service.DeleteAsync(saleId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "Given 4-9 items When creating sale Then applies 10 percent discount")]
    public async Task CreateAsync_QuantityBetween4And9_Applies10PercentDiscount()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            SaleNumber = "S124",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto10",
                    Quantity = 5,
                    UnitPrice = 100,
                    Discount = 0, // Ignorado pela regra
                },
            },
        };
        var expectedDiscount = 5 * 100 * 0.10m;
        var sale = new Sale(
            createDto.SaleNumber,
            createDto.SaleDate,
            createDto.CustomerId,
            createDto.CustomerName,
            createDto.BranchId,
            createDto.BranchName
        );
        sale.Items.Add(
            new SaleItem
            {
                ProductId = createDto.Items[0].ProductId,
                ProductName = "Produto10",
                Quantity = 5,
                UnitPrice = 100,
                Discount = expectedDiscount,
                TotalAmount = 5 * 100 - expectedDiscount,
            }
        );
        sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.First().Discount.Should().Be(expectedDiscount);
    }

    [Fact(DisplayName = "Given 10-20 items When creating sale Then applies 20 percent discount")]
    public async Task CreateAsync_QuantityBetween10And20_Applies20PercentDiscount()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            SaleNumber = "S125",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto20",
                    Quantity = 15,
                    UnitPrice = 50,
                    Discount = 0, // Ignorado pela regra
                },
            },
        };
        var expectedDiscount = 15 * 50 * 0.20m;
        var sale = new Sale(
            createDto.SaleNumber,
            createDto.SaleDate,
            createDto.CustomerId,
            createDto.CustomerName,
            createDto.BranchId,
            createDto.BranchName
        );
        sale.Items.Add(
            new SaleItem
            {
                ProductId = createDto.Items[0].ProductId,
                ProductName = "Produto20",
                Quantity = 15,
                UnitPrice = 50,
                Discount = expectedDiscount,
                TotalAmount = 15 * 50 - expectedDiscount,
            }
        );
        sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.First().Discount.Should().Be(expectedDiscount);
    }

    [Fact(DisplayName = "Given more than 20 items When creating sale Then throws exception")]
    public async Task CreateAsync_QuantityAbove20_ThrowsException()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            SaleNumber = "S126",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "ProdutoExcess",
                    Quantity = 21,
                    UnitPrice = 10,
                    Discount = 0,
                },
            },
        };

        // Act
        Func<Task> act = async () => await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot purchase more than 20 identical items for product ProdutoExcess.");
    }

    [Fact(DisplayName = "Given less than 4 items When creating sale Then does not apply discount")]
    public async Task CreateAsync_QuantityBelow4_NoDiscount()
    {
        // Arrange
        var createDto = new CreateSaleDto
        {
            SaleNumber = "S127",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Cliente",
            BranchId = Guid.NewGuid(),
            BranchName = "Filial",
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "ProdutoNoDiscount",
                    Quantity = 2,
                    UnitPrice = 30,
                    Discount = 100, // Ignorado pela regra
                },
            },
        };
        var sale = new Sale(
            createDto.SaleNumber,
            createDto.SaleDate,
            createDto.CustomerId,
            createDto.CustomerName,
            createDto.BranchId,
            createDto.BranchName
        );
        sale.Items.Add(
            new SaleItem
            {
                ProductId = createDto.Items[0].ProductId,
                ProductName = "ProdutoNoDiscount",
                Quantity = 2,
                UnitPrice = 30,
                Discount = 0,
                TotalAmount = 2 * 30,
            }
        );
        sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var result = await _service.CreateAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.First().Discount.Should().Be(0);
    }
}
