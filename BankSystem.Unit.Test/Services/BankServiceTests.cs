using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Repositories;
using BankSystem.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BankSystem.Unit.Test.Services
{
    public class BankServiceTests
    {
        private readonly Mock<IContaRepository> _contaRepoMock;
        private readonly Mock<ITransacaoRepository> _transacaoRepoMock;
        private readonly Mock<ILogger<BankService>> _loggerMock;
        private readonly BankService _service;

        public BankServiceTests()
        {
            _contaRepoMock = new Mock<IContaRepository>();
            _transacaoRepoMock = new Mock<ITransacaoRepository>();
            _loggerMock = new Mock<ILogger<BankService>>();

            _service = new BankService(_contaRepoMock.Object, _transacaoRepoMock.Object, _loggerMock.Object);
        }

        private Cliente CriarClienteMock()
        {
            var cliente = new Cliente(
                "Felipe Silva",
                "44444444444",
                new DateTime(1997, 5, 20),
                "joao@email.com",
                "SenhaForte123!",
                "11911111111",
                3500,
                new Endereco("12311103", "Av Paulista", "2000", "Ap 3", "Boa Vista", "São Paulo", "SP")
                );
            cliente.Id = Guid.NewGuid();
            return cliente;
        }

        [Fact]
        public async Task TransferirAsync_DeContaCorrente_DeveCobrarTaxa()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();

            var contaOrigem = new ContaCorrente(12345, 100m, cliente.Id);
            // Destino tem 0
            var destino = new ContaCorrente(13425, 0.1m, cliente.Id);

            TransferenciaInputModel transferenciaInput = new TransferenciaInputModel
            {
                ContaDestinoId = destino.Id,
                Valor = 90m
            };
            _contaRepoMock.Setup(r => r.GetByIdAsync(contaOrigem.Id)).ReturnsAsync(contaOrigem);
            _contaRepoMock.Setup(r => r.GetByIdAsync(transferenciaInput.ContaDestinoId)).ReturnsAsync(destino);

            // Act
            await _service.TransferirAsync(contaOrigem.Id, transferenciaInput.ContaDestinoId, transferenciaInput);

            Assert.Equal(90.1m, destino.Saldo);

            //_transacaoRepoMock.Verify(r => r.AddAsync(It.Is<Transacao>(t => t.Tipo == "Transferencia")), Times.Once);
            _transacaoRepoMock.Verify(r => r.AddAsync(It.Is<Transacao>(t => t.Tipo.Contains("Taxa"))), Times.Once);

            // Verifica o Commit
            _contaRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task SacarAsync_SemSaldo_DeveLancarExcecao()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();

            var conta = new ContaCorrente(12345, 50m, cliente.Id); // Tem 50
            _contaRepoMock.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(conta);

            // Act & Assert
            // Tenta sacar 100 tendo apenas 50
            TransacaoInputModel saqueInput = new TransacaoInputModel(100);
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SacarAsync(cliente.Id, saqueInput));

            // Garante que NADA foi salvo
            _contaRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
