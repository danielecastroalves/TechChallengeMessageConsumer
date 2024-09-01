using FintechMessageConsumer.Domain.Enums;

namespace FintechMessageConsumer.Domain.Entities
{
    public class ProductEntity : Entity
    {
        public string Titulo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string ValorMinimo { get; set; } = null!;
        public string TaxaAdministracao { get; set; } = null!;
        public string RiscoProduto { get; set; } = null!;
        public TipoAtivo TipoAtivo { get; set; }
        public string CodigoAtivo { get; set; }
        public InvestorProfile PerfilInvestimento { get; set; }
    }

    public enum TipoAtivo
    {
        Acoes,
        Titulos,
        Criptomoedas,
        TesouroDireto,
        FundosDeInvestimento,
        CDB,
        RendaFixa,
        RendaVariavel
    }
}
