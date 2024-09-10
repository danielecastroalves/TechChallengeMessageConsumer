namespace FintechMessageConsumer.Domain.Entities
{
    public class PortfolioEntity : Entity
    {
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public List<Ativos> Ativos { get; set; } = [];
    }

    public class Ativos
    {
        public Guid IdProduto { get; set; }
        public int QuantidadeCotas { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
