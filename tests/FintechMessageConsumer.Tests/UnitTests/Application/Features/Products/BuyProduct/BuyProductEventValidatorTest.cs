using FintechMessageConsumer.Application.Features.Products.BuyProduct;
using FluentValidation.TestHelper;

namespace FintechMessageConsumer.Tests.UnitTests.Application.Features.Products.BuyProduct;
public sealed class BuyProductEventValidatorTest
{
    private readonly BuyProductEventValidator _validator;

    public BuyProductEventValidatorTest()
    {
        _validator = new BuyProductEventValidator();
    }

    [Fact]
    public void ShouldHaveErrorWhenIdProdutoIsEmpty()
    {
        // Arrange
        var model = new BuyProductEvent { IdProduto = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdProduto);
    }

    [Fact]
    public void ShouldHaveErrorWhenIdClienteIsEmpty()
    {
        // Arrange
        var model = new BuyProductEvent { IdCliente = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdCliente);
    }


    [Fact]
    public void ShouldHaveErrorWhenQuantidadeIsZero()
    {
        // Arrange
        var model = new BuyProductEvent { Quantidade = 0 };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantidade);
    }

    [Fact]
    public void ShouldHaveErrorWhenPrecoIsZero()
    {
        // Arrange
        var model = new BuyProductEvent { Preco = 0 };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Preco);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenAllFieldsAreValid()
    {
        // Arrange
        var model = new BuyProductEvent
        {
            IdProduto = Guid.NewGuid(),
            IdCliente = Guid.NewGuid(),
            Quantidade = 10,
            Preco = 100.0m
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
