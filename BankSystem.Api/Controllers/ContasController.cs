using BankSystem.Api.Models.ViewModels;
using BankSystem.Api.Service;
using BankSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BankSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContasController(IContaService contaService, IBankService bankService, IFinancialAdvisorService advisorService) : ControllerBase
    {


        [HttpPost]
        public async Task<ActionResult<ContaViewModel>> CreateConta([FromBody] ContaInputModel inputModel)
        {
            var usuarioLogadoId = GetUsuarioIdDoToken();
            var usuarioRole = GetUsuarioRole();

            if (usuarioRole != "Admin" && usuarioLogadoId != inputModel.ClienteId)
            {
                throw new InvalidOperationException("Não tem permissão para criar contas para outros clientes.");
            }

            var viewModel = await contaService.AddAsync(inputModel);
            return Created($"/api/contas/{viewModel.Id}", viewModel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ContaViewModel>))]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> Get( [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var usuarioLogadoId = GetUsuarioIdDoToken();
            var usuarioRole = GetUsuarioRole();

            if (usuarioRole != "Admin")
            {
                var viewModels = await contaService.GetContasByClienteIdAsync(usuarioLogadoId);
                return Ok(viewModels);
            }
            else
            {
                var viewModels = await contaService.GetAllContasAsync(pageNumber, pageSize);
                return Ok(viewModels);
            }
        }

        [HttpGet("GetAllInclusiveDeleted")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ContaViewModel>))]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> GetAllInclusiveDeleted([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var usuarioRole = GetUsuarioRole();

            if (usuarioRole == "Admin")
            {
                var viewModels = await contaService.GetAllContasInclusiveDeletedAsync(pageNumber, pageSize);
                return Ok(viewModels);
            }
            else
            {
                throw new InvalidOperationException("Não tem permissão para acessar essa rota.");
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var validacao = await ValidarAcessoContaAsync(id);
            if (validacao != null) return validacao;

            var viewModel = await contaService.GetByIdAsync(id);
            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarConta(Guid id)
        {
            var validacao = await ValidarAcessoContaAsync(id);
            if (validacao != null) return validacao;

            await contaService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/deposito")]
        public async Task<IActionResult> Depositar(Guid id, [FromBody] TransacaoInputModel input)
        { 
            var viewModel = await bankService.DepositarAsync(id, input);
            return Ok(viewModel);
        }

        [HttpPost("{id:guid}/saque")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Sacar(Guid id, [FromBody] TransacaoInputModel input)
        {
            var validacao = await ValidarAcessoContaAsync(id);
            if (validacao != null) return validacao;

            return Ok(await bankService.SacarAsync(id, input));

        }

        [HttpPost("{id:guid}/transferencia")]
        public async Task<IActionResult> Transferir(Guid id, [FromBody] TransferenciaInputModel input)
        {
            var validacao = await ValidarAcessoContaAsync(id);
            if (validacao != null) return validacao;

            await bankService.TransferirAsync(id, input.ContaDestinoId, input);
            return NoContent();
        }

        [HttpGet("{id:guid}/extrato")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TransacaoViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExtrato(Guid id)
        {
            var validacao = await ValidarAcessoContaAsync(id);
            if (validacao != null) return validacao;

            var transacoes = await bankService.GetExtratoAsync(id);

            var viewModels = transacoes.Select(t => new TransacaoViewModel
            {
                Id = t.Id,
                Tipo = t.Tipo,
                Valor = t.Valor,
                DataHora = t.DataHora,
                ContaOrigemId = t.ContaOrigemId,
                ContaDestinoId = t.ContaDestinoId
            });

            return Ok(viewModels);
        }

        [HttpGet("{id:guid}/analise-ia")]
        public async Task<IActionResult> GetAnaliseIA(Guid id)
        {
            var analise = await advisorService.GerarAnaliseFinanceiraAsync(id);

            return Ok(new
            {
                ContaId = id,
                DataAnalise = DateTime.Now,
                Conselho = analise
            });
        }

        private async Task<IActionResult?> ValidarAcessoContaAsync(Guid contaId)
        {
            //Busca a conta para saber quem é o dono
            var conta = await contaService.GetByIdAsync(contaId);

            if (conta == null)
            {
                return NotFound(new { Mensagem = "Conta não encontrada." });
            }

            var usuarioLogadoId = GetUsuarioIdDoToken();
            var usuarioRole = GetUsuarioRole();

            if (usuarioRole != "Admin" && conta.ClienteId != usuarioLogadoId)
            {
                return StatusCode(403, new { Mensagem = "Você não tem permissão para acessar ou movimentar esta conta." });
            }

            return null;
        }

        private Guid GetUsuarioIdDoToken()
        {
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idString!);
        }

        private string GetUsuarioRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "Cliente";
        }

    }
}