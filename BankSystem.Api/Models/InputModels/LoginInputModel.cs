using System.ComponentModel.DataAnnotations;

namespace BankSystem.Api.Models.InputModels
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
