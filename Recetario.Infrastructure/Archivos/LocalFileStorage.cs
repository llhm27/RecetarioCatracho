using Recetario.Application.Archivos;

namespace Recetario.Infrastructure.Archivos;

/// <summary>Guarda los archivos en una carpeta del disco del servidor, con nombres generados con Guid.</summary>
public sealed class LocalFileStorage : IFileStorage
{
    private static readonly StringComparison Comparacion =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    private readonly string _rutaBase;

    public LocalFileStorage(string rutaBase)
    {
        _rutaBase = Path.TrimEndingDirectorySeparator(Path.GetFullPath(rutaBase));
        Directory.CreateDirectory(_rutaBase);
    }

    public async Task<string> GuardarAsync(Stream archivo, string carpeta, string extension,
        CancellationToken cancellationToken)
    {
        var directorio = Path.Combine(_rutaBase, carpeta);
        Directory.CreateDirectory(directorio);

        var nombre = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var destino = Path.Combine(directorio, nombre);
        try
        {
            await using var salida = new FileStream(destino, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                bufferSize: 81920, useAsync: true);
            await archivo.CopyToAsync(salida, cancellationToken);
        }
        catch
        {
            if (File.Exists(destino))
                File.Delete(destino);
            throw;
        }

        // La ruta relativa es la que se registra en la base de datos (nunca el contenido del archivo).
        return $"{carpeta}/{nombre}";
    }

    public Task EliminarAsync(string ruta, CancellationToken cancellationToken)
    {
        var rutaFisica = ResolverRuta(ruta);
        if (rutaFisica is not null && File.Exists(rutaFisica))
            File.Delete(rutaFisica);
        return Task.CompletedTask;
    }

    public string? ObtenerRutaFisica(string ruta)
    {
        var rutaFisica = ResolverRuta(ruta);
        return rutaFisica is not null && File.Exists(rutaFisica) ? rutaFisica : null;
    }

    // Evita que una ruta con ".." salga de la carpeta de almacenamiento.
    private string? ResolverRuta(string ruta)
    {
        var completa = Path.GetFullPath(Path.Combine(_rutaBase, ruta));
        return completa.StartsWith(_rutaBase + Path.DirectorySeparatorChar, Comparacion) ? completa : null;
    }
}
