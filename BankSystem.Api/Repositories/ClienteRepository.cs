using BankSystem.Api.Data;
using BankSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Api.Repositories
{
    public class ClienteRepository(BankSystemDbContext context) : IClienteRepository
    {
        public async Task<bool> CpfJaExisteAsync(string cpf)
        {
            return await context.Clientes.AsNoTracking().AnyAsync(c => c.Cpf == cpf);
        }

        public async Task<Cliente?> GetByCpfAsync(string cpf)
        {
            return await context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Cpf == cpf);
        }

        public async Task<Cliente?> GetByIdAsync(Guid id)
        {
            return await context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ClienteExisteAsync(Guid id)
        {
            return await context.Clientes.AsNoTracking().AnyAsync(c => c.Id == id);
        }

        public async Task AddAsync(Cliente cliente)
        {
            await context.Clientes.AddAsync(cliente);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await context.SaveChangesAsync()) > 0;
        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}