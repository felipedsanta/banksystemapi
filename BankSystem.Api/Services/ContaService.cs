using BankSystem.Api.Models;
using BankSystem.Api.Repositories;
using BankSystem.Api.Service;


namespace BankSystem.Api.Services
{
    public class ContaService(IContaRepository contaRepo) : IContaService
    {
        public async Task<List<ContaViewModel>> GetContasAsync(int pageNumber, int pageSize)
        {
            var contas = await contaRepo.GetAllAsync(pageNumber, pageSize);
            var viewModels = contas.Select(MapToViewModel).ToList();
            return viewModels;
        }

        public async Task<ContaViewModel> AddAsync(ContaInputModel input)
        {
            var conta = new Conta(
                input.Numero,
                input.Titular!,
                input.Saldo,
                input.Tipo,
                input.ClienteId
            );
            await contaRepo.AddAsync(conta);
            await contaRepo.SaveChangesAsync();

            return MapToViewModel(conta);
        }

        public async Task<bool> NumeroContaJaExisteAsync(int numero)
        {
            return await contaRepo.NumeroContaJaExisteAsync(numero);
        }

        public async Task<ContaViewModel?> GetByIdAsync(Guid id)
        {
            var conta = await contaRepo.GetByIdAsync(id);
            if (conta == null)
            {
                return null;
            }
            return MapToViewModel(conta);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var conta = await contaRepo.GetByIdAsync(id);
            if (conta == null)
            {
                return false;
            }
            contaRepo.Delete(conta);
            await contaRepo.SaveChangesAsync();
            return true;
        }

        public async Task<ContaViewModel> Depositar(Guid id, decimal valor)
        {
            var conta = await contaRepo.GetByIdAsync(id);
            if (conta == null)
            {
                throw new KeyNotFoundException("Conta não encontrada.");
            }
            conta.Depositar(valor);
            await contaRepo.SaveChangesAsync();
            return MapToViewModel(conta);
        }
        private ContaViewModel MapToViewModel(Conta conta)
        {
            return new ContaViewModel
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                DataCriacao = conta.DataCriacao,
                Tipo = conta.Tipo
            };
        }
    }
}