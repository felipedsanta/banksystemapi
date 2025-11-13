using System.ComponentModel.DataAnnotations;

public class ClienteInputModel
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(11)]
    public string Cpf { get; set; } = string.Empty;
}