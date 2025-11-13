using BankSystem.Api.Models;

namespace BankSystem.Api.Service
{
    public interface IContaService
    {
        Task<List<ContaViewModel>> GetContasAsync(int pageNumber, int pageSize);
        Task<ContaViewModel> AddAsync(ContaInputModel input);
        Task<ContaViewModel?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<ContaViewModel> Depositar(Guid id, decimal valor);
        Task<bool> NumeroContaJaExisteAsync(int numero);
    }
}
