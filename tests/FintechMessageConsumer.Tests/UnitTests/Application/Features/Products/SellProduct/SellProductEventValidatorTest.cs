using FintechMessageConsumer.Application.Features.Products.SellProduct;
using FluentValidation.TestHelper;

namespace FintechMessageConsumer.Tests.UnitTests.Application.Features.Products.SellProduct;
public sealed class SellProductEventValidatorTest
{
    private readonly SellProductEventValidator _validator;

    public SellProductEventValidatorTest()
    {
        _validator = new SellProductEventValidator();
    }

    [Fact]
    public void ShouldHaveErrorWhenIdProdutoIsEmpty()
    {
        // Arrange
        var model = new SellProductEvent { IdProduto = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdProduto);
    }

    [Fact]
    public void ShouldHaveErrorWhenIdClienteIsEmpty()
    {
        // Arrange
        var model = new SellProductEvent { IdCliente = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IdCliente);
    }


    [Fact]
    public void ShouldHaveErrorWhenQuantidadeIsZero()
    {
        // Arrange
        var model = new SellProductEvent { Quantidade = 0 };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantidade);
    }

    [Fact]
    public void ShouldHaveErrorWhenPrecoIsZero()
    {
        // Arrange
        var model = new SellProductEvent { Preco = 0 };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Preco);
    }

    [Fact]
    public void ShouldNotHaveErrorWhenAllFieldsAreValid()
    {
        // Arrange
        var model = new SellProductEvent
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
