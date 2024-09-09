using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.Products.BuyProduct
{
    public class BuyProductEventHandler : IRequestHandler<BuyProductEvent>
    {
        private readonly IRepository<ClienteEntity> _clienteRepository;
        private readonly IRepository<TransactionEntity> _transactionRepository;
        private readonly ILogger<BuyProductEventHandler> _logger;

        public BuyProductEventHandler
        (
            IRepository<ClienteEntity> repositorio,
            ILogger<BuyProductEventHandler> logger,
            IRepository<TransactionEntity> transactionRepository
        )
        {
            _clienteRepository = repositorio;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<Unit> Handle(BuyProductEvent request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await _clienteRepository.GetByFilterAsync(x => x.Id == request.IdCliente);

            await SaveTransaction(request, entity, cancellationToken);

            var wallet = new List<Wallet>() { new Wallet { ProductId = request.IdProduto } };

            entity.Portfolio.Carteira = entity.Portfolio == null ? wallet : AddToList(request.IdProduto, entity);

            await _clienteRepository.UpdateAsync(x => x.Id == request.IdCliente, entity, CancellationToken.None);

            _logger.LogInformation(
                "[ProductsEvent] " +
                "[Client wallet has been updated successfully] " +
                "[ClientId: {CliendId}] " +
                "[ProductId: {ProductId}]",
                request.IdCliente,
                request.IdProduto);

            return Unit.Value;
        }

        private async Task SaveTransaction(BuyProductEvent request, ClienteEntity entity, CancellationToken cancellationToken)
        {
            var transaction = new TransacaoEntity
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
