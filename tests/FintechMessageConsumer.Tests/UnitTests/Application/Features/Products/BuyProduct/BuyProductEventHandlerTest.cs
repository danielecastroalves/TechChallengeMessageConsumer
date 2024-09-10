using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Application.Features.Products.BuyProduct;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace FintechMessageConsumer.Tests.UnitTests.Application.Features.Products.BuyProduct;
public sealed class BuyProductEventHandlerTest
{
    private readonly Mock<IRepository<ClienteEntity>> _mockClienteRepository;
    private readonly Mock<IRepository<PortfolioEntity>> _mockPortfolioRepository;
    private readonly Mock<IRepository<TransactionEntity>> _mockTransactionRepository;
    private readonly Mock<ILogger<BuyProductEventHandler>> _mockLogger;
    private readonly BuyProductEventHandler _handler;
    private readonly Faker _faker;

    public BuyProductEventHandlerTest()
    {
        _mockClienteRepository = new Mock<IRepository<ClienteEntity>>();
        _mockPortfolioRepository = new Mock<IRepository<PortfolioEntity>>();
        _mockTransactionRepository = new Mock<IRepository<TransactionEntity>>();
        _mockLogger = new Mock<ILogger<BuyProductEventHandler>>();
        _handler = new BuyProductEventHandler(
            _mockClienteRepository.Object,
            _mockPortfolioRepository.Object,
            _mockTransactionRepository.Object,
            _mockLogger.Object
        );
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_WithNoExistingPortfolio_ShouldCreateNewPortfolioAndSaveTransaction()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 1000);

        var cliente = new ClienteEntity { Portfolios = [] };

        _mockClienteRepository.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<ClienteEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _mockPortfolioRepository.Setup(r => r.GetListByFilterAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortfolioEntity>());

        var @event = new BuyProductEvent
        {
            IdCliente = clientId,
            IdProduto = productId,
            Quantidade = quantity,
            Preco = price
        };

        // Act
        var result = await _handler.Handle(@event, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _mockClienteRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Expression<Func<ClienteEntity, bool>>>(),
            It.Is<ClienteEntity>(c => c.Portfolios.Count == 1),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _mockPortfolioRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Expression<Func<PortfolioEntity, bool>>>(),
            It.Is<PortfolioEntity>(p =>
                p.UsuarioId == clientId &&
                p.Nome == "Portfolio" &&
                p.Descricao == "Meu primeiro Portfolio" &&
                p.Ativos.Count == 1 &&
                p.Ativos.First().IdProduto == productId &&
                p.Ativos.First().QuantidadeCotas == quantity &&
                p.Ativos.First().ValorTotal == price
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _mockTransactionRepository.Verify(r => r.AddAsync(
            It.Is<TransactionEntity>(t =>
                t.AtivoId == productId &&
                t.TipoTransacao == nameof(TransacationType.Compra) &&
                t.Quantidade == quantity &&
                t.Preco == price
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_WithExistingPortfolioButNoMatchingProduct_ShouldAddNewProductToPortfolioAndSaveTransaction()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var existingProductId = _faker.Random.Guid();
        var newProductId = _faker.Random.Guid();
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 1000);

        var cliente = new ClienteEntity { Portfolios = new List<Guid> { Guid.NewGuid() } };
        var existingPortfolio = new PortfolioEntity
        {
            UsuarioId = clientId,
            Nome = "Existing Portfolio",
            Ativos = new List<Ativos>
            {
                new Ativos { IdProduto = existingProductId, QuantidadeCotas = 10, ValorTotal = 100 }
            }
        };

        _mockClienteRepository.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<ClienteEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _mockPortfolioRepository.Setup(r => r.GetListByFilterAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortfolioEntity> { existingPortfolio });

        var @event = new BuyProductEvent
        {
            IdCliente = clientId,
            IdProduto = newProductId,
            Quantidade = quantity,
            Preco = price
        };

        // Act
        var result = await _handler.Handle(@event, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _mockPortfolioRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Expression<Func<PortfolioEntity, bool>>>(),
            It.Is<PortfolioEntity>(p =>
                p.Ativos.Count == 2 &&
                p.Ativos.Any(a => a.IdProduto == newProductId && a.QuantidadeCotas == quantity && a.ValorTotal == price)
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _mockTransactionRepository.Verify(r => r.AddAsync(
            It.Is<TransactionEntity>(t =>
                t.AtivoId == newProductId &&
                t.TipoTransacao == nameof(TransacationType.Compra) &&
                t.Quantidade == quantity &&
                t.Preco == price
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingPortfolioAndMatchingProduct_ShouldUpdateExistingProductAndSaveTransaction()
    {
        // Arrange
        var clientId = _faker.Random.Guid();
        var productId = _faker.Random.Guid();
        var initialQuantity = 10;
        var initialValue = 100m;
        var quantity = _faker.Random.Int(1, 100);
        var price = _faker.Random.Decimal(1, 1000);

        var cliente = new ClienteEntity { Portfolios = [Guid.NewGuid()] };
        var existingPortfolio = new PortfolioEntity
        {
            UsuarioId = clientId,
            Nome = "Existing Portfolio",
            Ativos = new List<Ativos>
            {
                new Ativos { IdProduto = productId, QuantidadeCotas = initialQuantity, ValorTotal = initialValue }
            }
        };

        _mockClienteRepository.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<ClienteEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        _mockPortfolioRepository.Setup(r => r.GetListByFilterAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortfolioEntity> { existingPortfolio });

        var @event = new BuyProductEvent
        {
            IdCliente = clientId,
            IdProduto = productId,
            Quantidade = quantity,
            Preco = price
        };

        // Act
        var result = await _handler.Handle(@event, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _mockPortfolioRepository.Verify(r => r.UpdateAsync(
            It.IsAny<Expression<Func<PortfolioEntity, bool>>>(),
            It.Is<PortfolioEntity>(p =>
                p.Ativos.Count == 1 &&
                p.Ativos.First().IdProduto == productId &&
                p.Ativos.First().QuantidadeCotas == initialQuantity + quantity &&
                p.Ativos.First().ValorTotal == initialValue + price
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _mockTransactionRepository.Verify(r => r.AddAsync(
            It.Is<TransactionEntity>(t =>
                t.AtivoId == productId &&
                t.TipoTransacao == nameof(TransacationType.Compra) &&
                t.Quantidade == quantity &&
                t.Preco == price
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCancellationRequested_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var @event = new BuyProductEvent
        {
            IdCliente = _faker.Random.Guid(),
            IdProduto = _faker.Random.Guid(),
            Quantidade = _faker.Random.Int(1, 100),
            Preco = _faker.Random.Decimal(1, 1000)
        };

        var cancellationToken = new CancellationToken(true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => _handler.Handle(@event, cancellationToken));
    }
}
