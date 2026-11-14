using Recetario.Domain;

namespace Recetario.Application.Recetas;

public interface IRecetaRepository
{
    Task<ResultadoPaginado<RecetaListaDto>> ObtenerAsync(string? busqueda, int? categoriaId,
        Dificultad? dificultad, int pagina, int tamanoPagina, CancellationToken cancellationToken);
    Task<RecetaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);
    Task<Receta?> ObtenerEntidadPorIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExisteCategoriaAsync(int categoriaId, CancellationToken cancellationToken);
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken);
    Task<RecetaDto> CrearAsync(Receta receta, CancellationToken cancellationToken);
    Task GuardarAsync(CancellationToken cancellationToken);
    Task EliminarAsync(Receta receta, CancellationToken cancellationToken);
}
