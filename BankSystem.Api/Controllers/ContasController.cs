using BankSystem.Api.Models;
using BankSystem.Api.Repositories;
using BankSystem.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ContasController(IContaService contaService) : ControllerBase
    {


        [HttpPost]
        public async Task<ActionResult<ContaViewModel>> CreateConta([FromBody] ContaInputModel inputModel)
        {
            if (await contaService.NumeroContaJaExisteAsync(inputModel.Numero))
            {
                return Conflict($"Já existe uma conta com o número {inputModel.Numero}.");
            }

            var viewModel = await contaService.AddAsync(inputModel);
            return Created($"/api/contas/{viewModel.Id}", viewModel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ContaViewModel>))]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> Get( [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var viewModels = await contaService.GetContasAsync(pageNumber, pageSize);
            return Ok(viewModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var viewModel = await contaService.GetByIdAsync(id);
            return Ok(viewModel);
        }

        [HttpPut("{id}/deposito")]
        public async Task<IActionResult> Depositar(Guid id, [FromBody] DepositoInputModel input)
        { 
            var viewModel = await contaService.Depositar(id, input.Valor);
            return Ok(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarConta(Guid id)
        {
            await contaService.DeleteAsync(id);
            return NoContent();
        }
    }
}