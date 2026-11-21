using Recetario.Application.Archivos;
using Recetario.Domain;

namespace Recetario.Application.Recetas;

public sealed class RecetaMultimediaService(IRecetaRepository repositorio, IFileStorage almacenamiento)
    : IRecetaMultimediaService
{
    private const string CarpetaImagenes = "imagenes";
    private const string CarpetaVideos = "videos";

    // 5 MB en Base64 son cerca de 7 millones de caracteres. Se deja margen para saltos de línea y para el
    // prefijo "data:image/...;base64," y así se evita decodificar textos enormes antes de rechazarlos.
    private const int MaxCaracteresBase64 = 7_500_000;

    private static readonly HashSet<string> ExtensionesImagen = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly byte[] FirmaPng = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    // ---------------------------------------------------------------- Imagen (Base64)

    public async Task<ArchivoGuardadoDto> GuardarImagenBase64Async(int id, ImagenBase64Dto dto,
        CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);

        if (string.IsNullOrWhiteSpace(dto.NombreArchivo))
            throw new ArchivoInvalidoException("El nombre del archivo es obligatorio.");
        if (string.IsNullOrWhiteSpace(dto.ImagenBase64))
            throw new ArchivoInvalidoException("La imagen en Base64 es obligatoria.");

        var texto = QuitarPrefijoDataUri(dto.ImagenBase64.Trim());
        if (texto.Length > MaxCaracteresBase64)
            throw new ArchivoMuyGrandeException("La imagen supera el máximo permitido de 5 MB.");

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(texto);
        }
        catch (FormatException)
        {
            throw new ArchivoInvalidoException("El texto recibido no es un Base64 válido.");
        }

        if (bytes.Length == 0)
            throw new ArchivoInvalidoException("La imagen está vacía.");
        if (bytes.Length > LimitesArchivo.MaxImagenBytes)
            throw new ArchivoMuyGrandeException("La imagen supera el máximo permitido de 5 MB.");

        // La extensión del nombre solo se revisa; el archivo se guarda con la extensión del formato real.
        var extensionDeclarada = Path.GetExtension(dto.NombreArchivo.Trim()).ToLowerInvariant();
        if (extensionDeclarada.Length > 0 && !ExtensionesImagen.Contains(extensionDeclarada))
            throw new FormatoNoPermitidoException("Solo se permiten imágenes JPG, PNG o WebP.");

        var extension = DetectarExtensionImagen(bytes)
            ?? throw new FormatoNoPermitidoException("Solo se permiten imágenes JPG, PNG o WebP.");

        string rutaNueva;
        await using (var contenido = new MemoryStream(bytes, writable: false))
            rutaNueva = await almacenamiento.GuardarAsync(contenido, CarpetaImagenes, extension, cancellationToken);

        await PersistirReemplazandoAsync(rutaNueva, receta.ImagenRuta, () => receta.ImagenRuta = rutaNueva,
            cancellationToken);
        return new ArchivoGuardadoDto(rutaNueva);
    }

    public async Task<ArchivoDescarga> ObtenerImagenAsync(int id, CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);
        var rutaFisica = receta.ImagenRuta is null ? null : almacenamiento.ObtenerRutaFisica(receta.ImagenRuta);
        if (rutaFisica is null)
            throw new ArchivoNoEncontradoException($"La receta con identificador {id} no tiene imagen.");
        return new ArchivoDescarga(rutaFisica, TipoContenidoImagen(rutaFisica));
    }

    public async Task EliminarImagenAsync(int id, CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);
        var ruta = receta.ImagenRuta
            ?? throw new ArchivoNoEncontradoException($"La receta con identificador {id} no tiene imagen.");
        receta.ImagenRuta = null;
        await repositorio.GuardarAsync(cancellationToken);
        await almacenamiento.EliminarAsync(ruta, cancellationToken);
    }

    // ---------------------------------------------------------------- Video (multipart)

    public async Task<ArchivoGuardadoDto> GuardarVideoAsync(int id, Stream contenido, long longitud,
        string nombreArchivo, CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);

        if (longitud <= 0)
            throw new ArchivoInvalidoException("No se recibió ningún video o el archivo está vacío.");
        if (longitud > LimitesArchivo.MaxVideoBytes)
            throw new ArchivoMuyGrandeException("El video supera el máximo permitido de 30 MB.");

        if (!string.Equals(Path.GetExtension(nombreArchivo), ".mp4", StringComparison.OrdinalIgnoreCase))
            throw new FormatoNoPermitidoException("Solo se permite video en formato MP4.");

        var cabecera = new byte[12];
        var leidos = await LeerCabeceraAsync(contenido, cabecera, cancellationToken);
        if (!EsMp4(cabecera.AsSpan(0, leidos)))
            throw new FormatoNoPermitidoException("El archivo no es un video MP4 válido.");

        // El flujo de IFormFile permite posicionarse; se regresa al inicio para guardar el archivo completo.
        contenido.Seek(0, SeekOrigin.Begin);
        var rutaNueva = await almacenamiento.GuardarAsync(contenido, CarpetaVideos, ".mp4", cancellationToken);

        await PersistirReemplazandoAsync(rutaNueva, receta.VideoRuta, () => receta.VideoRuta = rutaNueva,
            cancellationToken);
        return new ArchivoGuardadoDto(rutaNueva);
    }

    public async Task<ArchivoDescarga> ObtenerVideoAsync(int id, CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);
        var rutaFisica = receta.VideoRuta is null ? null : almacenamiento.ObtenerRutaFisica(receta.VideoRuta);
        if (rutaFisica is null)
            throw new ArchivoNoEncontradoException($"La receta con identificador {id} no tiene video.");
        return new ArchivoDescarga(rutaFisica, "video/mp4");
    }

    public async Task EliminarVideoAsync(int id, CancellationToken cancellationToken)
    {
        var receta = await ObtenerRecetaAsync(id, cancellationToken);
        var ruta = receta.VideoRuta
            ?? throw new ArchivoNoEncontradoException($"La receta con identificador {id} no tiene video.");
        receta.VideoRuta = null;
        await repositorio.GuardarAsync(cancellationToken);
        await almacenamiento.EliminarAsync(ruta, cancellationToken);
    }

    // ---------------------------------------------------------------- Auxiliares

    private async Task<Receta> ObtenerRecetaAsync(int id, CancellationToken cancellationToken) =>
        await repositorio.ObtenerEntidadPorIdAsync(id, cancellationToken)
        ?? throw new RecetaNoEncontradaException(id);

    // Registra la ruta nueva en la base de datos y recién después borra el archivo anterior: si falla el
    // guardado, no se pierde el archivo que ya tenía la receta y se limpia el recién escrito.
    private async Task PersistirReemplazandoAsync(string rutaNueva, string? rutaAnterior, Action asignar,
        CancellationToken cancellationToken)
    {
        try
        {
            asignar();
            await repositorio.GuardarAsync(cancellationToken);
        }
        catch
        {
            await almacenamiento.EliminarAsync(rutaNueva, CancellationToken.None);
            throw;
        }

        if (rutaAnterior is not null)
            await almacenamiento.EliminarAsync(rutaAnterior, cancellationToken);
    }

    private static string QuitarPrefijoDataUri(string texto)
    {
        if (!texto.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            return texto;
        var coma = texto.IndexOf(',');
        return coma < 0 ? texto : texto[(coma + 1)..];
    }

    // Se identifica el formato por los primeros bytes del archivo (firma), no por el nombre que declara el cliente.
    private static string? DetectarExtensionImagen(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return ".jpg";
        if (bytes.Length >= FirmaPng.Length && bytes[..FirmaPng.Length].SequenceEqual(FirmaPng))
            return ".png";
        if (bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("WEBP"u8))
            return ".webp";
        return null;
    }

    private static bool EsMp4(ReadOnlySpan<byte> cabecera) =>
        cabecera.Length >= 12 && cabecera.Slice(4, 4).SequenceEqual("ftyp"u8);

    private static async Task<int> LeerCabeceraAsync(Stream contenido, byte[] cabecera,
        CancellationToken cancellationToken)
    {
        var total = 0;
        while (total < cabecera.Length)
        {
            var leidos = await contenido.ReadAsync(cabecera.AsMemory(total), cancellationToken);
            if (leidos == 0)
                break;
            total += leidos;
        }
        return total;
    }

    private static string TipoContenidoImagen(string ruta) => Path.GetExtension(ruta).ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => "application/octet-stream"
    };
}
