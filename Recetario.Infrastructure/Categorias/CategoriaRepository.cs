using Microsoft.EntityFrameworkCore;
using Recetario.Application.Categorias;
using Recetario.Infrastructure.Persistencia;

namespace Recetario.Infrastructure.Categorias;

public sealed class CategoriaRepository(AppDbContext contexto) : ICategoriaRepository
{
    public Task<List<CategoriaDto>> ObtenerTodasAsync(CancellationToken cancellationToken) =>
        contexto.Categorias.AsNoTracking()
            .OrderBy(categoria => categoria.Id)
            .Select(categoria => new CategoriaDto(categoria.Id, categoria.Nombre))
            .ToListAsync(cancellationToken);
}
