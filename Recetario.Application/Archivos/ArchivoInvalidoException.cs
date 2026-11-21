namespace Recetario.Application.Archivos;

/// <summary>Se traduce a HTTP 400: el contenido recibido no es válido (por ejemplo, Base64 mal formado).</summary>
public sealed class ArchivoInvalidoException(string mensaje) : Exception(mensaje);
