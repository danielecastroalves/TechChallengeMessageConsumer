using FluentValidation;
using MediatR;

namespace FintechMessageConsumer.Application.Features.Products.SellProduct
{
    public class SellProductEvent : IRequest
    {
        public Guid IdProduto { get; set; }
        public Guid IdCliente { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }

    public class SellProductEventValidator : AbstractValidator<SellProductEvent>
    {
        public SellProductEventValidator()
        {
            RuleFor(x => x.IdProduto).NotEmpty().NotNull();
            RuleFor(x => x.IdCliente).NotEmpty().NotNull();
            RuleFor(x => x.Quantidade).NotEmpty().NotNull();
            RuleFor(x => x.Preco).NotEmpty().NotNull();
        }
    }
}
