using BankSystem.Api.Data;
using BankSystem.Api.Models;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Unit.Test.Repositories
{
    public class ClienteRepositoryTests
    {
        private readonly DbContextOptions<BankSystemDbContext> _options;

        public ClienteRepositoryTests()
        {
            // Cria o banco em memória
            _options = new DbContextOptionsBuilder<BankSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private Cliente CriarClienteMock()
        {
            var cliente = new Cliente(
                "Cliente Teste",
                "11122233344",
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

        private BankSystemDbContext CriarContexto()
        {
            return new BankSystemDbContext(_options);
        }

        [Fact]
        public async Task CpfJaExisteAsync_QuandoCpfExiste_DeveRetornarTrue()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();
            using (var context = CriarContexto())
            {
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                var existe = await repository.CpfJaExisteAsync(cliente.Cpf);

                // Assert
                Assert.True(existe);
            }
        }

        [Fact]
        public async Task CpfJaExisteAsync_QuandoCpfNaoExiste_DeveRetornarFalse()
        {
            // Arrange (Neste caso o banco é vazio)

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                var existe = await repository.CpfJaExisteAsync("99988877766"); // CPF que não está lá

                // Assert
                Assert.False(existe);
            }
        }

        [Fact]
        public async Task GetByIdAsync_QuandoClienteExiste_DeveRetornarCliente()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();
            using (var context = CriarContexto())
            {
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                var resultado = await repository.GetByIdAsync(cliente.Id);

                // Assert
                Assert.NotNull(resultado);
                Assert.Equal("Cliente Teste", resultado.Nome);
            }
        }

        [Fact]
        public async Task ExisteAsync_QuandoIdExiste_DeveRetornarTrue()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();
            using (var context = CriarContexto())
            {
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                var existe = await repository.ClienteExisteAsync(cliente.Id);

                // Assert
                Assert.True(existe);
            }
        }


        [Fact]
        public async Task AddAsync_DeveAdicionarClienteNoContexto()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                await repository.AddAsync(cliente);
                await context.SaveChangesAsync();
            }

            // Assert (Verificar se persistiu)
            using (var context = CriarContexto())
            {
                var clienteSalvo = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf == "11122233344");

                Assert.NotNull(clienteSalvo);
                Assert.Equal("Cliente Teste", clienteSalvo.Nome);
            }
        }
        [Fact]
        public async Task SaveChangesAsync_QuandoHaAlteracoes_DeveRetornarTrue()
        {
            // Arrange
            using (var context = CriarContexto())
            {
                var repository = new ClienteRepository(context);
                Cliente cliente = CriarClienteMock();
                await repository.AddAsync(cliente);
                // Act
                var resultado = await repository.SaveChangesAsync();
                // Assert
                Assert.True(resultado);
            }
        }
    }
}