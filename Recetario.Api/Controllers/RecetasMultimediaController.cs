using Microsoft.AspNetCore.Mvc;
using Recetario.Application.Archivos;
using Recetario.Application.Recetas;

namespace Recetario.Api.Controllers;

[ApiController]
[Route("api/recetas/{id:int}")]
public sealed class RecetasMultimediaController(IRecetaMultimediaService servicio) : ControllerBase
{
    // ---------------------------------------------------------------- Imagen (JSON con Base64)

    [HttpPut("imagen-base64")]
    [RequestSizeLimit(LimitesArchivo.MaxCuerpoImagenBytes)]
    [ProducesResponseType(typeof(ArchivoGuardadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ArchivoGuardadoDto>> SubirImagen(int id, ImagenBase64Dto dto,
        CancellationToken cancellationToken) =>
        Ok(await servicio.GuardarImagenBase64Async(id, dto, cancellationToken));

    [HttpGet("imagen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerImagen(int id, CancellationToken cancellationToken)
    {
        var archivo = await servicio.ObtenerImagenAsync(id, cancellationToken);
        return PhysicalFile(archivo.RutaFisica, archivo.TipoContenido);
    }

    [HttpDelete("imagen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarImagen(int id, CancellationToken cancellationToken)
    {
        await servicio.EliminarImagenAsync(id, cancellationToken);
        return NoContent();
    }

    // ---------------------------------------------------------------- Video (multipart/form-data)

    [HttpPut("video")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(LimitesArchivo.MaxCuerpoVideoBytes)]
    [ProducesResponseType(typeof(ArchivoGuardadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    public async Task<ActionResult<ArchivoGuardadoDto>> SubirVideo(int id, IFormFile archivo,
        CancellationToken cancellationToken)
    {
        await using var contenido = archivo.OpenReadStream();
        return Ok(await servicio.GuardarVideoAsync(id, contenido, archivo.Length, archivo.FileName,
            cancellationToken));
    }

    [HttpGet("video")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status206PartialContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerVideo(int id, CancellationToken cancellationToken)
    {
        var archivo = await servicio.ObtenerVideoAsync(id, cancellationToken);
        // enableRangeProcessing atiende el encabezado Range (respuesta 206) para poder reproducir y avanzar el video.
        return PhysicalFile(archivo.RutaFisica, archivo.TipoContenido, enableRangeProcessing: true);
    }

    [HttpDelete("video")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarVideo(int id, CancellationToken cancellationToken)
    {
        await servicio.EliminarVideoAsync(id, cancellationToken);
        return NoContent();
    }
}
