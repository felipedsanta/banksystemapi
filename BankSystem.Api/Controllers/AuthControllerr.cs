using BankSystem.Api.Models.InputModels;
using BankSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel input)
        {
            var token = await authService.LoginAsync(input);

            if (token == null)
            {
                return Unauthorized(new { Mensagem = "Email ou senha inválidos." });
            }

            return Ok(new
            {
                Token = token,
                Mensagem = "Login realizado com sucesso!"
            });
        }
    }
}