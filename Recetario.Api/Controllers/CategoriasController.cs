using Microsoft.AspNetCore.Mvc;
using Recetario.Application.Categorias;

namespace Recetario.Api.Controllers;

[ApiController]
[Route("api/categorias")]
public sealed class CategoriasController(ICategoriaService servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoriaDto>>> ObtenerTodas(CancellationToken cancellationToken)
    {
        var categorias = await servicio.ObtenerTodasAsync(cancellationToken);
        return Ok(categorias);
    }
}
