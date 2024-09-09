namespace FintechMessageConsumer.Domain.Entities
{
    public class ProductEntity : Entity
    {
        public string Titulo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public decimal ValorMinimo { get; set; }
        public string TaxaAdministracao { get; set; } = null!;
        public string RiscoProduto { get; set; } = null!;
        public string TipoAtivo { get; set; } = null!;
        public string CodigoAtivo { get; set; } = null!;
        public string PerfilInvestimento { get; set; } = null!;
    }
}
