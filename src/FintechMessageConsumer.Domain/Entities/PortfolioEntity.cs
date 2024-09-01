namespace FintechMessageConsumer.Domain.Entities
{
    public class PortfolioEntity : Entity
    {
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public List<Wallet> Carteira { get; set; }
    }

    public class Wallet
    {
        public Guid ProductId { get; set; }
    }
}
