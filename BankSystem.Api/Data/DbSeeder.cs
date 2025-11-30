using BankSystem.Api.Models;
using BankSystem.Api.Models.ValueObjects;
using BankSystem.Api.Services; // Para acessar o AuthService (gerar hash)

namespace BankSystem.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(BankSystemDbContext context, IAuthService authService)
    {
        if (context.Clientes.Any())
        {
            return; 
        }

        var senhaHashPadrao = authService.GerarHashSenha("123456Ff!");

        var clientes = new List<Cliente>
        {
            new Cliente(
                "Admin Sistema", 
                "00000000000", 
                new DateTime(1980, 1, 1), 
                "admin@bank.com", 
                senhaHashPadrao, 
                "11900000000", 
                50000, 
                new Endereco("00000000", "Rua Admin", "1", null, "Matrix", "São Paulo", "SP"),
                "Admin" // Role
            ),
            new Cliente(
                "João Silva", 
                "11111111111", 
                new DateTime(1995, 5, 20), 
                "joao@email.com", 
                senhaHashPadrao, 
                "11911111111", 
                3500, 
                new Endereco("01310100", "Av Paulista", "1000", "Ap 10", "Bela Vista", "São Paulo", "SP")
            ),
            new Cliente(
                "Maria Souza", 
                "22222222222", 
                new DateTime(1988, 10, 12), 
                "maria@email.com", 
                senhaHashPadrao, 
                "21922222222", 
                15000, 
                new Endereco("20000000", "Av Atlantica", "500", null, "Copacabana", "Rio de Janeiro", "RJ")
            )
        };

        context.Clientes.AddRange(clientes);
        await context.SaveChangesAsync();
    }
}