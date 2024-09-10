using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Application.Features.Products.SellProduct;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using Moq;

namespace FintechMessageConsumer.Tests.UnitTests.Application.Features.Products.SellProduct
{
    public sealed class SellProductEventHandlerTest
    {
        private readonly Mock<IRepository<ClienteEntity>> _mockClienteRepository;
        private readonly Mock<IRepository<PortfolioEntity>> _mockPortfolioRepository;
        private readonly Mock<IRepository<TransactionEntity>> _mockTransactionRepository;
        private readonly Mock<ILogger<SellProductEventHandler>> _mockLogger;
        private readonly SellProductEventHandler _handler;
        private readonly Faker _faker;

        public SellProductEventHandlerTest()
        {
            _mockClienteRepository = new Mock<IRepository<ClienteEntity>>();
            _mockPortfolioRepository = new Mock<IRepository<PortfolioEntity>>();
            _mockTransactionRepository = new Mock<IRepository<TransactionEntity>>();
            _mockLogger = new Mock<ILogger<SellProductEventHandler>>();
            _handler = new SellProductEventHandler(
                _mockClienteRepository.Object,
                _mockPortfolioRepository.Object,
                _mockTransactionRepository.Object,
                _mockLogger.Object
            );
            _faker = new Faker();
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldUpdatePortfolioAndSaveTransaction()
        {
            // Arrange
            var productId = _faker.Random.Guid();
            var quantity = _faker.Random.Int(1, 100);
            var price = _faker.Random.Decimal(1, 1000);

            var cliente = new ClienteEntity();
            var portfolio = new PortfolioEntity
            {
                UsuarioId = cliente.Id,
                Ativos =
                [
                    new Ativos { IdProduto = productId, QuantidadeCotas = 10, ValorTotal = 100 }
                ]
            };
            cliente.Portfolios.Add(portfolio.Id);

            _mockClienteRepository.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<ClienteEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cliente);

            _mockPortfolioRepository.Setup(r => r.GetListByFilterAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([portfolio]);

            var @event = new SellProductEvent
            {
                IdCliente = cliente.Id,
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
                    p.Ativos[0].QuantidadeCotas == 10 + quantity &&
                    p.Ativos[0].ValorTotal == 100 + price
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _mockTransactionRepository.Verify(r => r.AddAsync(
                It.Is<TransactionEntity>(t =>
                    t.PortfolioId == portfolio.Id &&
                    t.AtivoId == productId &&
                    t.TipoTransacao == TransacationType.Compra.GetDisplayName() &&
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
        public async Task Handle_WithEmptyPortfolio_ShouldLogErrorAndReturnUnitValue()
        {
            // Arrange
            var clientId = _faker.Random.Guid();
            var productId = _faker.Random.Guid();

            var cliente = new ClienteEntity();

            _mockClienteRepository.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<ClienteEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cliente);

            _mockPortfolioRepository.Setup(r => r.GetListByFilterAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PortfolioEntity>());

            var @event = new SellProductEvent
            {
                IdCliente = clientId,
                IdProduto = productId,
                Quantidade = _faker.Random.Int(1, 100),
                Preco = _faker.Random.Decimal(1, 1000)
            };

            // Act
            var result = await _handler.Handle(@event, CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)
                ),
                Times.Once
            );

            _mockPortfolioRepository.Verify(r => r.UpdateAsync(It.IsAny<Expression<Func<PortfolioEntity, bool>>>(), It.IsAny<PortfolioEntity>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockTransactionRepository.Verify(r => r.AddAsync(It.IsAny<TransactionEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCancellationRequested_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var @event = new SellProductEvent
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
}
