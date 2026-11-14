using Microsoft.EntityFrameworkCore;
using Recetario.Application.Recetas;
using Recetario.Domain;
using Recetario.Infrastructure.Persistencia;

namespace Recetario.Infrastructure.Recetas;

public sealed class RecetaRepository(AppDbContext contexto) : IRecetaRepository
{
    public async Task<ResultadoPaginado<RecetaListaDto>> ObtenerAsync(string? busqueda, int? categoriaId,
        Dificultad? dificultad, int pagina, int tamanoPagina, CancellationToken cancellationToken)
    {
        var consulta = contexto.Recetas.AsNoTracking().Include(receta => receta.Categoria).AsQueryable();
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            var termino = busqueda.Trim();
            consulta = consulta.Where(receta => receta.Nombre.Contains(termino)
                || receta.Ingredientes.Contains(termino));
        }
        if (categoriaId.HasValue)
            consulta = consulta.Where(receta => receta.CategoriaId == categoriaId.Value);
        if (dificultad.HasValue)
            consulta = consulta.Where(receta => receta.Dificultad == dificultad.Value);

        var total = await consulta.CountAsync(cancellationToken);
        var elementos = await consulta.OrderBy(receta => receta.Nombre)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(receta => new RecetaListaDto(receta.Id, receta.Nombre, receta.CategoriaId,
                receta.Categoria.Nombre, receta.Dificultad, receta.TiempoPreparacionMin, receta.Porciones))
            .ToListAsync(cancellationToken);
        return new ResultadoPaginado<RecetaListaDto>(elementos, total, pagina, tamanoPagina);
    }

    public Task<RecetaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken) =>
        contexto.Recetas.AsNoTracking().Include(receta => receta.Categoria)
            .Where(receta => receta.Id == id)
            .Select(receta => new RecetaDto(receta.Id, receta.Nombre, receta.CategoriaId,
                receta.Categoria.Nombre, receta.Dificultad, receta.TiempoPreparacionMin, receta.Porciones,
                receta.Ingredientes, receta.Pasos, receta.FechaCreacion))
            .SingleOrDefaultAsync(cancellationToken);

    public Task<Receta?> ObtenerEntidadPorIdAsync(int id, CancellationToken cancellationToken) =>
        contexto.Recetas.SingleOrDefaultAsync(receta => receta.Id == id, cancellationToken);

    public Task<bool> ExisteCategoriaAsync(int categoriaId, CancellationToken cancellationToken) =>
        contexto.Categorias.AsNoTracking().AnyAsync(categoria => categoria.Id == categoriaId, cancellationToken);

    public Task<bool> ExisteNombreAsync(string nombre, int? excluirId, CancellationToken cancellationToken)
    {
        var nombreNormalizado = nombre.ToUpper();
        var consulta = contexto.Recetas.AsNoTracking()
            .Where(receta => receta.Nombre.ToUpper() == nombreNormalizado);
        if (excluirId.HasValue)
            consulta = consulta.Where(receta => receta.Id != excluirId.Value);
        return consulta.AnyAsync(cancellationToken);
    }

    public async Task<RecetaDto> CrearAsync(Receta receta, CancellationToken cancellationToken)
    {
        contexto.Recetas.Add(receta);
        await contexto.SaveChangesAsync(cancellationToken);
        return (await ObtenerPorIdAsync(receta.Id, cancellationToken))!;
    }

    public async Task GuardarAsync(CancellationToken cancellationToken) =>
        await contexto.SaveChangesAsync(cancellationToken);

    public async Task EliminarAsync(Receta receta, CancellationToken cancellationToken)
    {
        contexto.Recetas.Remove(receta);
        await contexto.SaveChangesAsync(cancellationToken);
    }
}
