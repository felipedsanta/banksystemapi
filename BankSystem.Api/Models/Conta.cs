using BankSystem.Api.Models.Enums;
using BankSystem.Api.Models.Interfaces;
using System.Text.Json.Serialization;


namespace BankSystem.Api.Models;

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "tipo_discriminator")]
    [JsonDerivedType(typeof(ContaCorrente), "corrente")]
    [JsonDerivedType(typeof(ContaPoupanca), "poupanca")]
    public abstract class Conta : ISoftDelete
    {
    public Guid Id { get; set; }
    public int Numero { get; private set; }
    public decimal Saldo { get; protected set; }
    public bool Ativo { get; private set; } = true;
    public TipoConta Tipo { get; private set; }
    public DateTime DataCriacao { get; private set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; private set; }
    public Guid ClienteId { get; private set; }
    public Cliente Cliente { get; private set; } = null!;

    protected Conta(int numero, decimal saldoInicial, TipoConta tipo, Guid clienteId)
    {
        Id = Guid.NewGuid();
        Numero = numero;
        Saldo = saldoInicial;
        Tipo = tipo;
        ClienteId = clienteId;
        DataCriacao = DateTime.UtcNow;
    }

    public virtual void Debitar(TransacaoInputModel input)
    {
        if (Saldo < input.Valor) throw new InvalidOperationException("Saldo insuficiente");
        Saldo -= input.Valor;
    }
    public virtual void Creditar(TransacaoInputModel input) => Saldo += input.Valor;

    public void Excluir()
    {
        if (Saldo != 0) throw new InvalidOperationException("Não é possível encerrar uma conta com saldo.");

        Ativo = false;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Restaurar()
    {
        Ativo = true;
        DataAtualizacao = DateTime.UtcNow;
    }
}
