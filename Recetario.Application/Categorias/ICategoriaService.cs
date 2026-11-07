namespace Recetario.Application.Categorias;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> ObtenerTodasAsync(CancellationToken cancellationToken);
}
