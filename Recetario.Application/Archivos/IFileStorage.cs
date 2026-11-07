namespace Recetario.Application.Archivos;

public interface IFileStorage
{
    Task<string> GuardarAsync(Stream archivo, string nombreArchivo, CancellationToken cancellationToken);
    Task EliminarAsync(string ruta, CancellationToken cancellationToken);
}
