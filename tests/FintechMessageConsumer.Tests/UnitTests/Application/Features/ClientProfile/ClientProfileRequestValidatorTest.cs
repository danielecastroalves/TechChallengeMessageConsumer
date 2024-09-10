using FintechMessageConsumer.Application.Features.ClientProfile.SetClientProfile;
using FintechMessageConsumer.Application.Features.Products.BuyProduct;
using FluentValidation.TestHelper;

namespace FintechMessageConsumer.Tests.UnitTests.Application.Features.Products.BuyProduct;
public sealed class ClientProfileRequestValidatorTest
{
    private readonly ClientProfileEventValidator _validator;

    public ClientProfileRequestValidatorTest()
    {
        _validator = new ClientProfileEventValidator();
    }

    [Fact]
    public void ShouldHaveErrorWhenClientIdIsEmpty()
    {
        // Arrange
        var model = new SetClientProfileEvent { ClientId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ClientId);
    }

    [Fact]
    public void ShouldHaveErrorWhenQuestionsIsEmpty()
    {
        // Arrange
        var model = new SetClientProfileEvent { Questions =  new List<Question>() };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Questions);
    }


    [Fact]
    public void ShouldNotHaveErrorWhenAllFieldsAreValid()
    {
        // Arrange
        var model = new SetClientProfileEvent
        {
            
            ClientId = Guid.NewGuid(),
            Questions =
            [
                new()
                {
                    QuestionId = Guid.NewGuid(),
                    QuestionValue = 1
                }
            ]
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
