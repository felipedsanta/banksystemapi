using BankSystem.Api.Models;

namespace BankSystem.Api.Repositories
{
    public interface IClienteRepository
    {
        Task<bool> CpfJaExisteAsync(string cpf);
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<Cliente?> GetByCpfAsync(string cpf);
        Task<bool> ClienteExisteAsync(Guid id);
        Task AddAsync(Cliente cliente);
        Task<bool> SaveChangesAsync();
        Task<Cliente?> GetByEmailAsync(string email);
        void Delete(Cliente cliente);
    }
}