using System.ComponentModel;

namespace FintechMessageConsumer.Domain.Enums
{
    public enum TransacationType
    {
        [Description("Compra")]
        Compra,

        [Description("Venda")]
        Venda
    }
}
