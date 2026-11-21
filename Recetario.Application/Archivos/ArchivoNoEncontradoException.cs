namespace Recetario.Application.Archivos;

/// <summary>Se traduce a HTTP 404: la receta no tiene imagen o video, o el archivo ya no está en disco.</summary>
public sealed class ArchivoNoEncontradoException(string mensaje) : Exception(mensaje);
