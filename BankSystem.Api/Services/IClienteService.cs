namespace BankSystem.Api.Services
{
    public interface IClienteService
    {
        Task<ClienteViewModel?> GetClientePorIdAsync(Guid id);
        Task<ClienteViewModel> CriarClienteAsync(ClienteInputModel input);
        Task<IEnumerable<ContaViewModel>> GetContasDoClienteAsync(Guid clienteId);
        Task<bool> CpfJaExisteAsync(string cpf);
        Task<bool> ClienteExisteAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestaurarClienteAsync(Guid id);
    }
}
