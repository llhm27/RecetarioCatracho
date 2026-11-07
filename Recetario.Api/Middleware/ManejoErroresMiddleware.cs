using Microsoft.AspNetCore.Mvc;
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
            contexto.Response.Clear();
            contexto.Response.StatusCode = excepcion is DomainException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError;
            contexto.Response.ContentType = "application/problem+json";
            var problema = new ProblemDetails
            {
                Status = contexto.Response.StatusCode,
                Title = excepcion is DomainException ? "Regla de dominio incumplida" : "Error interno del servidor",
                Detail = excepcion is DomainException ? excepcion.Message : "Ocurrió un error inesperado.",
                Instance = contexto.Request.Path
            };
            await contexto.Response.WriteAsJsonAsync(problema, cancellationToken: contexto.RequestAborted);
        }
    }
}
