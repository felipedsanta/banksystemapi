using BankSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClienteController(IClienteService clienteService) : ControllerBase
{

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CriarCliente([FromBody] ClienteInputModel input)
    {
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
        var viewModels = await clienteService.GetContasDoClienteAsync(id);

        return Ok(viewModels);
    }
}