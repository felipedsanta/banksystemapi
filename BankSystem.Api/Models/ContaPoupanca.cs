using BankSystem.Api.Models.Enums;

namespace BankSystem.Api.Models
{
    public class ContaPoupanca : Conta
    {
        public ContaPoupanca(int numero, decimal saldo, Guid clienteId)
        : base(numero, saldo, TipoConta.Poupanca, clienteId) { }
    }
}
