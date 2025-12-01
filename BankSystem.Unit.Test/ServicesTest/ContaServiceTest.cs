using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;
using BankSystem.Api.Repositories;
using BankSystem.Api.Services;
using Moq;


namespace BankSystem.Unit.Test.Services
{
    public class ContaServiceTests
    {
        private readonly Mock<IContaRepository> _contaRepoMock;
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly ContaService _contaService;

        public ContaServiceTests()
        {
            _contaRepoMock = new Mock<IContaRepository>();
            _clienteRepoMock = new Mock<IClienteRepository>();
            _contaService = new ContaService(_contaRepoMock.Object, _clienteRepoMock.Object);

            _contaService = new ContaService(
            _contaRepoMock.Object,
            _clienteRepoMock.Object
            );
        }

        private Conta CriarContaMock(decimal saldoInicial = 1000m)
        {
            return new ContaCorrente(12345, saldoInicial, Guid.NewGuid());
        }

        [Fact]
        public async Task CreateContaAsync_QuandoCorrente_DeveSalvarContaCorrente()
        {
            // Arrange
            var input = new ContaInputModel
            {
                Numero = 123,
                Saldo = 0,
                ClienteId = Guid.NewGuid(),
                Tipo = TipoConta.Corrente
            };

            _contaRepoMock.Setup(r => r.NumeroContaJaExisteAsync(123)).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(input.ClienteId)).ReturnsAsync(true);

            // Act
            var result = await _contaService.AddAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TipoConta.Corrente, result.Tipo);

            // Verify: Garante que salvou uma ContaCorrente
            _contaRepoMock.Verify(r => r.AddAsync(It.Is<Conta>(c => c is ContaCorrente)), Times.Once);
            _contaRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllContasAsync_QuandoExistemContas_DeveRetornarListaDeViewModels()
        {
            // Arrange
            var conta = CriarContaMock();
            var listaDeContas = new List<Conta> { conta };

            _contaRepoMock.Setup(repo => repo.GetAllAsync(1, 10))
                          .ReturnsAsync(listaDeContas);

            // Act
            var result = await _contaService.GetAllContasAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.IsType<List<ContaViewModel>>(result);
            Assert.Equal(conta.Numero, result.First().Numero);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoContaExiste_DeveRetornarViewModel()
        {
            // Arrange
            var conta = CriarContaMock();
            _contaRepoMock.Setup(repo => repo.GetByIdAsync(conta.Id))
                          .ReturnsAsync(conta);

            // Act
            var result = await _contaService.GetByIdAsync(conta.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ContaViewModel>(result);
            Assert.Equal(conta.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNull()
        {
            // Arrange
            _contaRepoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                          .ReturnsAsync((Conta?)null);

            // Act
            var result = await _contaService.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        // --- TESTES DE CRIAÇÃO (POST) ---

        [Fact]
        public async Task CreateContaAsync_QuandoDadosValidos_DeveSalvarERetornarViewModel()
        {
            // Arrange
            var input = new ContaInputModel
            {
                Numero = 5555,
                Saldo = 50m,
                Tipo = TipoConta.Poupanca,
                ClienteId = Guid.NewGuid()
            };

            _contaRepoMock.Setup(r => r.NumeroContaJaExisteAsync(input.Numero)).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(input.ClienteId)).ReturnsAsync(true);

            // Act
            var viewModel = await _contaService.AddAsync(input);

            // Assert
            Assert.NotNull(viewModel);
            Assert.Equal(input.Numero, viewModel.Numero);
            Assert.Equal(TipoConta.Poupanca, viewModel.Tipo);

            // Verify
            _contaRepoMock.Verify(repo => repo.AddAsync(It.Is<Conta>(c => c is ContaPoupanca)), Times.Once);
            _contaRepoMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateContaAsync_QuandoNumeroJaExiste_DeveLancarExcecao()
        {
            // Arrange
            var input = new ContaInputModel { Numero = 12345 };
            _contaRepoMock.Setup(r => r.NumeroContaJaExisteAsync(input.Numero)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _contaService.AddAsync(input)
            );

            // Verify: Garante que NADA foi salvo
            _contaRepoMock.Verify(r => r.AddAsync(It.IsAny<Conta>()), Times.Never);
        }

        [Fact]
        public async Task CreateContaAsync_QuandoClienteNaoExiste_DeveLancarExcecao()
        {
            // Arrange
            var input = new ContaInputModel { Numero = 12345, ClienteId = Guid.NewGuid() };
            _contaRepoMock.Setup(r => r.NumeroContaJaExisteAsync(input.Numero)).ReturnsAsync(false);
            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(input.ClienteId)).ReturnsAsync(false); // Não existe

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _contaService.AddAsync(input)
            );
        }

        // --- TESTE DE EXCLUSÃO (DELETE) ---

        [Fact]
        public async Task DeletarContaAsync_QuandoContaExiste_DeveChamarDeleteESalvar()
        {
            // Arrange
            var conta = CriarContaMock();
            _contaRepoMock.Setup(r => r.GetByIdAsync(conta.Id)).ReturnsAsync(conta);

            // Act
            await _contaService.DeleteAsync(conta.Id);

            // Assert
            _contaRepoMock.Verify(r => r.Delete(conta), Times.Once);
            _contaRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}