using BankSystem.Api.Models;

namespace BankSystem.Api.Repositories;

public interface ITransacaoRepository
{
    Task AddAsync(Transacao transacao);
    Task<IEnumerable<Transacao>> GetExtratoByContaIdAsync(Guid contaId);
}