namespace BankSystem.Api.Models
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string Nome { get; private set; }
        public string Cpf { get; private set; }

        public ICollection<Conta> Contas { get; private set; } = new List<Conta>();

        public Cliente(string nome, string cpf)
        {
            Nome = nome;
            Cpf = cpf;
        }
    }
}