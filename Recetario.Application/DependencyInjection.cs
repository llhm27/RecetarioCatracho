using Microsoft.Extensions.DependencyInjection;
using Recetario.Application.Categorias;
using Recetario.Application.Recetas;

namespace Recetario.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<IRecetaService, RecetaService>();
        return services;
    }
}
