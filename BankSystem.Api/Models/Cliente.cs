using BankSystem.Api.Models.Interfaces;
using BankSystem.Api.Models.ValueObjects;
using System.Text.Json.Serialization;

namespace BankSystem.Api.Models
{
    public class Cliente : ISoftDelete
    {
        public Guid Id { get; set; }
        public string Cpf { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }
        public string Celular { get; private set; } = string.Empty;
        public decimal RendaMensal { get; private set; }
        public Endereco Endereco { get; private set; } = null!;
        public string Email { get; private set; }

        [JsonIgnore]
        public string SenhaHash { get; private set; }
        public string Role { get; private set; }

        public bool Ativo { get; private set; } = true;
        public DateTime DataCadastro { get; private set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; private set; }

        public ICollection<Conta> Contas { get; private set; } = new List<Conta>();

        protected Cliente() { }

        public Cliente(string nome, string cpf, DateTime dataNascimento, string email, string senhaHash, string celular, decimal rendaMensal, Endereco endereco, string role = "Cliente")
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Email = email;
            SenhaHash = senhaHash;
            Celular = celular;
            RendaMensal = rendaMensal;
            Endereco = endereco;
            Role = role;
            DataCadastro = DateTime.UtcNow;
        }

        public void Excluir()
        {
            if (!Ativo) return;
            Ativo = false;
            DataAtualizacao = DateTime.UtcNow;
        }

        public void Restaurar()
        {
            Ativo = true;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}