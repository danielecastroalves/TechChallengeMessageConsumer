using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.Products.SellProduct
{
    public class SellProductEventHandler : IRequestHandler<SellProductEvent>
    {
        private readonly IRepository<ClienteEntity> _clienteRepository;
        private readonly IRepository<PortfolioEntity> _portfolioRepository;
        private readonly IRepository<TransactionEntity> _transactionRepository;
        private readonly ILogger<SellProductEventHandler> _logger;

        public SellProductEventHandler
        (
            IRepository<ClienteEntity> repositorio,
            IRepository<PortfolioEntity> portfolioRepository,
            IRepository<TransactionEntity> transactionRepository,
            ILogger<SellProductEventHandler> logger
        )
        {
            _clienteRepository = repositorio;
            _portfolioRepository = portfolioRepository;
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(SellProductEvent @event, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var clienteEntity = await _clienteRepository
                .GetByFilterAsync(x => x.Id == @event.IdCliente, cancellationToken);

            var portfolioEntity = await _portfolioRepository
                .GetListByFilterAsync(x => x.UsuarioId == @event.IdCliente, cancellationToken);

            if (clienteEntity.Portfolios.Count == 0)
            {
                _logger.LogError("[SellProduct] " +
                    "[Portfolio not Found] " +
                    "[ClientId: {CliendId}] ",
                    @event.IdCliente);

                return Unit.Value;
            }

            var portfolio = portfolioEntity
                .First(x => x.Ativos.Exists(x => x.IdProduto == @event.IdProduto));

            await SavePortfolio(@event, portfolio, cancellationToken);

            _logger.LogInformation(
                "[SellProduct] " +
                "[Client wallet has been updated successfully] " +
                "[ClientId: {CliendId}] " +
                "[ProductId: {ProductId}]",
                @event.IdCliente,
                @event.IdProduto);

            return Unit.Value;
        }

        private async Task SavePortfolio
        (
            SellProductEvent @event,
            PortfolioEntity portfolioEntity,
            CancellationToken cancellationToken
        )
        {
            portfolioEntity.Ativos.First(x => x.IdProduto == @event.IdProduto).QuantidadeCotas += @event.Quantidade;
            portfolioEntity.Ativos.First(x => x.IdProduto == @event.IdProduto).ValorTotal += @event.Preco;

            await _portfolioRepository
                .UpdateAsync(x => x.Id == portfolioEntity.Id, portfolioEntity, cancellationToken);

            await SaveTransaction(@event, portfolioEntity, cancellationToken);
        }

        private async Task SaveTransaction
        (
            SellProductEvent @event,
            PortfolioEntity portfolioEntity,
            CancellationToken cancellationToken
        )
        {
            var transaction = new TransactionEntity
            {
                PortfolioId = portfolioEntity.Id,
                AtivoId = @event.IdProduto,
                TipoTransacao = nameof(TransacationType.Compra),
                Quantidade = @event.Quantidade,
                Preco = @event.Preco,
                DataTransacao = DateTime.Now
            };

            await _transactionRepository.AddAsync(transaction, cancellationToken);
        }
    }
}
