namespace Recetario.Application.Categorias;

public sealed class CategoriaService(ICategoriaRepository repositorio) : ICategoriaService
{
    public Task<List<CategoriaDto>> ObtenerTodasAsync(CancellationToken cancellationToken) =>
        repositorio.ObtenerTodasAsync(cancellationToken);
}
