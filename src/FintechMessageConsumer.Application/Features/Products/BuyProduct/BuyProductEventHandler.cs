using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.Products.BuyProduct;

public class BuyProductEventHandler : IRequestHandler<BuyProductEvent>
{
    private readonly IRepository<ClienteEntity> _clienteRepository;
    private readonly IRepository<PortfolioEntity> _portfolioRepository;
    private readonly IRepository<TransactionEntity> _transactionRepository;
    private readonly ILogger<BuyProductEventHandler> _logger;

    public BuyProductEventHandler
    (
        IRepository<ClienteEntity> repositorio,
        IRepository<PortfolioEntity> portfolioRepository,
        IRepository<TransactionEntity> transactionRepository,
        ILogger<BuyProductEventHandler> logger
    )
    {
        _clienteRepository = repositorio;
        _portfolioRepository = portfolioRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(BuyProductEvent @event, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var clienteEntity = await _clienteRepository
            .GetByFilterAsync(x => x.Id == @event.IdCliente, cancellationToken);

        var portfolioEntity = await _portfolioRepository
            .GetListByFilterAsync(x => x.UsuarioId == @event.IdCliente, cancellationToken);

        if (clienteEntity.Portfolios.Count == 0)
        {
            var portfolio = new PortfolioEntity
            {
                UsuarioId = @event.IdCliente,
                Nome = "Portfolio",
                Descricao = "Meu primeiro Portfolio",
                Ativos =
                [
                    new Ativos
                    {
                        IdProduto = @event.IdProduto
                    }
                ]
            };

            clienteEntity.Portfolios.Add(portfolio.Id);

            await _clienteRepository
                .UpdateAsync(x => x.Id == @event.IdCliente, clienteEntity, CancellationToken.None);

            await SavePortfolio(@event, portfolio, cancellationToken);
        }
        else if (portfolioEntity.Any())
        {
            var portfolio = portfolioEntity
                .FirstOrDefault(x => x.Ativos.Exists(x => x.IdProduto == @event.IdProduto));

            if (portfolio == null)
            {
                portfolio = portfolioEntity.First(x => x.UsuarioId == @event.IdCliente);

                portfolio.Ativos.Add(new Ativos
                {
                    IdProduto = @event.IdProduto
                });

                await SavePortfolio(@event, portfolio, cancellationToken);
            }
            else
            {
                await SavePortfolio(@event, portfolio, cancellationToken);
            }
        }

        _logger.LogInformation(
            "[BuyProduct] " +
            "[Client wallet has been updated successfully] " +
            "[ClientId: {CliendId}] " +
            "[ProductId: {ProductId}]",
            @event.IdCliente,
            @event.IdProduto);

        return Unit.Value;
    }

    private async Task SavePortfolio
    (
        BuyProductEvent @event,
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
        BuyProductEvent @event,
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
