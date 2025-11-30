using BankSystem.Api.Models.Enums;

public class ContaViewModel
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string? Titular { get; set; }
    public decimal Saldo { get; set; }
    public bool Ativa { get; set; }
    public TipoConta Tipo { get; set; }
    public DateTime DataCriacao { get; set; }

    public Guid ClienteId { get; set; }
}