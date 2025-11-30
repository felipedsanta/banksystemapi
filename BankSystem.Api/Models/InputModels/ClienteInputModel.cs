using BankSystem.Api.Models.InputModels;
using System.ComponentModel.DataAnnotations;

public class ClienteInputModel
{
    [Required] 
    public string Nome { get; set; }
    [Required] 
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 dígitos.")]
    public string Cpf { get; set; }
    [Required] 
    public DateTime DataNascimento { get; set; }
    [Required]
    [EmailAddress] 
    public string Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "A senha deve ter no mínimo 8 caracteres, contendo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.")]
    public string Senha { get; set; } // Senha raw
    [Required] 
    public string Celular { get; set; }
    [Required] 
    public decimal RendaMensal { get; set; }
    [Required] 
    public EnderecoInputModel Endereco { get; set; }
}