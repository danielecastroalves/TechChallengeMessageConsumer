using MediatR;

namespace FintechMessageConsumer.Application.Features.Products
{
    public class ProductsEvent : IRequest
    {
        public Guid ProductId { get; set; }
        public decimal ApplicationValue { get; set; }
        public Guid ClientId {  get; set; }
    }
}
