using BankSystem.Api.Data;
using BankSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.Repositories
{
    public class ContaRepository(BankSystemDbContext context) : IContaRepository
    {
        public async Task<IEnumerable<Conta>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await context.Contas
                .AsNoTracking()
                .OrderBy(c => c.Numero)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conta>> GetContasByClienteIdAsync(Guid clienteId)
        {
            return await context.Contas
                .AsNoTracking()
                .Where(c => c.ClienteId == clienteId)
                .OrderBy(c => c.Numero)
                .ToListAsync();
        }

        public async Task<Conta?> GetByIdAsync(Guid id)
        {
            return await context.Contas.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> NumeroContaJaExisteAsync(int numero)
        {
            return await context.Contas
                .AsNoTracking()
                .AnyAsync(c => c.Numero == numero);
        }

        public async Task AddAsync(Conta conta)
        {
            await context.Contas.AddAsync(conta);
        }

        public void Delete(Conta conta)
        {
            context.Contas.Remove(conta);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}