namespace BankSystem.Api.Models.ValueObjects
{
    public class Endereco
    {
        public string Cep { get; private set; } = string.Empty;
        public string Logradouro { get; private set; } = string.Empty;
        public string Numero { get; private set; } = string.Empty;
        public string? Complemento { get; private set; }
        public string Bairro { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string Estado { get; private set; } = string.Empty;

        protected Endereco() { }

        public Endereco(string cep, string logradouro, string numero, 
            string? complemento, string bairro, string cidade, string estado)
        {
            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }
    }
}
