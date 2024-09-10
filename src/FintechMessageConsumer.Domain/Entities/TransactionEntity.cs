namespace FintechMessageConsumer.Domain.Entities
{
    public class TransactionEntity : Entity
    {
        public Guid PortfolioId { get; set; }
        public Guid AtivoId { get; set; }
        public string TipoTransacao { get; set; } = null!;
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}
