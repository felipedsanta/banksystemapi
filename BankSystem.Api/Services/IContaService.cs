using BankSystem.Api.Models;

namespace BankSystem.Api.Service
{
    public interface IContaService
    {
        Task<List<ContaViewModel>> GetAllContasAsync(int pageNumber, int pageSize);
        Task<List<ContaViewModel>> GetAllContasInclusiveDeletedAsync(int pageNumber, int pageSize);
        Task<ContaViewModel> AddAsync(ContaInputModel input);
        Task<ContaViewModel?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> NumeroContaJaExisteAsync(int numero);
        Task<List<ContaViewModel>> GetContasByClienteIdAsync(Guid clienteId);
    }
}
