using Recetario.Application.Archivos;

namespace Recetario.Application.Recetas;

public interface IRecetaMultimediaService
{
    Task<ArchivoGuardadoDto> GuardarImagenBase64Async(int id, ImagenBase64Dto dto, CancellationToken cancellationToken);
    Task<ArchivoDescarga> ObtenerImagenAsync(int id, CancellationToken cancellationToken);
    Task EliminarImagenAsync(int id, CancellationToken cancellationToken);

    Task<ArchivoGuardadoDto> GuardarVideoAsync(int id, Stream contenido, long longitud, string nombreArchivo,
        CancellationToken cancellationToken);
    Task<ArchivoDescarga> ObtenerVideoAsync(int id, CancellationToken cancellationToken);
    Task EliminarVideoAsync(int id, CancellationToken cancellationToken);
}
