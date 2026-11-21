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

## Imagen y video de las recetas

Cada receta puede tener una imagen y un video. Los archivos se guardan en disco, en la carpeta `Uploads/` del proyecto API (se puede cambiar con `Almacenamiento:RutaBase` en `appsettings.json`), con un nombre generado con `Guid`. En la base de datos solo se registra la ruta (`ImagenRuta` y `VideoRuta`), nunca el contenido.

| Método | Ruta | Descripción |
|---|---|---|
| PUT | `/api/recetas/{id}/imagen-base64` | Recibe JSON `{ "nombreArchivo": "foto.jpg", "imagenBase64": "..." }`. Reemplaza la imagen anterior. |
| GET | `/api/recetas/{id}/imagen` | Devuelve el archivo con su tipo de contenido (`image/jpeg`, `image/png` o `image/webp`). |
| DELETE | `/api/recetas/{id}/imagen` | Elimina la imagen del disco y su ruta de la base de datos. |
| PUT | `/api/recetas/{id}/video` | `multipart/form-data` con el campo `archivo` (MP4). Reemplaza el video anterior. |
| GET | `/api/recetas/{id}/video` | Devuelve el MP4 con soporte de rangos (`Range` → `206 Partial Content`) para reproducirlo. |
| DELETE | `/api/recetas/{id}/video` | Elimina el video del disco y su ruta de la base de datos. |

Al eliminar una receta (`DELETE /api/recetas/{id}`) también se borran su imagen y su video del disco.

### Validaciones

| Código | Cuándo |
|---|---|
| 400 | El texto no es Base64 válido, o el archivo está vacío. |
| 413 | La imagen supera 5 MB o el video supera 30 MB. |
| 415 | La imagen no es JPG, PNG o WebP, o el video no es MP4. El formato se comprueba con los primeros bytes del archivo, no solo con su nombre. |
| 404 | La receta no existe, o no tiene imagen o video. |

El video se envía como `multipart/form-data` porque en Base64 el tamaño aumentaría aproximadamente un tercio.

### Pruebas en Postman

Importa `postman/Recetario-Multimedia.postman_collection.json`, ajusta las variables `baseUrl` y `recetaId`, y ejecuta las solicitudes en orden. En las solicitudes de video elige el archivo en **Body → form-data → archivo → Select Files**.

## Integrantes

- [NOMBRE]
- [NOMBRE]
- [NOMBRE]
