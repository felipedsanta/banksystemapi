using BankSystem.Api.Models.Enums;

namespace BankSystem.Api.Models
{
    public class ContaCorrente : Conta
    {
        public ContaCorrente(int numero, decimal saldo, Guid clienteId)
        : base(numero, saldo, TipoConta.Corrente, clienteId) { }
    }
}
