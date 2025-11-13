using BankSystem.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController(BankSystemDbContext context) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> CheckHealth()
        {
            try
            {
                bool canConnect = await context.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new { Status = "Healthy", Message = "Conexão com o banco de dados OK." });
                }
                else
                {
                    return StatusCode(503, new { Status = "Unhealthy", Message = "Não foi possível conectar ao banco de dados." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Message = "Erro ao verificar o banco de dados.",
                    Error = ex.Message
                });
            }
        }
    }
}