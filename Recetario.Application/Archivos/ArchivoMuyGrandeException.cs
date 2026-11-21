namespace Recetario.Application.Archivos;

/// <summary>Se traduce a HTTP 413: el archivo supera el tamaño máximo permitido.</summary>
public sealed class ArchivoMuyGrandeException(string mensaje) : Exception(mensaje);
