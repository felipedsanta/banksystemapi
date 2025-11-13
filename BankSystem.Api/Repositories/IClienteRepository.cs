using BankSystem.Api.Models;

namespace BankSystem.Api.Repositories
{
    public interface IClienteRepository
    {
        Task<bool> CpfJaExisteAsync(string cpf);
        Task<Cliente?> GetByIdAsync(Guid id);
        Task<bool> ExisteAsync(Guid id);
        Task AddAsync(Cliente cliente);
        Task<bool> SaveChangesAsync();
    }
}