namespace FintechMessageConsumer.Domain.Entities
{
    public class TransactionEntity : Entity
    {
        public Guid PortfolioId { get; set; }
        public Guid AtivoId { get; set; }
        public TransactionType TipoTransacao { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco {  get; set; }
    }

    public enum TransactionType
    {
        Buy,
        Sale
    }
}
