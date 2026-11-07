using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recetario.Application.Categorias;
using Recetario.Infrastructure.Categorias;
using Recetario.Infrastructure.Persistencia;

namespace Recetario.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Falta la cadena ConnectionStrings:Default.");
        services.AddDbContext<AppDbContext>(opciones => opciones.UseSqlServer(cadenaConexion));
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        return services;
    }
}
