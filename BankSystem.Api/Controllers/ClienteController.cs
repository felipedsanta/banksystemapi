using BankSystem.Api.Service;
using BankSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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
        var validacao = await ValidarAcessoClienteAsync(id);
        if (validacao != null) return validacao;

        var viewModel = await clienteService.GetClientePorIdAsync(id);

        if (viewModel == null) return NotFound();

        return Ok(viewModel);
    }

    [HttpGet("{id:guid}/contas")]
    public async Task<IActionResult> GetContasDoCliente(Guid id)
    {
        var validacao = await ValidarAcessoClienteAsync(id);
        if (validacao != null) return validacao;

        var viewModels = await clienteService.GetContasDoClienteAsync(id);

        return Ok(viewModels);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarCliente(Guid id)
    {
        var validacao = await ValidarAcessoClienteAsync(id);
        if (validacao != null) return validacao;

        await clienteService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:guid}/restaurar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestaurarCliente(Guid id)
    {
        var usuarioRole = GetUsuarioRole();
        if (usuarioRole != "Admin")
        {
            return StatusCode(403, new { Mensagem = "Apenas administradores podem restaurar registros." });
        }

        await clienteService.RestaurarClienteAsync(id);
        return NoContent();
    }

    private async Task<IActionResult?> ValidarAcessoClienteAsync(Guid clienteId)
    {
        // Busca o cliente
        var cliente = await clienteService.GetClientePorIdAsync(clienteId);

        if (cliente == null)
        {
            return NotFound(new { Mensagem = "Cliente não encontrado." });
        }

        var usuarioLogadoId = GetUsuarioIdDoToken();
        var usuarioRole = GetUsuarioRole();

        if (usuarioRole != "Admin" && cliente.Id != usuarioLogadoId)
        {
            return StatusCode(403, new { Mensagem = "Você não tem permissão para acessar esse cliente." });
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