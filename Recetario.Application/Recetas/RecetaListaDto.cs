using Recetario.Domain;

namespace Recetario.Application.Recetas;

public sealed record RecetaListaDto(int Id, string Nombre, int CategoriaId, string CategoriaNombre,
    Dificultad Dificultad, int TiempoPreparacionMin, int Porciones);
