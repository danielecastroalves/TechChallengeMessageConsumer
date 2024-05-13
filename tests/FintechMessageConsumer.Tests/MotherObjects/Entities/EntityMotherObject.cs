using Bogus;
using FintechMessageConsumer.Domain.Entities;

namespace FintechMessageConsumer.Tests.MotherObjects.Entities
{
    public class MockedEntity : Entity;

    public static class EntityMotherObject
    {
        public static MockedEntity ValidObject()
        {
            return new Faker<MockedEntity>()
                .CustomInstantiator(_ => Activator.CreateInstance<MockedEntity>())
                .RuleFor(x => x.Id, f => f.Lorem.Random.Guid())
                .RuleFor(x => x.DataInsercao, f => f.Date.Recent())
                .RuleFor(x => x.DataAtualizacao, f => f.Date.Recent())
                .Generate();
        }
    }
}
