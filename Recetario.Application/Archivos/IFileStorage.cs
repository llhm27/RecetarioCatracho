namespace Recetario.Application.Archivos;

public interface IFileStorage
{
    /// <summary>
    /// Guarda el contenido en disco con un nombre generado (Guid) dentro de la carpeta indicada
    /// y devuelve la ruta relativa, por ejemplo "imagenes/3f2c9a....jpg". Esa ruta es la que se
    /// registra en la base de datos.
    /// </summary>
    Task<string> GuardarAsync(Stream archivo, string carpeta, string extension, CancellationToken cancellationToken);

    /// <summary>Elimina el archivo del disco. No falla si el archivo ya no existe.</summary>
    Task EliminarAsync(string ruta, CancellationToken cancellationToken);

    /// <summary>Devuelve la ruta física absoluta del archivo, o null si no existe.</summary>
    string? ObtenerRutaFisica(string ruta);
}
