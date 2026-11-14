using Recetario.Domain;

namespace Recetario.Application.Recetas;

public sealed record RecetaDto(int Id, string Nombre, int CategoriaId, string CategoriaNombre,
    Dificultad Dificultad, int TiempoPreparacionMin, int Porciones, string Ingredientes,
    string Pasos, DateTime FechaCreacion);
