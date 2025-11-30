using BankSystem.Api.Models.InputModels;

namespace BankSystem.Api.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginInputModel input);

        string GerarHashSenha(string senha);
    }
}
