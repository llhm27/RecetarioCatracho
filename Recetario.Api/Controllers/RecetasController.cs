using Microsoft.AspNetCore.Mvc;
using Recetario.Application.Recetas;
using Recetario.Domain;

namespace Recetario.Api.Controllers;

[ApiController]
[Route("api/recetas")]
public sealed class RecetasController(IRecetaService servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ResultadoPaginado<RecetaListaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResultadoPaginado<RecetaListaDto>>> Obtener(
        [FromQuery] string? search, [FromQuery] int? categoriaId, [FromQuery] Dificultad? dificultad,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 50)
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Parámetros de paginación no válidos.",
                Detail = "La página debe ser mayor o igual a 1 y pageSize debe estar entre 1 y 50."
            });

        return Ok(await servicio.ObtenerAsync(search, categoriaId, dificultad, page, pageSize, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RecetaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecetaDto>> ObtenerPorId(int id, CancellationToken cancellationToken) =>
        Ok(await servicio.ObtenerPorIdAsync(id, cancellationToken));

    [HttpPost]
    [ProducesResponseType(typeof(RecetaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RecetaDto>> Crear(CrearRecetaDto dto, CancellationToken cancellationToken)
    {
        var receta = await servicio.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = receta.Id }, receta);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, ActualizarRecetaDto dto, CancellationToken cancellationToken)
    {
        await servicio.ActualizarAsync(id, dto, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        await servicio.EliminarAsync(id, cancellationToken);
        return NoContent();
    }
}
