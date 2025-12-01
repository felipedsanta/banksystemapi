using BankSystem.Api.Models;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Repositories;

namespace BankSystem.Api.Services
{
    public class ClienteService(IClienteRepository clienteRepo, IContaRepository contaRepo, IAuthService authService) : IClienteService
    {
        public async Task<bool> CpfJaExisteAsync(string cpf)
        {
            return await clienteRepo.CpfJaExisteAsync(cpf);
        }

        public async Task<bool> ClienteExisteAsync(Guid id)
        {
            return await clienteRepo.ClienteExisteAsync(id);
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
            if (await CpfJaExisteAsync(input.Cpf))
            {
                throw new InvalidOperationException("Um cliente com este CPF já existe.");
            }
            // Gera o Hash
            string senhaHash = authService.GerarHashSenha(input.Senha);

            var cliente = new Cliente(
                input.Nome,
                input.Cpf,
                input.DataNascimento,
                input.Email,
                input.Senha = senhaHash, // <--- SALVA O HASH
                input.Celular,
                input.RendaMensal,
                new Endereco(
                    input.Endereco.Cep,
                    input.Endereco.Logradouro,
                    input.Endereco.Numero,
                    input.Endereco.Complemento,
                    input.Endereco.Bairro,
                    input.Endereco.Cidade,
                    input.Endereco.Estado
                    )
            );

            await clienteRepo.AddAsync(cliente);
            await clienteRepo.SaveChangesAsync();

            return MapClienteToViewModel(cliente);
        }

        public async Task<IEnumerable<ContaViewModel>> GetContasDoClienteAsync(Guid clienteId)
        {
            if (!await ClienteExisteAsync(clienteId))
            {
                throw new KeyNotFoundException("Cliente não encontrado.");
            }
            var contas = await contaRepo.GetContasByClienteIdAsync(clienteId);
            return contas.Select(MapContaToViewModel).ToList();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cliente = await clienteRepo.GetByIdAsync(id);
            if (cliente == null)
            {
                return false;
            }

            var contas = await contaRepo.GetContasByClienteIdAsync(id);
            if (contas.Any(c => c.Saldo > 0))
            {
                throw new InvalidOperationException("Não é possível inativar cliente com saldo em conta. Realize o saque ou transferência antes.");
            }

            foreach (var conta in contas)
            {
                contaRepo.Delete(conta);
            }

            clienteRepo.Delete(cliente);
            await clienteRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestaurarClienteAsync(Guid id)
        {
            var cliente = await clienteRepo.GetByIdInclusiveDeletedAsync(id);

            if (cliente == null)
            {
                throw new KeyNotFoundException("Cliente não encontrado (nem entre os inativos).");
            }
            if (cliente.Ativo)
            {
                throw new InvalidOperationException("Este cliente já está ativo.");
            }
            cliente.Restaurar();

            foreach (var conta in cliente.Contas)
            {
                conta.Restaurar();
            }
            await clienteRepo.SaveChangesAsync();
            return true;
        }

        private ContaViewModel MapContaToViewModel(Conta conta)
        {
            return new ContaViewModel
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Titular = conta.Cliente != null ? conta.Cliente.Nome : "Cliente não carregado",
                Saldo = conta.Saldo,
                Ativa = conta.Ativo,
                Tipo = conta.Tipo,
                DataCriacao = conta.DataCriacao,
                ClienteId = conta.ClienteId
            };
        }

        private ClienteViewModel MapClienteToViewModel(Cliente cliente)
        {
            return new ClienteViewModel
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Cpf = cliente.Cpf,
                DataNascimento = cliente.DataNascimento,
                Celular = cliente.Celular,
                RendaMensal = cliente.RendaMensal,
                Endereco = new EnderecoViewModel
                {
                    Cep = cliente.Endereco.Cep,
                    Logradouro = cliente.Endereco.Logradouro,
                    Numero = cliente.Endereco.Numero,
                    Complemento = cliente.Endereco.Complemento,
                    Bairro = cliente.Endereco.Bairro,
                    Cidade = cliente.Endereco.Cidade,
                    Estado = cliente.Endereco.Estado
                },
                Role = cliente.Role,
            };
        }
    }
}