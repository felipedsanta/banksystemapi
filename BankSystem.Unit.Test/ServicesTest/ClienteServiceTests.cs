using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;
using BankSystem.Api.Models.InputModels;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Repositories;
using BankSystem.Api.Services;
using Moq;

namespace BankSystem.Unit.Test.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<IClienteRepository> _clienteRepoMock;
        private readonly Mock<IContaRepository> _contaRepoMock;
        private readonly Mock<IAuthService> _authServiceMock;

        private readonly ClienteService _clienteService;

        public ClienteServiceTests()
        {
            _clienteRepoMock = new Mock<IClienteRepository>();
            _contaRepoMock = new Mock<IContaRepository>();
            _authServiceMock = new Mock<IAuthService>();

            _clienteService = new ClienteService(
             _clienteRepoMock.Object,
             _contaRepoMock.Object,
             _authServiceMock.Object);
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
        public async Task CpfJaExisteAsync_QuandoRepoRetornaTrue_DeveRetornarTrue()
        {
            // Arrange
            string cpf = "44444444444";
            _clienteRepoMock.Setup(r => r.CpfJaExisteAsync(cpf)).ReturnsAsync(true);

            // Act
            var result = await _clienteService.CpfJaExisteAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ClienteExisteAsync_QuandoRepoRetornaFalse_DeveRetornarFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _clienteService.ClienteExisteAsync(id);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public async Task GetClientePorIdAsync_QuandoClienteExiste_DeveRetornarViewModel()
        {
            // Arrange
            var cliente = CriarClienteMock();
            _clienteRepoMock.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);

            // Act
            var result = await _clienteService.GetClientePorIdAsync(cliente.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cliente.Id, result.Id);
            Assert.Equal(cliente.Nome, result.Nome);
            Assert.Equal(cliente.Cpf, result.Cpf);
        }

        [Fact]
        public async Task GetClientePorIdAsync_QuandoNaoExiste_DeveRetornarNull()
        {
            // Arrange
            _clienteRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

            // Act
            var result = await _clienteService.GetClientePorIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }


        [Fact]
        public async Task CriarClienteAsync_DeveGerarHashSalvarERetornarViewModel()
        {
            // Arrange
            var input = new ClienteInputModel
            {
                Nome = "Teste",
                Cpf = "11122233344",
                Email = "teste@email.com",
                Senha = "SenhaForte123!",
                DataNascimento = new DateTime(1990, 1, 1),
                Celular = "11999999999",
                RendaMensal = 5000,
                Endereco = new EnderecoInputModel { Cep = "00000000", Logradouro = "Rua", Numero = "1", Bairro = "B", Cidade = "C", Estado = "SP" }
            };

            // Simula o AuthService gerando um hash falso
            _authServiceMock.Setup(a => a.GerarHashSenha(input.Senha)).Returns("HASH_FAKE_123");

            // Act
            var result = await _clienteService.CriarClienteAsync(input);

            // Assert
            Assert.NotNull(result);

            // Verifica se o repositório foi chamado com um cliente que tem o HASH, não a senha
            _clienteRepoMock.Verify(r => r.AddAsync(It.Is<Cliente>(c => c.SenhaHash == "HASH_FAKE_123")), Times.Once);
        }

        [Fact]
        public async Task GetContasDoClienteAsync_DeveRetornarListaDeContaViewModel()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();

            // Criar uma lista falsa de contas
            var contas = new List<Conta>
            {
                new ContaPoupanca(1001, 100m, cliente.Id),
                new ContaPoupanca(1002, 200m, cliente.Id)
            };

            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(cliente.Id))
                    .ReturnsAsync(true);

            // Configurar o Mock do Repositório de CONTAS
            _contaRepoMock.Setup(r => r.GetContasByClienteIdAsync(cliente.Id))
                          .ReturnsAsync(contas);

            // Act
            var result = await _clienteService.GetContasDoClienteAsync(cliente.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var primeiraConta = result.First();
            Assert.Equal(1001, primeiraConta.Numero);
            Assert.Equal(cliente.Id, primeiraConta.ClienteId);
        }

        [Fact]
        public async Task GetContasDoClienteAsync_QuandoSemContas_DeveRetornarListaVazia()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();

            _clienteRepoMock.Setup(r => r.ClienteExisteAsync(cliente.Id))
                    .ReturnsAsync(true);

            _contaRepoMock.Setup(r => r.GetContasByClienteIdAsync(cliente.Id))
                          .ReturnsAsync(new List<Conta>()); // Lista vazia

            // Act
            var result = await _clienteService.GetContasDoClienteAsync(cliente.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}