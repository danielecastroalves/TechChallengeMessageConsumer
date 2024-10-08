using FintechMessageConsumer.Domain.Enums;

namespace FintechMessageConsumer.Domain.Entities
{
    public class ClienteEntity : User
    {
        public string NomeCliente { get; set; } = null!;
        public string Documento { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DataNascimento { get; set; }
        public string PerfilInvestimento { get; set; } = nameof(InvestorProfile.Indefinido);
        public List<Guid> Portfolios { get; set; } = [];
    }

    public class User : Entity
    {
        public string Login { get; set; } = null!;
        public string Senha { get; set; } = null!;
        public Roles Permissao { get; set; }
    }
}
