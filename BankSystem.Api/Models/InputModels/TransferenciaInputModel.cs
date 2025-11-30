using System.ComponentModel.DataAnnotations;

public class TransferenciaInputModel
{
    [Required]
    public Guid ContaDestinoId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}