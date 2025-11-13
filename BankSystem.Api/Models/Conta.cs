using BankSystem.Api.Models.Enums;

namespace BankSystem.Api.Models
{
    public class Conta
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Numero { get; private set; }
        public string ?Titular { get; private set; }
        public decimal Saldo { get; private set; }
        public TipoConta Tipo { get; private set; }
        public DateTime DataCriacao { get; private set; } = DateTime.Now;
        public Guid ClienteId { get; private set; }
        public Cliente Cliente { get; private set; } = null!;

        public Conta(int numero, string titular, decimal saldo, TipoConta tipo, Guid clienteId)
        {
            Numero = numero;
            Titular = titular;
            Saldo = saldo;
            Tipo = tipo;
            ClienteId = clienteId;
        }

        public bool Depositar(decimal valor)
        {
            if (valor <= 0) return false;
            Saldo += valor;
            return true;
        }
    }
}
