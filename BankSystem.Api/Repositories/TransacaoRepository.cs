using BankSystem.Api.Data;
using BankSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.Repositories;

public class TransacaoRepository(BankSystemDbContext context) : ITransacaoRepository
{
    public async Task AddAsync(Transacao transacao)
    {
        await context.Transacoes.AddAsync(transacao);
    }

    public async Task<IEnumerable<Transacao>> GetExtratoByContaIdAsync(Guid contaId)
    {
        return await context.Transacoes
            .AsNoTracking()
            .Where(t =>
            (t.ContaOrigemId == contaId && t.Tipo != "Transferencia Recebida") || (t.ContaDestinoId == contaId && t.Tipo != "Transferencia Enviada"))
            .OrderByDescending(t => t.DataHora)
            .ToListAsync();
    }
}