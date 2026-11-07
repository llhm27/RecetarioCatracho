namespace Recetario.Domain;

public sealed class DomainException(string mensaje) : Exception(mensaje);
