using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.Products
{
    public class ProductsEventHandler : IRequestHandler<ProductsEvent>
    {
        private readonly IRepository<ClienteEntity> _repositorio;
        private readonly ILogger<ProductsEventHandler> _logger;

        public ProductsEventHandler
        (
            IRepository<ClienteEntity> repositorio,
            ILogger<ProductsEventHandler> logger
        )
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public async Task<Unit> Handle(ProductsEvent request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var entity = await _repositorio.GetByFilterAsync(x => x.Id == request.ClientId);

            var newListProducts = new List<Product>() { new Product { ProductId = request.ProductId, ValueInvested = request.ApplicationValue } };

            entity.Wallet = entity.Wallet == null ? newListProducts : AddToList(request, entity);

            await _repositorio.UpdateAsync(x => x.Id == request.ClientId, entity, CancellationToken.None);

            _logger.LogInformation(
                "[ProductsEvent] " +
                "[Client wallet has been updated successfully] " +
                "[ClientId: {CliendId}] " +
                "[ProductId: {ProductId}]",
                request.ClientId,
                request.ProductId);

            return Unit.Value;
        }

        private List<Product> AddToList(ProductsEvent request, ClienteEntity entity)
        {
            entity.Wallet.Add(new Product { ProductId = request.ProductId, ValueInvested = request.ApplicationValue });
            return entity.Wallet;
        }
    }
}
