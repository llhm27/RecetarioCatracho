using Recetario.Domain;

namespace Recetario.Application.Recetas;

public sealed class RecetaService(IRecetaRepository repositorio) : IRecetaService
{
    public Task<ResultadoPaginado<RecetaListaDto>> ObtenerAsync(string? busqueda, int? categoriaId,
        Dificultad? dificultad, int pagina, int tamanoPagina, CancellationToken cancellationToken) =>
        repositorio.ObtenerAsync(busqueda, categoriaId, dificultad, pagina, tamanoPagina, cancellationToken);

    public async Task<RecetaDto> ObtenerPorIdAsync(int id, CancellationToken cancellationToken) =>
        await repositorio.ObtenerPorIdAsync(id, cancellationToken)
        ?? throw new RecetaNoEncontradaException(id);

    public async Task<RecetaDto> CrearAsync(CrearRecetaDto dto, CancellationToken cancellationToken)
    {
        await ValidarReferenciasYNombreAsync(dto.CategoriaId, dto.Nombre, null, cancellationToken);
        var receta = new Receta
        {
            Nombre = dto.Nombre.Trim(), CategoriaId = dto.CategoriaId, Dificultad = dto.Dificultad,
            TiempoPreparacionMin = dto.TiempoPreparacionMin, Porciones = dto.Porciones,
            Ingredientes = dto.Ingredientes, Pasos = dto.Pasos, FechaCreacion = DateTime.UtcNow
        };
        receta.ValidarReglas();
        return await repositorio.CrearAsync(receta, cancellationToken);
    }

    public async Task ActualizarAsync(int id, ActualizarRecetaDto dto, CancellationToken cancellationToken)
    {
        var receta = await repositorio.ObtenerEntidadPorIdAsync(id, cancellationToken)
            ?? throw new RecetaNoEncontradaException(id);
        await ValidarReferenciasYNombreAsync(dto.CategoriaId, dto.Nombre, id, cancellationToken);
        receta.Nombre = dto.Nombre.Trim();
        receta.CategoriaId = dto.CategoriaId;
        receta.Dificultad = dto.Dificultad;
        receta.TiempoPreparacionMin = dto.TiempoPreparacionMin;
        receta.Porciones = dto.Porciones;
        receta.Ingredientes = dto.Ingredientes;
        receta.Pasos = dto.Pasos;
        receta.ValidarReglas();
        await repositorio.GuardarAsync(cancellationToken);
    }

    public async Task EliminarAsync(int id, CancellationToken cancellationToken)
    {
        var receta = await repositorio.ObtenerEntidadPorIdAsync(id, cancellationToken)
            ?? throw new RecetaNoEncontradaException(id);
        await repositorio.EliminarAsync(receta, cancellationToken);
    }

    private async Task ValidarReferenciasYNombreAsync(int categoriaId, string nombre, int? excluirId,
        CancellationToken cancellationToken)
    {
        if (!await repositorio.ExisteCategoriaAsync(categoriaId, cancellationToken))
            throw new DomainException($"No existe la categoría con identificador {categoriaId}.");
        if (await repositorio.ExisteNombreAsync(nombre.Trim(), excluirId, cancellationToken))
            throw new ConflictoException($"Ya existe una receta con el nombre '{nombre.Trim()}'.");
    }
}
