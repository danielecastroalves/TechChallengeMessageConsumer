using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.Products.SellProduct
{
    public class SellProductEventHandler : IRequestHandler<SellProductEvent>
    {
        private readonly IRepository<ClienteEntity> _repositorio;
        private readonly IRepository<TransactionEntity> _transactionRepository;
        private readonly ILogger<SellProductEventHandler> _logger;

        public SellProductEventHandler
        (
            IRepository<ClienteEntity> repositorio,
            ILogger<SellProductEventHandler> logger,
            IRepository<TransactionEntity> transactionRepository
        )
        {
            _repositorio = repositorio;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<Unit> Handle(SellProductEvent request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await _repositorio.GetByFilterAsync(x => x.Id == request.IdCliente);

            await SaveTransaction(request, entity, cancellationToken);

            var wallet = new List<Wallet>() { new Wallet { ProductId = request.IdProduto } };

            entity.Portfolio.Carteira = entity.Portfolio == null ? wallet : AddToList(request.IdProduto, entity);

            await _repositorio.UpdateAsync(x => x.Id == request.IdCliente, entity, CancellationToken.None);

            _logger.LogInformation(
                "[ProductsEvent] " +
                "[Client wallet has been updated successfully] " +
                "[ClientId: {CliendId}] " +
                "[ProductId: {ProductId}]",
                request.IdCliente,
                request.IdProduto);

            return Unit.Value;
        }

        private async Task SaveTransaction(SellProductEvent request, ClienteEntity entity, CancellationToken cancellationToken)
        {
            var transaction = new TransactionEntity
            {
                AtivoId = request.ProductId,
                PortfolioId = entity.Portfolio.Id,
                Preco = request.Price,
                Quantidade = request.Amount,
                TipoTransacao = request.TransactionType
            };

            await _transactionRepository.AddAsync(transaction, cancellationToken);
        }

        private List<Wallet> AddToList(Guid productId, ClienteEntity entity)
        {
            entity.Portfolio.Carteira.Add(new Wallet { ProductId = productId });
            return entity.Portfolio.Carteira;
        }
    }
}
