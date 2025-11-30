using System.ComponentModel.DataAnnotations;

public class TransacaoInputModel
{
    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, 1000000.00, ErrorMessage = "O valor da transação deve ser positivo e maior que zero.")]
    public decimal Valor { get; set; }

    public TransacaoInputModel(decimal valor)
    {
        Valor = valor;
    }
}