using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;

namespace BankSystem.Api.Repositories
{
    public interface IContaRepository
    {
        Task<IEnumerable<Conta>> GetAllAsync(int pageNumber, int pageSize);
        Task<Conta?> GetByIdAsync(Guid id);
        Task<bool> NumeroContaJaExisteAsync(int numero);
        Task AddAsync(Conta conta);
        void Delete(Conta conta);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<Conta>> GetContasByClienteIdAsync(Guid clienteId);
    }
}