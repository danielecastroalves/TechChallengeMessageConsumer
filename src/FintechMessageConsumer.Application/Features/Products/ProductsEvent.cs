using FintechMessageConsumer.Application.Features.ClientProfile.SetClientProfile;
using FluentValidation;
using MediatR;

namespace FintechMessageConsumer.Application.Features.Products
{
    public class ProductsEvent : IRequest
    {
        public Guid ProductId { get; set; }
        public decimal ApplicationValue { get; set; }
        public Guid ClientId {  get; set; }
    }

    public class ClientProfileRequestValidator : AbstractValidator<ProductsEvent>
    {
        public ClientProfileRequestValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().NotNull();
            RuleFor(x => x.ProductId).NotEmpty().NotNull();
            RuleFor(x => x.ApplicationValue).NotEmpty().NotNull();
        }
    }
}
