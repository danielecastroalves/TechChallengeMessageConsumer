using FintechMessageConsumer.Domain.Entities;
using FluentValidation;
using MediatR;

namespace FintechMessageConsumer.Application.Features.Products
{
    public class ProductsEvent : IRequest
    {
        public Guid ProductId { get; set; }
        public Guid ClientId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public TransactionType TransactionType { get; set; }
    }

    public class ClientProfileRequestValidator : AbstractValidator<ProductsEvent>
    {
        public ClientProfileRequestValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().NotNull();
            RuleFor(x => x.ClientId).NotEmpty().NotNull();
            RuleFor(x => x.Price).NotEmpty().NotNull();
            RuleFor(x => x.Amount).NotEmpty().NotNull();
            RuleFor(x => x.TransactionType).NotEmpty().NotNull();
        }
    }
}
