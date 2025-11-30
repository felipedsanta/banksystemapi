using BankSystem.Api.Models.InputModels;
using BankSystem.Api.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankSystem.Api.Services
{
    public class AuthService(IClienteRepository clienteRepo, IConfiguration config) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginInputModel input)
        {
            var cliente = await clienteRepo.GetByCpfAsync(input.Cpf);

            if (cliente == null) return null;

            bool senhaValida = BCrypt.Net.BCrypt.Verify(input.Senha, cliente.SenhaHash);

            if (!senhaValida) return null;

            return GerarTokenJwt(cliente);
        }

        public string GerarHashSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        private string GerarTokenJwt(Models.Cliente cliente)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtKey = config["Jwt:Key"] ?? throw new Exception("Chave JWT ausente");

            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                    new Claim(ClaimTypes.Name, cliente.Nome),
                    new Claim(ClaimTypes.Email, cliente.Email),
                    new Claim(ClaimTypes.Role, cliente.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Token expira em 2 horas
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}