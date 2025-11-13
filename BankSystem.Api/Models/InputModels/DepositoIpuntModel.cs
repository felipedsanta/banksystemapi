using System.ComponentModel.DataAnnotations;

public class DepositoInputModel
{
    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, 1000000.00, ErrorMessage = "O valor do depósito deve ser positivo e maior que zero.")]
    public decimal Valor { get; set; }
}