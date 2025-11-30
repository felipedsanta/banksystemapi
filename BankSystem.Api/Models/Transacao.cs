using System.Text.Json.Serialization;

namespace BankSystem.Api.Models
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public string Tipo { get; private set; } = string.Empty;
        public decimal Valor { get; private set; }
        public DateTime DataHora { get; private set; } = DateTime.Now;
        public Guid? ContaOrigemId { get; private set; }
        public Guid? ContaDestinoId { get; private set; }

        [JsonIgnore]
        public Conta? ContaOrigem { get; private set; }
        [JsonIgnore]
        public Conta? ContaDestino { get; set; }

        protected Transacao() { }

        public Transacao(string tipo, decimal valor, Guid? contaOrigemId, Guid? contaDestinoId)
        {
            Id = Guid.NewGuid();
            DataHora = DateTime.UtcNow; // Use UtcNow para padronizar o horário
            Tipo = tipo;
            Valor = valor;
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
        }
    }
}
