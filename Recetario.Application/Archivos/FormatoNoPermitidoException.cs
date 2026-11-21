namespace Recetario.Application.Archivos;

/// <summary>Se traduce a HTTP 415: el formato del archivo no está permitido.</summary>
public sealed class FormatoNoPermitidoException(string mensaje) : Exception(mensaje);
