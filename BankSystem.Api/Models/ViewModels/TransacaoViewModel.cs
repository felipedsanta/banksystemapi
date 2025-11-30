namespace BankSystem.Api.Models.ViewModels
{
    public class TransacaoViewModel
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
        public Guid? ContaOrigemId { get; set; }
        public Guid? ContaDestinoId { get; set; }
    }
}
