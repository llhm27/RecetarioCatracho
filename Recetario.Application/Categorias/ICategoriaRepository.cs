namespace Recetario.Application.Categorias;

public interface ICategoriaRepository
{
    Task<List<CategoriaDto>> ObtenerTodasAsync(CancellationToken cancellationToken);
}
