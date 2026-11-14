namespace Recetario.Application.Recetas;

public sealed class ConflictoException(string mensaje) : Exception(mensaje);
