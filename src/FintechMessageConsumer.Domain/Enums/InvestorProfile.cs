using System.ComponentModel;

namespace FintechMessageConsumer.Domain.Enums
{
    public enum InvestorProfile
    {
        [Description("Indefinido")]
        Indefinido,

        [Description("Agressivo")]
        Agressivo,

        [Description("Moderado")]
        Moderado,

        [Description("Conservador")]
        Conservador
    }
}
