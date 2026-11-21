using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recetario.Application.Archivos;
using Recetario.Application.Categorias;
using Recetario.Application.Recetas;
using Recetario.Infrastructure.Archivos;
using Recetario.Infrastructure.Categorias;
using Recetario.Infrastructure.Persistencia;
using Recetario.Infrastructure.Recetas;

namespace Recetario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuracion,
        string rutaContenido)
    {
        var cadenaConexion = configuracion.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena ConnectionStrings:Default.");
        services.AddDbContext<AppDbContext>(opciones => opciones.UseSqlServer(cadenaConexion));
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IRecetaRepository, RecetaRepository>();

        // Carpeta donde se guardan imágenes y videos. Por defecto "Uploads" dentro de la carpeta del proyecto API;
        // se puede cambiar con Almacenamiento:RutaBase (ruta absoluta o relativa a la carpeta del proyecto).
        var rutaBase = Path.GetFullPath(configuracion["Almacenamiento:RutaBase"] ?? "Uploads", rutaContenido);
        services.AddSingleton<IFileStorage>(_ => new LocalFileStorage(rutaBase));
        return services;
    }
}
