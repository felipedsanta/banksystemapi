using System.ComponentModel.DataAnnotations;

namespace BankSystem.Api.Models.InputModels
{
    public class EnderecoInputModel
    {
        [Required] public string Cep { get; set; }
        [Required] public string Logradouro { get; set; }
        [Required] public string Numero { get; set; }
        public string? Complemento { get; set; }
        [Required] public string Bairro { get; set; }
        [Required] public string Cidade { get; set; }
        [Required] public string Estado { get; set; }
    }
}
