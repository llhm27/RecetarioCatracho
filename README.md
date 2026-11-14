# Recetario Catracho

Recetario Catracho es una API para consultar recetas hondureñas de la cafetería **Sabores de mi Tierra**. Es un proyecto de clase de **Programación para Dispositivos Móviles I** y está desarrollado con .NET 10, Entity Framework Core y SQL Server.

## Requisitos

- SDK de .NET 10.
- SQL Server, por ejemplo SQL Server Express o LocalDB.
- Herramienta de Entity Framework Core 10: `dotnet tool install --global dotnet-ef --version 10.0.0`.

## Configurar la conexión

La cadena de conexión se guarda en User Secrets para no incluir credenciales en los archivos de configuración. Desde la raíz del repositorio, inicializa los secretos del proyecto API y configura tu servidor:

```powershell
dotnet user-secrets init --project .\Recetario.Api\Recetario.Api.csproj
dotnet user-secrets set "ConnectionStrings:Default" "Server=(localdb)\MSSQLLocalDB;Database=RecetarioCatracho;Trusted_Connection=True;TrustServerCertificate=True" --project .\Recetario.Api\Recetario.Api.csproj
```

Si usas otra instancia, reemplaza el valor de `Server` por el correspondiente a tu SQL Server.

## Preparar y ejecutar el proyecto

Restaura los paquetes y compila la solución:

```powershell
dotnet restore .\Recetario.sln
dotnet build .\Recetario.sln
```

Aplica la migración existente para crear o actualizar la base de datos. Las migraciones están en `Recetario.Infrastructure/Persistencia/Migrations`:

```powershell
dotnet ef database update --project .\Recetario.Infrastructure\Recetario.Infrastructure.csproj --startup-project .\Recetario.Api\Recetario.Api.csproj
```

Inicia la API en el puerto 5009:

```powershell
dotnet run --project .\Recetario.Api\Recetario.Api.csproj --urls http://localhost:5009
```

Con la aplicación en ejecución, abre Scalar en [http://localhost:5009/scalar/](http://localhost:5009/scalar/).

## Capas del proyecto

- **Recetario.Domain** contiene las entidades y reglas del negocio, sin depender de las demás capas.
- **Recetario.Application** define los casos de uso, contratos, servicios y DTO.
- **Recetario.Infrastructure** implementa el acceso a SQL Server, la persistencia con EF Core y los repositorios.
- **Recetario.Api** expone los endpoints HTTP y configura el middleware y las dependencias de la aplicación.

## Integrantes

- [NOMBRE]
- [NOMBRE]
- [NOMBRE]
