using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;
using BankSystem.Api.Repositories;
using BankSystem.Api.Service;


namespace BankSystem.Api.Services
{
    public class ContaService(IContaRepository contaRepo, IClienteRepository clienteRepo) : IContaService
    {
        public async Task<List<ContaViewModel>> GetAllContasAsync(int pageNumber, int pageSize)
        {
            var contas = await contaRepo.GetAllAsync(pageNumber, pageSize);
            if(contas is null) 
            {
                return new List<ContaViewModel>();
            }
            var viewModels = contas.Select(MapToViewModel).ToList();
            return viewModels;
        }

        public async Task<List<ContaViewModel>> GetAllContasInclusiveDeletedAsync(int pageNumber, int pageSize)
        {
            var contas = await contaRepo.GetAllInclusiveDeletedAsync(pageNumber, pageSize);
            if (contas is null)
            {
                return new List<ContaViewModel>();
            }
            var viewModels = contas.Select(MapToViewModel).ToList();
            return viewModels;
        }

        public async Task<List<ContaViewModel>> GetContasByClienteIdAsync(Guid clienteId)
        {
            var contas = await contaRepo.GetContasByClienteIdAsync(clienteId);
            var viewModels = contas.Select(MapToViewModel).ToList();
            return viewModels;
        }

        public async Task<ContaViewModel> AddAsync(ContaInputModel inputModel)
        {
            if (await contaRepo.NumeroContaJaExisteAsync(inputModel.Numero))
            {
                throw new InvalidOperationException("Este número de conta já está em uso.");
            }

            if (!await clienteRepo.ClienteExisteAsync(inputModel.ClienteId))
            {
                throw new KeyNotFoundException("O ClienteId fornecido não existe.");
            }

            Conta novaConta;
            switch (inputModel.Tipo)
            {
                case TipoConta.Corrente:
                    novaConta = new ContaCorrente(
                        inputModel.Numero,
                        inputModel.Saldo,
                        inputModel.ClienteId
                    );
                    break;

                case TipoConta.Poupanca:
                    novaConta = new ContaPoupanca(
                        inputModel.Numero,
                        inputModel.Saldo,
                        inputModel.ClienteId
                    );
                    break;

                default:
                    throw new ArgumentException("Tipo de conta inválido.");
            }
            await contaRepo.AddAsync(novaConta);
            await contaRepo.SaveChangesAsync();

            return MapToViewModel(novaConta);
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

        private ContaViewModel MapToViewModel(Conta conta)
        {
            return new ContaViewModel
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Titular = conta.Cliente != null ? conta.Cliente.Nome : "Cliente não carregado",
                Saldo = conta.Saldo,
                DataCriacao = conta.DataCriacao,
                Tipo = conta.Tipo,
                ClienteId = conta.ClienteId
            };
        }
    }
}