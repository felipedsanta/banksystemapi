namespace BankSystem.Api.Services
{
    public interface IFinancialAdvisorService
    {
        Task<string?> GerarAnaliseFinanceiraAsync(Guid contaId);
    }
}
