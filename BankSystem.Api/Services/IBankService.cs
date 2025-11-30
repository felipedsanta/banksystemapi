using BankSystem.Api.Models;

namespace BankSystem.Api.Services
{
    public interface IBankService
    {
        Task<ContaViewModel?> DepositarAsync(Guid contaId, TransacaoInputModel input);
        Task<ContaViewModel?> SacarAsync(Guid contaId, TransacaoInputModel input);
        Task TransferirAsync(Guid origemId, Guid destinoId, TransferenciaInputModel input);
        Task<IEnumerable<Transacao>> GetExtratoAsync(Guid contaId);
    }
}
