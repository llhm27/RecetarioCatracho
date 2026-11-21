namespace Recetario.Application.Archivos;

public static class LimitesArchivo
{
    public const int MaxImagenBytes = 5 * 1024 * 1024;
    public const long MaxVideoBytes = 30L * 1024 * 1024;

    // Límites del cuerpo completo de la solicitud (con margen para el JSON/Base64 y los encabezados multipart).
    // Base64 aumenta el tamaño en aproximadamente un tercio: 5 MB de imagen son cerca de 7 MB de texto.
    public const long MaxCuerpoImagenBytes = 10L * 1024 * 1024;
    public const long MaxCuerpoVideoBytes = 35L * 1024 * 1024;
}
