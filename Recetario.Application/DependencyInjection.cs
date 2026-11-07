using Microsoft.Extensions.DependencyInjection;
using Recetario.Application.Categorias;

namespace Recetario.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoriaService, CategoriaService>();
        return services;
    }
}
