using Recetario.Domain;

namespace Recetario.Application.Recetas;

public interface IRecetaService
{
    Task<ResultadoPaginado<RecetaListaDto>> ObtenerAsync(string? busqueda, int? categoriaId,
        Dificultad? dificultad, int pagina, int tamanoPagina, CancellationToken cancellationToken);
    Task<RecetaDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);
    Task<RecetaDto> CrearAsync(CrearRecetaDto dto, CancellationToken cancellationToken);
    Task ActualizarAsync(int id, ActualizarRecetaDto dto, CancellationToken cancellationToken);
    Task EliminarAsync(int id, CancellationToken cancellationToken);
}
