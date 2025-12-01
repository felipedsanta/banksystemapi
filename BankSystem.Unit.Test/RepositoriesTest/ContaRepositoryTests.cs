using BankSystem.Api.Data;
using BankSystem.Api.Models;
using BankSystem.Api.Models.Enums;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Unit.Test.Repositories
{
    public class ContaRepositoryTests
    {
        private readonly DbContextOptions<BankSystemDbContext> _options;

        public ContaRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<BankSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private BankSystemDbContext CriarContexto()
        {
            return new BankSystemDbContext(_options);
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

        [Fact]
        public async Task GetContasByClienteIdAsync_DeveRetornarContasCorretas()
        {
            // Arrange
            Cliente cliente = CriarClienteMock();
            using (var context = CriarContexto())
            {
                
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();

                context.Contas.Add(new ContaCorrente(1001, 100m, cliente.Id));
                context.Contas.Add(new ContaPoupanca(1002, 500m, cliente.Id));
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CriarContexto())
            {
                var repo = new ContaRepository(context);
                var result = await repo.GetContasByClienteIdAsync(cliente.Id);

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, c => c.Tipo == TipoConta.Corrente);
                Assert.Contains(result, c => c.Tipo == TipoConta.Poupanca);
            }
        }

        [Fact]
        public async Task NumeroContaJaExisteAsync_QuandoExiste_DeveRetornarTrue()
        {
            // Arrange
            int numeroExistente = 12345;
            using (var context = CriarContexto())
            {
                context.Contas.Add(new ContaPoupanca(numeroExistente, 100m, Guid.NewGuid()));
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                var existe = await repository.NumeroContaJaExisteAsync(numeroExistente);

                // Assert
                Assert.True(existe);
            }
        }

        [Fact]
        public async Task AddAsync_DeveSalvarContaNoBanco()
        {
            // Arrange
            var novaConta = new ContaPoupanca(999, 0m, Guid.NewGuid());

            // Act
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                await repository.AddAsync(novaConta);
                await repository.SaveChangesAsync();
            }

            using (var context = CriarContexto())
            {
                var contaSalva = await context.Contas.FirstOrDefaultAsync(c => c.Numero == 999);
                Assert.NotNull(contaSalva);
                Assert.Equal(999, contaSalva.Numero);
            }
        }
        [Fact]
        public async Task Delete_DeveRemoverContaDoBanco()
        {
            // Arrange
            Guid contaId;
            using (var context = CriarContexto())
            {
                Cliente cliente = CriarClienteMock();
                context.Clientes.Add(cliente);

                var conta = new ContaCorrente(888, 0m, cliente.Id);
                context.Contas.Add(conta);
                await context.SaveChangesAsync();
                contaId = conta.Id;
            }
            // Act
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);

                var contaParaDeletar = await repository.GetByIdAsync(contaId);

                Assert.NotNull(contaParaDeletar);

                repository.Delete(contaParaDeletar!);
                await repository.SaveChangesAsync();
            }
            using (var context = CriarContexto())
            {
                var contaDeletada = await context.Contas.FirstOrDefaultAsync(c => c.Id == contaId);
                Assert.Null(contaDeletada);
            }
        }
        [Fact]
        public async Task GetByIdAsync_QuandoContaExiste_DeveRetornarConta()
        {
            // Arrange
            Guid contaId;
            using (var context = CriarContexto())
            {
                Cliente cliente = CriarClienteMock();
                context.Clientes.Add(cliente);

                var conta = new ContaCorrente(777, 200m, cliente.Id);
                context.Contas.Add(conta);
                await context.SaveChangesAsync();
                contaId = conta.Id;
            }
            // Act
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                var contaEncontrada = await repository.GetByIdAsync(contaId);
                // Assert
                Assert.NotNull(contaEncontrada);
                Assert.Equal(777, contaEncontrada.Numero);
            }
        }
        [Fact]
        public async Task GetByIdAsync_QuandoContaNaoExiste_DeveRetornarNull()
        {
            // Arrange
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                // Act
                var contaEncontrada = await repository.GetByIdAsync(Guid.NewGuid());
                // Assert
                Assert.Null(contaEncontrada);
            }
        }
        [Fact]
        public async Task SaveChangesAsync_QuandoAlteracoesFeitas_DeveRetornarTrue()
        {
            // Arrange
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                var novaConta = new ContaPoupanca(666, 300m, Guid.NewGuid());
                await repository.AddAsync(novaConta);
                // Act
                var resultado = await repository.SaveChangesAsync();
                // Assert
                Assert.True(resultado);
            }
        }
        [Fact]
        public async Task GetAllAsync_DeveRetornarContasComPaginacao()
        {
            // Arrange
            using (var context = CriarContexto())
            {
                Cliente cliente = CriarClienteMock();
                context.Clientes.Add(cliente);
                await context.SaveChangesAsync();

                for (int i = 1; i <= 10; i++)
                {
                    context.Contas.Add(new ContaCorrente(1000 + i, i * 100m, cliente.Id));
                }
                await context.SaveChangesAsync();
            }
            // Act
            using (var context = CriarContexto())
            {
                var repository = new ContaRepository(context);
                var contasPagina2 = await repository.GetAllAsync(pageNumber: 2, pageSize: 3);
                // Assert
                Assert.Equal(3, contasPagina2.Count());
                Assert.Equal(1004, contasPagina2.First().Numero); // Deve começar do 4º registro
            }
        }
    }
}