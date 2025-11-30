public class ClienteViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Celular { get; set; } = string.Empty;
    public decimal RendaMensal { get; set; }
    public EnderecoViewModel Endereco { get; set; } = null!;
    public string Role { get; set; } = string.Empty;

}