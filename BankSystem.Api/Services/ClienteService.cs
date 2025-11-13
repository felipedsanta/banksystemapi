using BankSystem.Api.Models;
using BankSystem.Api.Repositories;

namespace BankSystem.Api.Services
{
    public class ClienteService(IClienteRepository clienteRepo, IContaRepository contaRepo) : IClienteService
    {
        public async Task<bool> CpfJaExisteAsync(string cpf)
        {
            return await clienteRepo.CpfJaExisteAsync(cpf);
        }

        public async Task<bool> ClienteExisteAsync(Guid id)
        {
            return await clienteRepo.ExisteAsync(id);
        }

        public async Task<ClienteViewModel?> GetClientePorIdAsync(Guid id)
        {
            var cliente = await clienteRepo.GetByIdAsync(id);
            if (cliente == null)
            {
                return null;
            }
            return MapClienteToViewModel(cliente);
        }

        public async Task<ClienteViewModel> CriarClienteAsync(ClienteInputModel input)
        {
            var cliente = new Cliente(input.Nome, input.Cpf);

            await clienteRepo.AddAsync(cliente);
            await clienteRepo.SaveChangesAsync();

            return MapClienteToViewModel(cliente);
        }

        public async Task<IEnumerable<ContaViewModel>> GetContasDoClienteAsync(Guid clienteId)
        {
            var contas = await contaRepo.GetContasByClienteIdAsync(clienteId);
            return contas.Select(MapContaToViewModel).ToList();
        }

        private ClienteViewModel MapClienteToViewModel(Cliente cliente)
        {
            return new ClienteViewModel
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf
            };
        }

        private ContaViewModel MapContaToViewModel(Conta conta)
        {
            return new ContaViewModel
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Titular = conta.Titular,
                Saldo = conta.Saldo,
                Tipo = conta.Tipo,
                DataCriacao = conta.DataCriacao,
                ClienteId = conta.ClienteId
            };
        }
    }
}