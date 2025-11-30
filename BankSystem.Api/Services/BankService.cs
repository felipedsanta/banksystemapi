using BankSystem.Api.Models;
using BankSystem.Api.Repositories;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace BankSystem.Api.Services
{
    public class BankService(IContaRepository contaRepo, ITransacaoRepository transacaoRepo, ILogger<BankService> logger) : IBankService
    {
        public async Task<ContaViewModel?> DepositarAsync(Guid id, TransacaoInputModel input)
        {
            if (input.Valor <= 0) throw new ArgumentException("O valor deve ser positivo.");
            var conta = await contaRepo.GetByIdAsync(id);
            if (conta == null)
            {
                throw new KeyNotFoundException("Conta não encontrada.");
            }
            conta.Creditar(input);

            var transacao = new Transacao("Deposito", input.Valor, null, conta.Id);
            await transacaoRepo.AddAsync(transacao);

            await contaRepo.SaveChangesAsync();

            logger.LogInformation($"Depósito de R$ {input.Valor} realizado com sucesso na Conta {conta.Id}.");
            return MapToViewModel(conta);
        }

        public async Task<ContaViewModel?> SacarAsync(Guid Id, TransacaoInputModel input)
        {
            if (input.Valor <= 0) throw new ArgumentException("O valor do saque deve ser positivo.");

            var conta = await contaRepo.GetByIdAsync(Id);
            if (conta == null)
            {
                throw new KeyNotFoundException($"Conta {Id} não encontrada.");
            }
            conta.Debitar(input);

            var transacao = new Transacao("Saque", input.Valor, Id, null);
            await transacaoRepo.AddAsync(transacao);

            await contaRepo.SaveChangesAsync();

            logger.LogInformation($"Saque de R$ {input.Valor} realizado com sucesso na Conta {conta.Id}. Saldo restante: {conta.Saldo}");
            return MapToViewModel(conta);
        }

        public async Task TransferirAsync(Guid origemId, Guid destinoId, TransferenciaInputModel input)
        {
            if (origemId == destinoId) throw new ArgumentException("Não é possível transferir para a mesma conta.");
            if (input.Valor <= 0) throw new ArgumentException("O valor a ser transferido deve ser positivo.");

            var inicio = DateTime.Now;

            var origem = await contaRepo.GetByIdAsync(origemId);
            if (origem == null)
            {
                throw new KeyNotFoundException("Conta não encontrada.");
            }
            var destino = await contaRepo.GetByIdAsync(destinoId);
            if (destino == null)
            {
                throw new KeyNotFoundException("Conta não encontrada.");
            }

            logger.LogDebug("Iniciando transferência de {Origem} para {Destino}", origemId, destinoId);

            origem.Debitar(new TransacaoInputModel(input.Valor));
            var logSaque = new Transacao("Transferencia enviada", input.Valor, origemId, destinoId);

            destino.Creditar(new TransacaoInputModel(input.Valor));
            var logDeposito = new Transacao("Transferencia recebida", input.Valor, origemId, destinoId);

            await transacaoRepo.AddAsync(logSaque);
            await transacaoRepo.AddAsync(logDeposito);

            if (origem is ContaCorrente)
            {
                var taxa = input.Valor * 0.005m;
                if (taxa > 0)
                {
                    origem.Debitar(new TransacaoInputModel(taxa));
                    var logTaxa = new Transacao("Taxa_transferencia", taxa, origemId, null);
                    await transacaoRepo.AddAsync(logTaxa);
                }
            }
            await contaRepo.SaveChangesAsync();

            var duracao = DateTime.Now - inicio;
            logger.LogInformation("Transferência de R$ {Valor} finalizada. De: {OrigemId} Para: {DestinoId}. Tempo: {Duracao}ms",
                input.Valor, origemId, destinoId, duracao.TotalMilliseconds);
        }

        public async Task<IEnumerable<Transacao>> GetExtratoAsync(Guid Id)
        {
            if (await contaRepo.GetByIdAsync(Id) == null)
            {
                logger.LogWarning("Tentativa de acesso a extrato de conta inexistente: {ContaId}", Id);
                throw new KeyNotFoundException("Conta não encontrada.");
            }

            return await transacaoRepo.GetExtratoByContaIdAsync(Id);
        }

        private ContaViewModel MapToViewModel(Conta conta)
        {
            return new ContaViewModel
            {
                Id = conta.Id,
                Numero = conta.Numero,
                Saldo = conta.Saldo,
                DataCriacao = conta.DataCriacao,
                Tipo = conta.Tipo,
                ClienteId = conta.ClienteId
            };
        }
    }
}
