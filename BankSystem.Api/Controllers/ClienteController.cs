using BankSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class ClienteController(IClienteService clienteService) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteInputModel input)
    {
        if (await clienteService.CpfJaExisteAsync(input.Cpf))
        {
            return Conflict(new { Mensagem = "Um cliente com este CPF já existe." });
        }

        var viewModel = await clienteService.CriarClienteAsync(input);

        return CreatedAtAction(nameof(GetClientePorId), new { id = viewModel.Id }, viewModel);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClientePorId(Guid id)
    {
        var viewModel = await clienteService.GetClientePorIdAsync(id);

        if (viewModel == null) return NotFound();

        return Ok(viewModel);
    }

    [HttpGet("{id:guid}/contas")]
    public async Task<IActionResult> GetContasDoCliente(Guid id)
    {
        if (!await clienteService.ClienteExisteAsync(id))
        {
            return NotFound(new { Mensagem = "Cliente não encontrado." });
        }

        var viewModels = await clienteService.GetContasDoClienteAsync(id);

        return Ok(viewModels);
    }
}