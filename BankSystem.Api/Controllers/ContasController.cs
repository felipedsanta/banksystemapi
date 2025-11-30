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

            if (usuarioRole != "Admin")
            {
                inputModel.ClienteId = usuarioLogadoId;
            }

            var viewModel = await contaService.AddAsync(inputModel);
            return Created($"/api/contas/{viewModel.Id}", viewModel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ContaViewModel>))]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> Get( [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var viewModels = await contaService.GetAllContasAsync(pageNumber, pageSize);
            return Ok(viewModels);
        }

        [HttpGet("GetAllInclusiveDeleted")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ContaViewModel>))]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> GetAllInclusiveDeleted([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var viewModels = await contaService.GetAllContasInclusiveDeletedAsync(pageNumber, pageSize);
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var viewModel = await contaService.GetByIdAsync(id);
            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarConta(Guid id)
        {
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Sacar(Guid id, [FromBody] TransacaoInputModel input)
        {
            await bankService.SacarAsync(id, input);
            return NoContent();
        }

        [HttpPost("{id:guid}/transferencia")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Transferir(Guid id, [FromBody] TransferenciaInputModel input)
        {
            await bankService.TransferirAsync(id, input.ContaDestinoId, input);
            return NoContent();
        }

        [HttpGet("{id:guid}/extrato")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TransacaoViewModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExtrato(Guid id)
        {
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