using BankSystem.Api.Models.Enums;
using System.ComponentModel.DataAnnotations;

public class ContaInputModel
{
    [Required(ErrorMessage = "O número da conta é obrigatório.")]
    [Range(1000, 99999, ErrorMessage = "O número da conta deve ser um valor entre 1000 e 99999.")]
    public int Numero { get; set; }

    [Required(ErrorMessage = "O nome do titular é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome do titular deve ter entre 3 e 100 caracteres.")]
    public string? Titular { get; set; }

    [Required(ErrorMessage = "O depósito inicial é obrigatório.")]
    [Range(0.01, 10000.00, ErrorMessage = "O depósito inicial deve ser entre R$ 0,01 e R$ 10.000,00.")]
    public decimal Saldo { get; set; }

    [Required(ErrorMessage = "O tipo é obrigatório.")]
    [EnumDataType(typeof(TipoConta), ErrorMessage = "Tipo de conta inválido.")]
    public TipoConta Tipo { get; set; }

    [Required(ErrorMessage = "Toda conta deve pertencer a um cliente.")]
    public Guid ClienteId { get; set; }
}