using MediatR;

namespace FintechMessageConsumer.Application.Features.ClientProfile.SetClientProfile
{
    public class SetClientProfileEvent : IRequest
    {
        public Guid ClientId { get; set; }

        public List<Question> Questions { get; set; } = null!;
    }

    public class Question
    {
        public Guid QuestionId { get; set; }
        public int QuestionValue { get; set; }
    }
}
