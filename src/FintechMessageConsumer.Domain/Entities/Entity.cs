namespace FintechMessageConsumer.Domain.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public DateTime DataInsercao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
            Ativo = true;
            SetDataInsercao();
            SetDataAtualizacao();
        }

        public void SetDataInsercao()
        {
            DataInsercao = DateTime.UtcNow;
            SetDataAtualizacao();
        }

        public void SetDataAtualizacao()
        {
            DataAtualizacao = DateTime.UtcNow;
        }

        public void AtivaEntidade()
        {
            Ativo = true;
            SetDataAtualizacao();
        }

        public void InativaEntidade()
        {
            Ativo = false;
            SetDataAtualizacao();
        }
    }
}
