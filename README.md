# Recetario Catracho

API REST para consultar y compartir recetas hondureñas, construida con .NET 10, ASP.NET Core, Entity Framework Core y SQL Server, siguiendo arquitectura limpia.

## Requisitos

- SDK de .NET 10
- SQL Server local (por ejemplo, SQL Server Express o LocalDB)
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef --version 10.0.0`

## Configurar la conexión

La cadena se almacena en User Secrets y no en `appsettings.json`:

```powershell
dotnet user-secrets init --project .\Recetario.Api\Recetario.Api.csproj
dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\MSSQLLocalDB;Database=RecetarioCatracho;Trusted_Connection=True;TrustServerCertificate=True" --project .\Recetario.Api\Recetario.Api.csproj
```

Si se usa una instancia distinta de SQL Server, cambia el valor `Server=...`.

## Crear la base de datos

Desde la carpeta raíz:

```powershell
dotnet ef migrations add Inicial --project .\Recetario.Infrastructure\Recetario.Infrastructure.csproj --startup-project .\Recetario.Api\Recetario.Api.csproj --output-dir Persistencia\Migraciones
dotnet ef database update --project .\Recetario.Infrastructure\Recetario.Infrastructure.csproj --startup-project .\Recetario.Api\Recetario.Api.csproj
```

## Ejecutar

```powershell
dotnet restore .\Recetario.sln
dotnet build .\Recetario.sln
dotnet run --project .\Recetario.Api\Recetario.Api.csproj
```

Consulta `GET /api/categorias`. En Development, OpenAPI está en `/openapi/v1.json` y Scalar en `/scalar/v1`.

## Capas

- **Recetario.Domain**: entidades, enum y reglas del negocio; no depende de otras capas.
- **Recetario.Application**: contratos, DTO y servicios; depende del dominio.
- **Recetario.Infrastructure**: SQL Server, configuración EF, datos semilla y repositorios; implementa contratos de Application.
- **Recetario.Api**: entrada HTTP, controlador, middleware y composición de dependencias.

## Commits y publicación

Ejecuta cada bloque tras preparar los archivos indicados con `git add`:

```powershell
git add Recetario.sln Recetario.Domain/Recetario.Domain.csproj Recetario.Application/Recetario.Application.csproj Recetario.Infrastructure/Recetario.Infrastructure.csproj Recetario.Api/Recetario.Api.csproj
git commit -m "chore: crear solución y estructura de capas"

git add Recetario.Domain
git commit -m "feat(domain): agregar entidades y reglas de recetas"

git add Recetario.Infrastructure/Persistencia
git commit -m "feat(infrastructure): configurar DbContext y migración inicial"

git add Recetario.Application Recetario.Infrastructure/Categorias Recetario.Infrastructure/DependencyInjection.cs
git commit -m "feat(application): agregar repositorio e inyección de dependencias"

git add Recetario.Api
git commit -m "feat(api): publicar endpoint de categorías"

git add README.md .gitignore
git commit -m "docs: agregar instrucciones y gitignore"

git remote add origin https://github.com/USUARIO/Recetario.git
git branch -M main
git push -u origin main
```
