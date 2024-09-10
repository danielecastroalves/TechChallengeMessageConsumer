using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FintechMessageConsumer.Application.Features.ClientProfile.SetClientProfile
{
    public class SetClientProfileEventHandler : IRequestHandler<SetClientProfileEvent>
    {
        private readonly IRepository<ClienteEntity> _repositorio;
        private readonly ILogger<SetClientProfileEventHandler> _logger;

        public SetClientProfileEventHandler
        (
            IRepository<ClienteEntity> repositorio,
            ILogger<SetClientProfileEventHandler> logger
        )
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        public async Task<Unit> Handle(SetClientProfileEvent request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var totalPoints = 0;

            foreach (var value in request.Questions)
            {
                totalPoints += value.QuestionValue;
            }

            var investorProfile = SetInvestorProfile(totalPoints);

            await UpdateInvestorProfile(investorProfile, request.ClientId);

            _logger.LogInformation(
                "[SetClientProfile] " +
                "[Client Profile has been updated successfully] " +
                "[ClientId: {CliendId}] " +
                "[Profile: {Profile}]",
                request.ClientId,
                investorProfile);

            return Unit.Value;
        }

        private static InvestorProfile SetInvestorProfile(int pontuacaoTotal)
        {
            InvestorProfile profile = InvestorProfile.Indefinido;

            if (pontuacaoTotal <= 20)
                profile = InvestorProfile.Conservador;

            if (pontuacaoTotal > 20 && pontuacaoTotal < 40)
                profile = InvestorProfile.Moderado;

            if (pontuacaoTotal >= 40)
                profile = InvestorProfile.Agressivo;

            return profile;
        }

        private async Task UpdateInvestorProfile(InvestorProfile investorProfile, Guid clientId)
        {
            var entity = await _repositorio.GetByFilterAsync(x => x.Id == clientId);

            entity.PerfilInvestimento = investorProfile.ToString();

            await _repositorio.UpdateAsync(x => x.Id == clientId, entity, CancellationToken.None);
        }
    }
}
