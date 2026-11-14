namespace Recetario.Application.Recetas;

public sealed class RecetaNoEncontradaException(int id)
    : Exception($"No se encontró la receta con identificador {id}.");
