using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Recetario.Application.Recetas;
using Recetario.Domain;

namespace Recetario.Api.Middleware;

public sealed class ManejoErroresMiddleware(RequestDelegate siguiente, ILogger<ManejoErroresMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await siguiente(contexto);
        }
        catch (Exception excepcion)
        {
            logger.LogError(excepcion, "Error no controlado al procesar la solicitud.");
            var (estado, titulo, detalle) = ObtenerProblema(excepcion);
            contexto.Response.Clear();
            contexto.Response.StatusCode = estado;
            contexto.Response.ContentType = "application/problem+json";
            var problema = new ProblemDetails
            {
                Status = estado,
                Title = titulo,
                Detail = detalle,
                Instance = contexto.Request.Path
            };
            await contexto.Response.WriteAsJsonAsync(problema, cancellationToken: contexto.RequestAborted);
        }
    }

    private static (int Estado, string Titulo, string Detalle) ObtenerProblema(Exception excepcion) => excepcion switch
    {
        DomainException => (StatusCodes.Status400BadRequest, "Regla de dominio incumplida", excepcion.Message),
        RecetaNoEncontradaException => (StatusCodes.Status404NotFound, "Recurso no encontrado", excepcion.Message),
        ConflictoException => (StatusCodes.Status409Conflict, "Conflicto", excepcion.Message),
        DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } } =>
            (StatusCodes.Status409Conflict, "Conflicto", "Ya existe un registro con ese valor único."),
        _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor", "Ocurrió un error inesperado.")
    };
}
