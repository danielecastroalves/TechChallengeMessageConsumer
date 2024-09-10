using MediatR;
using FluentValidation;

namespace FintechMessageConsumer.Application.Features.ClientProfile.SetClientProfile
{
    public class SetClientProfileEvent : IRequest<Unit> 
    {
        public Guid ClientId { get; set; }

        public List<Question> Questions { get; set; } = null!;
    }

    public class Question
    {
        public Guid QuestionId { get; set; }
        public int QuestionValue { get; set; }
    }

    public class ClientProfileEventValidator : AbstractValidator<SetClientProfileEvent>
    {
        public ClientProfileEventValidator()
        {
            RuleFor(x => x.ClientId).NotEmpty().NotNull();
            RuleFor(x => x.Questions).NotEmpty().NotNull();
        }
    }
}
